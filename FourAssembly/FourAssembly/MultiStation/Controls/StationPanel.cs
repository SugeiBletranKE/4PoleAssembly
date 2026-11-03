namespace FourAssembly.MultiStation.Controls;

using FourAssembly.Forms;
using FourAssembly.Models;
using FourAssembly.MultiStation.Models;
using FourAssembly.MultiStation.Services;
using FourAssembly.Services;
using System.Text.Json;

public partial class StationPanel : UserControl
{
    private enum ZebraValidationStep { STATION, BG, TOOLS, HOUSING, COMPLETE }

    private const int CognexSerialLength = 24;
    private static readonly TimeSpan NokCognexCooldown = TimeSpan.FromSeconds(5);

    private readonly StationSettings _config;
    private readonly PlcService _plc;
    private readonly string _stationCode;

    private StationProcessState _processState = StationProcessState.DISABLED;
    private ZebraValidationStep _zebraStep = ZebraValidationStep.STATION;
    private StationConfig? _stationConfig;
    private CurrentProcess? _currentProcess;

    private List<string> _scannedCables = [];
    private HashSet<string> _rejectedCables = new(StringComparer.OrdinalIgnoreCase);
    private HashSet<string> _blockedRejectedCables = new(StringComparer.OrdinalIgnoreCase);
    private int _cableCount = 0;
    private int _totalScannedCount = 0;
    private int _totalScrapCount = 0;
    private int _goodPieceCount = 0;
    private int _bgGoodPieceCount = 0;
    private string _goodPieceBg = string.Empty;
    private int _hourlyGoodPieceCount = 0;
    private DateTime _hourlyCounterStart = GetHourStart(DateTime.Now);
    private string _goodPiecePartNumber = string.Empty;
    private readonly HashSet<string> _countedGoodSerials = new(StringComparer.OrdinalIgnoreCase);
    private bool _apiApproved = false;
    private bool _waitingRejectedCableRemoval = false;
    private bool _rejectedCableRemovalPending = false;
    private bool _operatorActionInProgress = false;
    private bool _cognexSerialReviewInProgress = false;
    private string _pendingBookingSerial = string.Empty;
    private int? _completedBookingState = null;
    private bool _bookingInProgress = false;
    private bool _plcCounterSent = false;
    private bool _sharedPistonTransitionInProgress = false;
    private DateTime _cognexReadBlockedUntil = DateTime.MinValue;
    private bool _lastOperatorButtonSignal = false;
    private bool? _lastMaterialBoxPresentSignal = null;
    private bool _operatorButtonPollInProgress = false;
    private bool _plcDisconnectedNoticeShown = false;
    private bool _materialSensorRecheckInProgress = false;
    private bool _waitingForMaterialBoxInsertion = false;
    private string _toolValidatedPartNumber = string.Empty;
    private bool _zebraReleasedForActivePartNumber = false;
    private bool _bgMismatchActive = false;
    private bool _cognexAdminLocked = false;
    private bool _adminUnlockDialogOpen = false;
    private bool _adminLockRequiresRejectedRemoval = false;
    private bool _adminLockRequiresPlcQualityPulse = false;
    private string _adminLockCode = string.Empty;
    private string _adminLockMessage = string.Empty;
    private StationProcessState _processStateBeforeMaterialSensorRecheck = StationProcessState.DISABLED;
    private ZebraValidationStep _zebraStepBeforeMaterialSensorRecheck = ZebraValidationStep.STATION;
    private readonly System.Windows.Forms.Timer _operatorButtonPollTimer = new();
    private static readonly JsonSerializerOptions _apiInfoJsonOptions = new()
    {
        WriteIndented = true
    };

    public StationPanel(StationSettings config, PlcService plc)
    {
        _config = config;
        _plc = plc;
        _stationCode = config.StationNumber.ToString();

        InitializeComponent();
        pnlMasterStatus.Visible = _config.StationNumber == 3;
        _lblStation.Text = _config.Name;
        _lblScrapCount.BringToFront();
        _lblInsertedCount.BringToFront();
        _btnAdminRecoveryReset.BringToFront();
        lblInfo.Visible = true;
        lblInfo.BringToFront();

        SubscribeToEvents();
        StartOperatorButtonPolling();
        Reset();
    }

    private void SubscribeToEvents()
    {
        ProcessManager.ProcessStarted += OnProcessStarted;
        ProcessManager.StationStateChanged += OnStationStateChanged;
        ProcessManager.ProcessReset += OnProcessReset;
        ZebraFlowService.ZebraReadyForStation += OnZebraReadyForStation;
        ZebraFlowService.ZebraCompletedAtStation += OnZebraCompletedAtStation;
        CognexService.CableReceived += OnCognexDataReceived;
    }

    private void StartOperatorButtonPolling()
    {
        // Si no hay ninguna memoria para monitorear, no se activa el timer.
        if (_config.PlcButton <= 0
            && _config.PlcCounterCompleteMemory <= 0
            && _config.PlcMaterialRemovedMemory <= 0
            && _config.PlcCavityErrorMemory <= 0)
            return;

        // Revisa la bobina del boton cada 100 ms para detectar cuando el operador lo presiona.
        _operatorButtonPollTimer.Interval = 100;
        _operatorButtonPollTimer.Tick += OnOperatorButtonPollTimerTick;
        _operatorButtonPollTimer.Start();

        LogService.Log($"[Station {_stationCode}] Operator PLC button polling enabled on coil {_config.PlcButton}");
    }

    private async void OnOperatorButtonPollTimerTick(object? sender, EventArgs e)
    {
        // Evita que se encimen dos lecturas si una consulta al PLC tarda mas de 100 ms.
        if (_operatorButtonPollInProgress)
            return;

        _operatorButtonPollInProgress = true;

        try
        {
            // Lee la bobina del PLC donde esta conectado el boton fisico de esta estacion.
            // Lo que se espera recibir del PLC es:
            //   false / 0 = boton no presionado
            //   true  / 1 = boton presionado
            // El PLC debe volver a 0 cuando el operador suelte el boton.
            //Agregar memoria que me mandara Javi, para revisar pines y todo eso
            var buttonReadAttempted = _config.PlcButton > 0;
            var signal = buttonReadAttempted
                && await Task.Run(() => _plc.ReadCoil(_config.PlcButton));

            if (buttonReadAttempted && !_plc.IsConnected)
            {
                if (!_plcDisconnectedNoticeShown)
                {
                    _plcDisconnectedNoticeShown = true;
                    ShowInstruction("PLC no conectado. Revisar conexiones .", Color.Red);
                }
                return;
            }

            if (buttonReadAttempted && _plcDisconnectedNoticeShown)
            {
                _plcDisconnectedNoticeShown = false;
                UpdateUI();
            }

            // Solo se ejecuta al pasar de OFF a ON. Asi no se repite la accion
            // mientras el operador mantiene el boton presionado.
            var risingEdge = signal && !_lastOperatorButtonSignal;
            _lastOperatorButtonSignal = signal;

            if (risingEdge)
            {
                LogService.Log($"[Station {_stationCode}] Operator PLC button signal received");
                await HandleOperatorButtonSignalAsync();
            }

            await CheckMaterialRemovedSignalAsync();
            EnsureCurrentGoodPieceHour();
            await CheckCavityErrorSignalAsync();
            await CheckPlcCounterCompleteSignalAsync();
            await UpdateInsertedCountFromPlcAsync();
        }
        finally
        {
            _operatorButtonPollInProgress = false;
        }
    }

    private async Task CheckCavityErrorSignalAsync()
    {
        if (_config.PlcCavityErrorMemory <= 0 || _config.PlcCavityErrorRegister <= 0)
            return;


        var errorActive = await Task.Run(() => _plc.ReadCoil(_config.PlcCavityErrorMemory));
        if (!_plc.IsConnected || !errorActive)
            return;

        // Una señal activada durante Presencias pertenece a una prueba de
        // mantenimiento. Se ignora después de cerrar el formulario hasta que el
        // PLC la regrese a cero; entonces la protección normal queda rearmada.

        if (_cognexAdminLocked || _adminUnlockDialogOpen)
            return;

        var detailValue = await Task.Run(() => _plc.ReadHoldingRegister(_config.PlcCavityErrorRegister));
        var detail = detailValue is >= 1 and <= 4
            ? $"Cavidad {detailValue}"
            : "Valor no reconocido";
        var message = detailValue is >= 1 and <= 4
            ? $"Inserción incorrecta en cavidad {detailValue}."
            : $"Cavidad incorrecta. Detalle PLC D{_config.PlcCavityErrorRegister}={detailValue}.";

        _cognexAdminLocked = true;
        _adminLockCode = "PLC_CAVITY_ERROR";
        _adminLockMessage = message;
        _adminLockRequiresRejectedRemoval = false;
        _adminLockRequiresPlcQualityPulse = true;
        LogService.Log($"[Station {_stationCode}] PLC cavity error active. M{_config.PlcCavityErrorMemory}=1, D{_config.PlcCavityErrorRegister}={detailValue} ({detail})");
        SystemEventLogService.LogError("PLC_CAVITY_ERROR", $"Station {_stationCode}", message);
        if (string.IsNullOrWhiteSpace(_pendingBookingSerial))
        {
            LogService.Log($"[Station {_stationCode}] Cavity error occurred without a pending Cognex serial. Scrap booking skipped.");
        }
        else
        {
            var scrapBooking = await BookPendingSerialAsync(2);
            if (!scrapBooking)
                _adminLockMessage = $"{message} No fue posible registrar el scrap en MES.";
        }
        UpdateUI();
        ShowAdminUnlockDialog();
    }

    private async Task CheckMaterialRemovedSignalAsync()
    {
        if (_config.PlcMaterialRemovedMemory <= 0
            || _currentProcess == null
            || _stationConfig == null
            || !_currentProcess.EnabledStations.Contains(_stationCode)
            || !CanRequestHousingFromMaterialSensor())
            return;

        var boxPresent = await Task.Run(() => _plc.ReadCoil(_config.PlcMaterialRemovedMemory));
        if (!_plc.IsConnected)
            return;

        if (!boxPresent)
        {
            _lastMaterialBoxPresentSignal = false;
            BeginMaterialSensorRecheck(waitForBoxInsertion: true);
            return;
        }

        var boxInserted = _lastMaterialBoxPresentSignal == false;
        _lastMaterialBoxPresentSignal = true;

        if (!boxInserted || !_waitingForMaterialBoxInsertion)
            return;

        _waitingForMaterialBoxInsertion = false;
        LogService.Log($"[Station {_stationCode}] Housing box present signal received on M{_config.PlcMaterialRemovedMemory}. Scan required.");
        UpdateUI();
    }

    private async Task CheckPlcCounterCompleteSignalAsync()
    {
        if (_config.PlcCounterCompleteMemory <= 0
            || _materialSensorRecheckInProgress
            || _waitingForMaterialBoxInsertion
            || _cognexAdminLocked)
            return;

        var signal = await Task.Run(() => _plc.ReadCoil(_config.PlcCounterCompleteMemory));
        if (!_plc.IsConnected)
            return;

        // Solo se procesa N=N después de haber enviado el lote al PLC.
        if (!_plcCounterSent
            || _processState != StationProcessState.COGNEX_DONE
            || !signal)
            return;

        // La estación solo completa cuando PLC confirma un nuevo N=N en M44/M49/M54.
        LogService.Log($"[Station {_stationCode}] PLC counter complete signal received on M{_config.PlcCounterCompleteMemory}");
        if (!_completedBookingState.HasValue && !await BookPendingSerialAsync(0))
            return;

        if (IsSharedRecipeActive())
        {
            if (!await CompleteSharedInsertionOutputsAsync())
                return;
        }
        else if (!await ResetCableInsertionOutputsAsync())
        {
            return;
        }

        RestartStationAfterCounterComplete();
        await SetYellowButtonAsync(active: true, "N=N");
        ProcessManager.RestartStationCognexActive(_stationCode);
        CognexFlowService.StartCognexAtStation(_stationCode);
        UpdateUI();
    }

    private async Task UpdateInsertedCountFromPlcAsync()
    {
        if (_config.PlcInsertedCountRegister <= 0)
        {
            UpdateInsertedCountLabel(0);
            return;
        }

        var insertedCount = await Task.Run(() => _plc.ReadHoldingRegister(_config.PlcInsertedCountRegister));
        if (!_plc.IsConnected)
            return;

        var previousInsertedCount = _insertedCount;
        var insertedCountChanged = insertedCount != previousInsertedCount;
        UpdateInsertedCountLabel(insertedCount);

        if (insertedCountChanged && IsSharedRecipeActive())
        {
            if (insertedCount > previousInsertedCount && _plcCounterSent)
                await HandleSharedInsertionDetectedAsync(insertedCount);

            UpdateUI();
        }
    }

    private void RestartStationAfterCounterComplete()
    {
        _processState = StationProcessState.COGNEX_ACTIVE;
        _zebraStep = ZebraValidationStep.STATION;
        _scannedCables.Clear();
        _rejectedCables.Clear();
        _blockedRejectedCables.Clear();
        _cableCount = 0;
        _apiApproved = false;
        _waitingRejectedCableRemoval = false;
        _rejectedCableRemovalPending = false;
        _operatorActionInProgress = false;
        _cognexSerialReviewInProgress = false;
        _pendingBookingSerial = string.Empty;
        _completedBookingState = null;
        _bookingInProgress = false;
        _materialSensorRecheckInProgress = false;
        _waitingForMaterialBoxInsertion = false;
        _plcCounterSent = false;
        _sharedPistonTransitionInProgress = false;
        _cognexReadBlockedUntil = DateTime.MinValue;
        _lstCables.Items.Clear();
        UpdateTotalScrapCountLabel();
        UpdateInsertedCountLabel(0);
        _txtApiInfo.Clear();
        SetScrapVisualState(false);
        _txtScanInput.Clear();
        _txtScanInput.Focus();

        LogService.Log($"[Station {_stationCode}] Restarted station after PLC counter complete signal");
    }

    private void OnProcessStarted(CurrentProcess process)
    {
        var partNumberChanged = !string.Equals(
            _currentProcess?.PartNumber,
            process.PartNumber,
            StringComparison.OrdinalIgnoreCase);

        _currentProcess = process;
        UpdateMasterStatusPanel();

        _goodPiecePartNumber = process.PartNumber;

        if (partNumberChanged)
            _ = SetYellowButtonAsync(active: false, "part number change");

        if (partNumberChanged)
        {
            // La vigilancia de la caja comienza nuevamente cuando Zebra libere
            // la estacion para el nuevo numero de parte.
            _zebraReleasedForActivePartNumber = false;
            _lastMaterialBoxPresentSignal = null;
        }

        if (!process.EnabledStations.Contains(_stationCode))
        {
            _processState = StationProcessState.DISABLED;
            UpdateUI();
            return;
        }

        _processState = StationProcessState.WAITING_ZEBRA;
        _zebraStep = ZebraValidationStep.STATION;

        var recipe = RecipeRepository.FindByPartNumber(process.PartNumber);
        if (recipe?.Stations.TryGetValue(_stationCode, out var config) == true)
        {
            _stationConfig = config;
            lblBG.Text = _stationConfig.BG;
            SetGoodPieceBg(_stationConfig.BG);
        }

        _scannedCables.Clear();
        _rejectedCables.Clear();
        _blockedRejectedCables.Clear();
        _cableCount = 0;
        _apiApproved = false;
        _pendingBookingSerial = string.Empty;
        _completedBookingState = null;
        _bookingInProgress = false;
        _waitingRejectedCableRemoval = false;
        _rejectedCableRemovalPending = false;
        _plcCounterSent = false;
        _materialSensorRecheckInProgress = false;
        _waitingForMaterialBoxInsertion = false;
        _bgMismatchActive = false;
        _cognexReadBlockedUntil = DateTime.MinValue;
        _lstCables.Items.Clear();
        _txtApiInfo.Clear();
        SetScrapVisualState(false);

        UpdateUI();
    }

    private void OnStationStateChanged(string stationCode, StationProcessState newState)
    {
        if (stationCode != _stationCode)
        {
            if (_processState == StationProcessState.WAITING_ZEBRA)
                UpdateUI();

            return;
        }

        _processState = newState;
        UpdateMasterStatusPanel();
        UpdateUI();
    }

    private void OnProcessReset()
    {
        if (IsDisposed)
            return;

        if (InvokeRequired)
        {
            BeginInvoke(() =>
            {
                _ = ResetConfiguredBgPinRegistersAsync();
                Reset();
            });
            return;
        }

        _ = ResetConfiguredBgPinRegistersAsync();
        Reset();
    }

    private async Task ResetConfiguredBgPinRegistersAsync()
    {
        await SetYellowButtonAsync(active: false, "reset");

        var registers = _config.PlcFemalePinMemories
            .Concat(_config.PlcMalePinMemories)
            .Where(register => register > 0)
            .Distinct()
            .ToList();

        foreach (var register in registers)
        {
            var ok = await Task.Run(() => _plc.WriteHoldingRegister(register, 0));
            if (ok)
            {
                LogService.Log($"[Station {_stationCode}] Reset BG pin register D{register} = 0");
                continue;
            }

            LogService.Log($"[Station {_stationCode}] Failed to reset BG pin register D{register}");
        }
    }

    private async Task SetYellowButtonAsync(bool active, string reason)
    {
        if (_config.PlcBtnYellow > 0)
        {
            var yellowOk = await Task.Run(() => _plc.WriteCoil(_config.PlcBtnYellow, active));
            LogService.Log(yellowOk
                ? $"[Station {_stationCode}] Button yellow M{_config.PlcBtnYellow} = {active} ({reason})"
                : $"[Station {_stationCode}] Failed button yellow M{_config.PlcBtnYellow} = {active} ({reason})");
        }
    }

    private void OnZebraReadyForStation(string stationCode)
    {
        if (stationCode != _stationCode)
            return;

        _processState = StationProcessState.ZEBRA_IN_PROGRESS;
        _zebraStep = ZebraValidationStep.STATION;
        _txtScanInput.Focus();
        UpdateUI();
    }

    private void OnZebraCompletedAtStation(string stationCode)
    {
        if (stationCode != _stationCode)
            return;
    }

    private void SubmitScan()
    {
        var barcode = _txtScanInput.Text.Trim();
        _txtScanInput.Clear();
        _txtScanInput.Focus();

        if (string.IsNullOrEmpty(barcode))
            return;

        if (_currentProcess == null || _stationConfig == null)
        {
            ShowError("Proceso no iniciado");
            return;
        }

        if (_processState == StationProcessState.DISABLED)
        {
            ShowError("Estación deshabilitada");
            return;
        }

        if (_processState == StationProcessState.WAITING_ZEBRA)
        {
            ShowError(GetWaitingReleaseInstruction());
            return;
        }

        if (_processState == StationProcessState.ZEBRA_IN_PROGRESS)
        {
            if (_materialSensorRecheckInProgress)
            {
                ProcessMaterialSensorRecheckStep(barcode);
                return;
            }

            ProcessZebraValidationStep(barcode);
            return;
        }

        if (_waitingRejectedCableRemoval)
        {
            ProcessRejectedCableRemoval(barcode);
            return;
        }

        if (_processState == StationProcessState.COGNEX_ACTIVE)
        {
            _ = ProcessCognexDataAsync(barcode);
            return;
        }

        ShowError($"Estado no válido para escaneo: {_processState}");
    }

    private void ProcessZebraValidationStep(string barcode)
    {
        bool isMaterialStation = IsMaterialStation();

        switch (_zebraStep)
        {
            case ZebraValidationStep.STATION:
                if (!StationValidationService.ValidateStationCode(barcode, _stationConfig!.Code))
                {
                    ShowError($"Estación incorrecta. Esperado: {_stationConfig!.Code}");
                    return;
                }
                ShowSuccess("✓ Estación correcta");
                MoveToNextZebraStepAfterStation();
                break;

            case ZebraValidationStep.BG:
                if (!StationValidationService.ValidateBG(barcode, _stationConfig!))
                {
                    // Usa la misma salida roja/memoria de NOK que un rechazo de API.
                    // Se mantiene activa hasta que el operador escanee el BG correcto.
                    SetBgMismatchState(true);
                    ShowError($"BG incorrecto. Esperado: {_stationConfig!.BG}");
                    return;
                }
                SetBgMismatchState(false);
                MoveToNextZebraStepAfterBg(isMaterialStation);
                break;

            case ZebraValidationStep.TOOLS:
                var expectedTool = GetCurrentToolForDisplay();
                if (expectedTool is null)
                {
                    MoveToNextZebraStepAfterTools();
                    break;
                }

                var toolResult = StationValidationService.ValidateTool(
                    barcode,
                    expectedTool);

                if (!toolResult.IsValid)
                {
                    ShowError(toolResult.Message);
                    return;
                }
                _toolValidatedPartNumber = _currentProcess?.PartNumber ?? string.Empty;
                MoveToNextZebraStepAfterTools();
                break;

            case ZebraValidationStep.HOUSING:
                if (!TryClearMaterialBoxInsertionWait())
                {
                    ShowError(GetMaterialBoxInstruction());
                    return;
                }

                if (!ValidateHousingScan(barcode))
                {
                    ShowError($"Housing incorrecto. Esperado: {GetExpectedHousingBarcode()}");
                    return;
                }
                MoveToNextZebraStepAfterHousing();
                break;

            case ZebraValidationStep.COMPLETE:
                CompleteZebraValidation();
                break;
        }
    }

    public void SubmitExternalScan(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return;

        _txtScanInput.Text = barcode.Trim();
        SubmitScan();
    }

    private void BeginMaterialSensorRecheck(bool waitForBoxInsertion)
    {
        if (!CanRequestHousingFromMaterialSensor())
        {
            LogService.Log($"[Station {_stationCode}] Housing sensor signal ignored because recipe has no housing validation");
            return;
        }

        if (!_materialSensorRecheckInProgress)
        {
            _processStateBeforeMaterialSensorRecheck = _processState;
            _zebraStepBeforeMaterialSensorRecheck = _zebraStep;
        }

        _materialSensorRecheckInProgress = true;
        _waitingForMaterialBoxInsertion = waitForBoxInsertion;
        _processState = StationProcessState.ZEBRA_IN_PROGRESS;
        _zebraStep = ZebraValidationStep.HOUSING;

        LogService.Log($"[Station {_stationCode}] Housing box missing on M{_config.PlcMaterialRemovedMemory}. Insert box and re-scan required.");
        UpdateUI();
    }

    private void ProcessMaterialSensorRecheckStep(string barcode)
    {
        if (!TryClearMaterialBoxInsertionWait())
        {
            ShowError(GetMaterialBoxInstruction());
            return;
        }

        switch (_zebraStep)
        {
            case ZebraValidationStep.HOUSING:
                if (!ValidateHousingScan(barcode))
                {
                    ShowError($"Housing incorrecto. Esperado: {GetExpectedHousingBarcode()}");
                    return;
                }

                CompleteMaterialSensorRecheck();
                return;
        }
    }

    private void CompleteMaterialSensorRecheck()
    {
        _materialSensorRecheckInProgress = false;
        _waitingForMaterialBoxInsertion = false;
        _processState = _processStateBeforeMaterialSensorRecheck;
        _zebraStep = _zebraStepBeforeMaterialSensorRecheck;

        // Si la caja se retiró durante Cognex, se reactiva explícitamente el
        // flujo compartido además de restaurar el estado local del panel.
        if (_processState == StationProcessState.COGNEX_ACTIVE)
        {
            ProcessManager.RestartStationCognexActive(_stationCode);
            CognexFlowService.StartCognexAtStation(_stationCode);
        }

        LogService.Log(
            $"[Station {_stationCode}] Housing re-scan completed after sensor signal. " +
            $"RestoredState={_processState}, CognexActive={CognexFlowService.IsCognexActiveAtStation(_stationCode)}, " +
            $"AdminLocked={_cognexAdminLocked}, ApiReviewInProgress={_cognexSerialReviewInProgress}");
        ShowSuccess("Housing validado.");
        UpdateUI();
    }

    private string GetMaterialBoxInstruction()
    {
        var materialCode = GetExpectedHousingBarcode();
        return string.IsNullOrWhiteSpace(materialCode)
            ? "Insertar caja del Housing."
            : $"Insertar caja del Housing: {materialCode}";
    }

    private void MoveToNextZebraStepAfterStation()
    {
        if (IsHousingEnabled())
        {
            _zebraStep = ZebraValidationStep.HOUSING;
            _waitingForMaterialBoxInsertion = IsMaterialBoxMissing();
            UpdateUI();
            return;
        }

        _zebraStep = ZebraValidationStep.BG;
        UpdateUI();
    }

    private void MoveToNextZebraStepAfterHousing()
    {
        _waitingForMaterialBoxInsertion = false;
        _zebraStep = ZebraValidationStep.BG;
        UpdateUI();
    }

    private void MoveToNextZebraStepAfterBg(bool isMaterialStation)
    {
        MoveToNextZebraStepAfterMaterials();
    }

    private void MoveToNextZebraStepAfterMaterials()
    {
        if (ShouldScanToolForCurrentVariant())
        {
            _zebraStep = ZebraValidationStep.TOOLS;
            UpdateUI();
            return;
        }

        MoveToNextZebraStepAfterTools();
    }

    private void MoveToNextZebraStepAfterTools()
    {
        CompleteZebraValidation();
    }

    private bool IsHousingEnabled()
    {
        return (IsSharedRecipeActive() || IsFirstEnabledStation())
            && !string.IsNullOrWhiteSpace(GetExpectedHousingBarcode());
    }

    private bool HasHousingForActiveProcess()
    {
        return !string.IsNullOrWhiteSpace(GetExpectedHousingBarcode());
    }

    private bool CanRequestHousingFromMaterialSensor()
    {
        if (_currentProcess == null || !IsHousingEnabled())
            return false;

        // Durante la liberacion Zebra inicial tambien se debe vigilar la caja.
        // Asi, al insertarla mientras el paso HOUSING esta esperando, la pantalla
        // cambia inmediatamente de "Insertar caja" a "Escanear Housing".
        var waitingInitialHousingScan = _processState == StationProcessState.ZEBRA_IN_PROGRESS
            && _zebraStep == ZebraValidationStep.HOUSING;

        // Despues de liberar Zebra, la memoria continua vigilada durante Cognex,
        // la espera de N=N y los batches posteriores.
        return waitingInitialHousingScan || _zebraReleasedForActivePartNumber;
    }

    private bool IsMaterialBoxMissing()
    {
        if (_config.PlcMaterialRemovedMemory <= 0 || !_plc.IsConnected)
            return false;

        var boxPresent = _plc.ReadCoil(_config.PlcMaterialRemovedMemory);
        if (!_plc.IsConnected)
            return false;

        _lastMaterialBoxPresentSignal = boxPresent;
        return !boxPresent;
    }

    private bool TryClearMaterialBoxInsertionWait()
    {
        if (!_waitingForMaterialBoxInsertion)
            return true;

        if (IsMaterialBoxMissing())
            return false;

        _waitingForMaterialBoxInsertion = false;
        return true;
    }

    private string GetExpectedHousingBarcode()
    {
        var configuredHousing = _stationConfig?.Housing?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(configuredHousing))
            return configuredHousing;

        if (_currentProcess == null)
            return string.Empty;

        var firstEnabledStation = _currentProcess.EnabledStations.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(firstEnabledStation))
            return string.Empty;

        var recipe = RecipeRepository.FindByPartNumber(_currentProcess.PartNumber);
        if (recipe?.Stations.TryGetValue(firstEnabledStation, out var firstStationConfig) == true)
        {
            var recipeHousing = firstStationConfig.Housing?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(recipeHousing))
                return recipeHousing;
        }

        return string.Empty;
    }

    private bool ValidateHousingScan(string barcode)
    {
        var expectedHousing = GetExpectedHousingBarcode();
        if (string.IsNullOrWhiteSpace(expectedHousing))
        {
            LogService.Log($"[Station {_stationCode}] Housing validation skipped because recipe Housing is empty");
            return true;
        }

        var scanned = NormalizeHousingCode(barcode);
        var expected = NormalizeHousingCode(expectedHousing);
        var isValid = !string.IsNullOrWhiteSpace(expected)
            && scanned.Contains(expected, StringComparison.OrdinalIgnoreCase);

        LogService.Log(
            $"[Station {_stationCode}] Housing validation: " +
            $"valid={isValid}, rawLength={barcode.Length}, expected='{expected}', " +
            $"normalizedScan='{scanned}'");
        return isValid;
    }

    private static string NormalizeHousingCode(string value)
    {
        return new string(value
            .Where(char.IsLetterOrDigit)
            .ToArray());
    }

    private bool IsFirstEnabledStation()
    {
        return string.Equals(
            _currentProcess?.EnabledStations.FirstOrDefault(),
            _stationCode,
            StringComparison.OrdinalIgnoreCase);
    }

    private string GetWaitingReleaseInstruction()
    {
        if (_currentProcess == null)
            return "En espera de numero de parte.";

        var stationIndex = _currentProcess.EnabledStations.FindIndex(station =>
            string.Equals(station, _stationCode, StringComparison.OrdinalIgnoreCase));

        if (stationIndex < 0)
            return "Estacion deshabilitada";

        var stationWaitingForRelease = ZebraFlowService.GetStationWaitingForZebra();
        if (string.IsNullOrWhiteSpace(stationWaitingForRelease))
            stationWaitingForRelease = ProcessManager.GetNextStationWaitingZebra();

        return string.IsNullOrWhiteSpace(stationWaitingForRelease)
            ? "Esperando liberacion."
            : $"Esperando liberacion de la estacion {stationWaitingForRelease}.";
    }

    private bool IsMaterialStation()
    {
        return IsSharedRecipeActive() || IsFirstEnabledStation();
    }

    private async void CompleteZebraValidation()
    {
        var connectorCode = GetConnectorPlcCode();
        _zebraStep = ZebraValidationStep.COMPLETE;
        ShowInstruction("Zebra completo. Enviando estacion/lado al PLC...", Color.DarkOrange);

        // Al terminar el ultimo escaneo Zebra, el PLC debe recibir:
        // D2 = estacion activa, D0 = lado como string "A" o "B".
        // Cambio solicitado: si esta escritura falla, no se bloquea Cognex ni la siguiente estacion.
        var plcStationSideOk = await WriteActiveStationAndConnectorSideAsync(connectorCode);
        if (!plcStationSideOk)
            LogService.Log($"[Station {_stationCode}] Zebra flow continues even though PLC station/side write failed");

        ZebraFlowService.CompleteZebraAtCurrentStation(_stationCode);
        CognexFlowService.StartCognexAtStation(_stationCode);
        _zebraReleasedForActivePartNumber = true;
        _processState = StationProcessState.COGNEX_ACTIVE;
        _zebraStep = ZebraValidationStep.STATION;
        UpdateUI();
    }

    private void OnCognexDataReceived(int stationNum, string data)
    {
        if (stationNum != _config.StationNumber)
            return;

        LogService.Log($"[Station {_stationCode}] Cognex data received: {data}");

        BeginInvoke(() =>
        {
            _ = ProcessCognexDataAsync(data);
        });
    }

    private async Task ProcessCognexDataAsync(string data)
    {
        if (_cognexAdminLocked)
        {
            LogService.Log($"[Station {_stationCode}] Cognex data ignored while admin lock is active: {data}");
            return;
        }

        if (_processState != StationProcessState.COGNEX_ACTIVE)
        {
            LogService.Log($"[Station {_stationCode}] Cognex data ignored because state is {_processState}");
            return;
        }

        if (_waitingRejectedCableRemoval)
        {
            ProcessRejectedCableRemoval(data);
            return;
        }

        if (_rejectedCableRemovalPending)
        {
            LogService.Log($"[Station {_stationCode}] Cognex data ignored until operator button confirms rejected cable removal: {data}");
            ShowError("Presiona el boton para retirar el serial rechazado.");
            return;
        }

        if (DateTime.Now < _cognexReadBlockedUntil)
        {
            LogService.Log($"[Station {_stationCode}] Cognex data ignored during NOK cooldown until {_cognexReadBlockedUntil:HH:mm:ss}: {data}");
            ShowError("Espera 5 segundos antes de escanear otra pieza.");
            return;
        }

        if (_cognexSerialReviewInProgress)
        {
            LogService.Log($"[Station {_stationCode}] Cognex data ignored while API is reviewing another serial: {data}");
            ShowError("Espera respuesta de API antes de escanear otra pieza.");
            return;
        }

        if (IsSharedRecipeActive() && _plcCounterSent && _insertedCount < _scannedCables.Count)
        {
            var expectedCavities = GetSharedRecipeExpectedCavityCount();
            var cavityNumber = _scannedCables.Count;
            ShowInstruction($"Ensamblar pieza al conector {cavityNumber}/{expectedCavities}.", Color.DarkOrange);
            LogService.Log($"[Station {_stationCode}] Cognex data ignored until cavity {cavityNumber}/{expectedCavities} is assembled. Inserted={_insertedCount}");
            return;
        }

        // Cambio solicitado: Cognex solo debe aceptar seriales completos de 24 caracteres.
        if (data.Length != CognexSerialLength)
        {
            LogService.Log($"[Station {_stationCode}] Cognex serial ignored because length is {data.Length}, expected {CognexSerialLength}: {data}");
            ShowError($"Serial Cognex incompleto: {data.Length}/{CognexSerialLength} caracteres.");
            return;
        }

        if (!ValidateCognexBg(data))
            return;

        if (_blockedRejectedCables.Contains(data))
        {
            LogService.Log($"[Station {_stationCode}] Previously rejected Cognex serial ignored for this cycle: {data}");
            ShowError($"Serial rechazado ya retirado: {data}");
            return;
        }

        if (_scannedCables.Contains(data, StringComparer.OrdinalIgnoreCase))
        {
            LogService.Log($"[Station {_stationCode}] Duplicate Cognex serial ignored: {data}");
            return;
        }

        if (HasSharedRecipeReachedExpectedCavities())
        {
            var expectedCavities = GetSharedRecipeExpectedCavityCount();
            ShowError($"Ya se completaron las {expectedCavities} cavidades esperadas.");
            LogService.Log($"[Station {_stationCode}] Extra Cognex serial ignored because shared recipe already reached {expectedCavities} cavities: {data}");
            return;
        }

        _scannedCables.Add(data);
        _pendingBookingSerial = data;
        _completedBookingState = null;
        _totalScannedCount++;
        _lstCables.Items.Add(CreateCableListItem(data));
        RefreshCableCount();
        _apiApproved = false;
        _waitingRejectedCableRemoval = false;
        _rejectedCableRemovalPending = false;
        _rejectedCables.Clear();

        LogService.Log($"[Station {_stationCode}] Cognex cable received. Reviewing serial individually: {data}");
        await ReviewSingleCognexSerialAsync(data);
    }

    private bool ValidateCognexBg(string serial)
    {
        var expectedBg = _stationConfig?.BG.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(expectedBg))
        {
            SetBgMismatchState(false);
            LogService.Log($"[Station {_stationCode}] Cognex BG validation skipped because recipe BG is empty");
            return true;
        }

        if (serial.EndsWith(expectedBg, StringComparison.OrdinalIgnoreCase))
        {
            SetBgMismatchState(false);
            return true;
        }

        SetBgMismatchState(true);
        LogService.Log($"[Station {_stationCode}] Cognex BG mismatch. Expected serial ending BG={expectedBg}, serial={serial}");
        LockForCognexAdmin(
            "COGNEX_BG_MISMATCH",
            $"BG Cognex incorrecto. El serial debe terminar en: {expectedBg}",
            requiresRejectedRemoval: false);
        return false;
    }

    private async Task ReviewSingleCognexSerialAsync(string serial)
    {
        var request = new StationInfoApiRequest
        {
            Station = GetApiStationNumber(),
            PartNumber = GetRecipePartNumber(),
            SerialNumbers = [serial]
        };

        var requestJson = JsonSerializer.Serialize(request, _apiInfoJsonOptions);
        ShowApiInfo(string.Empty);
        _cognexSerialReviewInProgress = true;
        // Mientras MES revisa la lectura de Cognex, bloquea la accion del boton
        // para evitar que una pulsacion coincida con la liberacion automatica.
        _operatorActionInProgress = true;

        try
        {
            var result = await StationInfoApiService.ReviewSerialsIndividuallyAsync(request);
            ShowApiResult(result);
            if (!result.Success)
            {
                LogService.Log($"[Station {_stationCode}] API single serial ERROR serial={serial} " +
                    $"resultCode={result.ResultCode?.ToString() ?? "N/A"} " +
                    $"resultText={result.ResultText}: {result.ResponseBody}");
            }
            LogMesPieceReview(serial, result);

            if (result.Success)
            {
                await HandleApprovedCognexSerialAsync(serial);
                return;
            }

            _apiApproved = false;
            var rejectedSerials = GetConfirmedMesRejectedSerials(result);
            if (rejectedSerials.Count == 0)
            {
                ShowError($"No se pudo confirmar la pieza con MES. Se requiere autorización para liberar. {result.Message}");
                LogService.Log($"[Station {_stationCode}] MES technical/unconfirmed failure. Scrap not incremented. Serial={serial}, Message={result.Message}");
                LockForCognexAdmin(
                    "MES_TECHNICAL_ERROR",
                    string.IsNullOrWhiteSpace(result.Message) ? "No fue posible confirmar la pieza con MES." : result.Message,
                    requiresRejectedRemoval: false);
                return;
            }

            BeginRejectedCableRemoval(rejectedSerials);
            ShowError(result.Message);
            LockForCognexAdmin(
                "API_REJECTED",
                string.IsNullOrWhiteSpace(result.Message) ? "La API rechazo el serial." : result.Message,
                requiresRejectedRemoval: true);
        }
        finally
        {
            _cognexSerialReviewInProgress = false;
            _operatorActionInProgress = false;
            UpdateUI();
        }
    }

    private async Task HandleApprovedCognexSerialAsync(string serial)
    {
        if (!IsSharedRecipeActive())
        {
            _apiApproved = true;
            ShowSuccess($"CHECK OK: {serial}. Liberando PLC...");
            LogService.Log($"[Station {_stationCode}] MES approved Cognex serial. Starting automatic PLC release without operator button. Serial={serial}");
            await FinalizeStationCycleAsync();
            return;
        }

        await HandleSharedCognexApprovalAsync(serial);
    }

    private async Task HandleSharedCognexApprovalAsync(string approvedSerial)
    {
        var expectedCavities = GetSharedRecipeExpectedCavityCount();
        if (expectedCavities <= 0)
        {
            _apiApproved = false;
            ShowError("La receta compartida no tiene cavidades configuradas.");
            LogService.Log($"[Station {_stationCode}] Shared recipe has no valid cavity count");
            return;
        }

        var approvedCavities = _scannedCables.Count;
        LogService.Log(
            $"[Station {_stationCode}] Shared Cognex approval: " +
            $"approved={approvedCavities}, expected={expectedCavities}, serial={approvedSerial}");

        if (approvedCavities > expectedCavities)
        {
            _apiApproved = false;
            ShowError($"Se recibieron mas cables que las {expectedCavities} cavidades configuradas.");
            return;
        }

        if (!await ReleaseSharedInsertionStepAsync(expectedCavities, approvedCavities))
            return;

        if (approvedCavities < expectedCavities)
        {
            _apiApproved = false;
            ShowInstruction($"Ensamblar cable {approvedCavities}/{expectedCavities}.", Color.DarkOrange);
            UpdateUI();
            return;
        }

        _apiApproved = true;
        ShowSuccess($"Cavidades completas {approvedCavities}/{expectedCavities}. Liberando estacion...");
        await FinalizeStationCycleAsync();
    }

    private bool IsSharedRecipeActive()
    {
        return GetActiveRecipe()?.SharedMode == true;
    }

    private Recipe? GetActiveRecipe()
    {
        return _currentProcess is null
            ? null
            : RecipeRepository.FindByPartNumber(_currentProcess.PartNumber);
    }

    private int GetSharedRecipeExpectedCavityCount()
    {
        if (!IsSharedRecipeActive() || _stationConfig?.BgPins is not { Count: >= 4 } pins)
            return 0;

        return pins.Take(4).Count(value => value == 1);
    }

    private bool HasSharedRecipeReachedExpectedCavities()
    {
        var expectedCavities = GetSharedRecipeExpectedCavityCount();
        return expectedCavities > 0 && _scannedCables.Count >= expectedCavities;
    }

    private bool CanFinalizeSharedRecipe()
    {
        if (!IsSharedRecipeActive())
            return true;

        var expectedCavities = GetSharedRecipeExpectedCavityCount();
        if (expectedCavities > 0 && _scannedCables.Count == expectedCavities)
            return true;

        ShowError($"Faltan cavidades Cognex: {_scannedCables.Count}/{expectedCavities}.");
        LogService.Log($"[Station {_stationCode}] Shared recipe release blocked: scanned={_scannedCables.Count}, expected={expectedCavities}");
        return false;
    }

    private string GetCognexActiveInstruction()
    {
        if (!IsSharedRecipeActive())
            return $"Escanear piezas. Activo - {_cableCount} cables.";

        var expectedCavities = GetSharedRecipeExpectedCavityCount();
        if (_plcCounterSent && _insertedCount < _scannedCables.Count)
            return $"Ensamblar pieza al conector {_scannedCables.Count}/{expectedCavities}.";

        var nextCavity = Math.Min(_scannedCables.Count + 1, expectedCavities);
        return $"Escanear pieza {nextCavity}/{expectedCavities}.";
    }

    private async Task<bool> ReleaseSharedRecipeToPLCAsync(int expectedCavities)
    {
        ShowInstruction("Preparando PLC para ensamble compartido...", Color.DarkOrange);

        var connectorCode = GetConnectorPlcCode();
        if (!await WriteActiveStationAndConnectorSideAsync(connectorCode))
            return false;

        var registerOk = await Task.Run(() =>
            _plc.WriteHoldingRegister(_config.PlcCounter, expectedCavities));
        if (!registerOk)
        {
            ShowError("PLC no conectado. No se pudo escribir el total de cavidades.");
            return false;
        }

        if (!await WritePlcMemoryAsync(_config.PlcQuantityProcessMemory, true, "proceso compartido con cantidad"))
            return false;

        if (!await WritePlcMemoryAsync(_config.PlcFinalProcessMemory, false, "abrir piston para ensamble compartido"))
            return false;

        _plcCounterSent = true;
        LogService.Log($"[Station {_stationCode}] Shared PLC insertion released for {expectedCavities} cavities");
        return true;
    }

    private async Task<bool> ReleaseSharedInsertionStepAsync(int expectedCavities, int cableNumber)
    {
        if (!_plcCounterSent)
            return await ReleaseSharedRecipeToPLCAsync(expectedCavities);

        if (!await WritePlcMemoryAsync(
                _config.PlcFinalProcessMemory,
                false,
                $"abrir piston para cable compartido {cableNumber}/{expectedCavities}"))
        {
            return false;
        }

        LogService.Log(
            $"[Station {_stationCode}] Shared insertion step released. " +
            $"Cable={cableNumber}/{expectedCavities}, Inserted={_insertedCount}");
        return true;
    }

    private async Task HandleSharedInsertionDetectedAsync(int insertedCount)
    {
        if (_sharedPistonTransitionInProgress)
            return;

        var expectedCavities = GetSharedRecipeExpectedCavityCount();
        if (expectedCavities <= 0 || insertedCount > expectedCavities)
            return;

        _sharedPistonTransitionInProgress = true;
        try
        {
            if (insertedCount < expectedCavities)
            {
                if (!await WritePlcMemoryAsync(
                        _config.PlcFinalProcessMemory,
                        true,
                        $"cerrar piston despues de cable compartido {insertedCount}/{expectedCavities}"))
                {
                    return;
                }

                ShowInstruction($"Escanear pieza {insertedCount + 1}/{expectedCavities}.", Color.Green);
                LogService.Log(
                    $"[Station {_stationCode}] Shared insertion detected and piston closed. " +
                    $"Inserted={insertedCount}/{expectedCavities}");
                return;
            }

            // La ultima insercion termina con el piston abierto para retirar la pieza.
            await WritePlcMemoryAsync(
                _config.PlcFinalProcessMemory,
                false,
                $"mantener piston abierto al completar compartido {insertedCount}/{expectedCavities}");
            ShowInstruction($"Proceso completo {insertedCount}/{expectedCavities}. Retirar pieza.", Color.Green);
        }
        finally
        {
            _sharedPistonTransitionInProgress = false;
        }
    }

    private async Task<bool> CompleteSharedInsertionOutputsAsync()
    {
        if (!await WritePlcMemoryAsync(
                _config.PlcQuantityProcessMemory,
                false,
                "finalizar proceso compartido con cantidad"))
        {
            return false;
        }

        if (!await WritePlcMemoryAsync(
                _config.PlcFinalProcessMemory,
                false,
                "dejar piston abierto para retirar pieza compartida"))
        {
            return false;
        }

        LogService.Log($"[Station {_stationCode}] Shared process completed with piston open");
        return true;
    }

    private void LogMesPieceReview(string serial, StationInfoApiResult result)
    {
        var serialResult = result.SerialResults.FirstOrDefault(r =>
            string.Equals(r.SerialNumber, serial, StringComparison.OrdinalIgnoreCase));
        var isConfirmedScrap = GetConfirmedMesRejectedSerials(result)
            .Contains(serial, StringComparer.OrdinalIgnoreCase);
        var classification = result.Success
            ? "OK"
            : isConfirmedScrap
                ? "SCRAP"
                : "API_ERROR";

        // Cambio solicitado: guardar trazabilidad MES de cada pieza revisada por Cognex/API.
        MesPieceLogService.LogPieceReview(new MesPieceLogEntry
        {
            Station = GetApiStationNumber(),
            PartNumber = GetRecipePartNumber(),
            SerialNumber = serial,
            CableCount = _cableCount,
            Success = result.Success,
            IsScrap = isConfirmedScrap,
            Classification = classification,
            StatusCode = serialResult?.StatusCode ?? result.StatusCode,
            ResultCode = serialResult?.ResultCode ?? result.ResultCode,
            ResultText = serialResult?.ResultText ?? result.ResultText,
            Message = serialResult?.Message ?? result.Message,
            ResponseBody = serialResult?.ResponseBody ?? result.ResponseBody
        });
    }

    private async Task<bool> BookPendingSerialAsync(int state)
    {
        if (_completedBookingState.HasValue)
            return _completedBookingState.Value == state;

        if (_bookingInProgress || string.IsNullOrWhiteSpace(_pendingBookingSerial))
            return false;

        var serial = _pendingBookingSerial;
        _bookingInProgress = true;
        try
        {
            var result = await StationInfoApiService.BookSingleSerialAsync(
                GetApiStationNumber(),
                GetRecipePartNumber(),
                serial,
                state);

            ShowApiResult(result);
            if (!result.Success)
            {
                LogService.Log(
                    $"[Station {_stationCode}] BOOK ERROR serial={serial}, state={state}, " +
                    $"resultCode={result.ResultCode?.ToString() ?? "N/A"}, " +
                    $"resultText={result.ResultText}: {result.ResponseBody}");
            }

            if (!result.Success)
            {
                ShowError($"No se pudo registrar el booking del serial {serial}. {result.Message}");
                if (state == 0 && !_cognexAdminLocked)
                {
                    _cognexAdminLocked = true;
                    _adminLockCode = "BOOKING_ERROR";
                    _adminLockMessage = $"No se pudo registrar el serial {serial} en MES. Autoriza para liberar manualmente.";
                    _adminLockRequiresRejectedRemoval = false;
                    _adminLockRequiresPlcQualityPulse = false;
                    UpdateUI();
                    ShowAdminUnlockDialog();
                }
                return false;
            }

            _completedBookingState = state;
            if (state == 0)
            {
                CountGoodPieces([serial]);
                MarkCableAsOk(serial);
                ShowSuccess($"BOOK OK: {serial}.");
            }
            else
            {
                _totalScrapCount++;
                UpdateTotalScrapCountLabel();
                ShowError($"SCRAP registrado en MES: {serial}.");
            }

            return true;
        }
        finally
        {
            _bookingInProgress = false;
            UpdateUI();
        }
    }

    private async Task FinalizeStationCycleAsync()
    {
        if (!CanFinalizeSharedRecipe())
            return;

        if (_processState != StationProcessState.COGNEX_ACTIVE && _processState != StationProcessState.COGNEX_DONE)
        {
            ShowError("Debe completar Cognex primero");
            return;
        }

        if (!_apiApproved)
        {
            ShowError("Debe aprobar API antes de enviar al PLC");
            return;
        }

        CognexFlowService.CompleteCognexAtStation(_stationCode);
        _processState = StationProcessState.COGNEX_DONE;
        UpdateUI();

        await ReleaseToPLCAsync();
    }

    public async Task HandleOperatorButtonSignalAsync()
    {
        // Punto central del boton: aqui entran tanto el boton fisico del PLC
        // como los botones de pantalla Review API y Finalize.
        LogService.Log(
            $"[Station {_stationCode}] Operator action state: " +
            $"ProcessState={_processState}, ApiApproved={_apiApproved}, CableCount={_cableCount}, " +
            $"PlcCounterSent={_plcCounterSent}, AdminLocked={_cognexAdminLocked}, " +
            $"WaitingRejectedRemoval={_waitingRejectedCableRemoval}, ActionInProgress={_operatorActionInProgress}");

        if (_cognexAdminLocked)
        {
            ShowAdminUnlockDialog();
            return;
        }

        if (_operatorActionInProgress)
            return;

        if (_plcCounterSent)
        {
            ShowInstruction(GetCableInsertionInstruction(), Color.DarkOrange);
            return;
        }

        // Si ya se pidio retirar un serial rechazado, el boton no finaliza:
        // solo vuelve a mostrar la instruccion para que se reescanee ese serial.
        if (_waitingRejectedCableRemoval)
        {
            ShowInstruction(GetRejectedCableInstruction(), Color.Red);
            _txtScanInput.Focus();
            return;
        }

        // Cambio solicitado: el boton ya no retira scrap; el retiro se hace reescaneando el serial rechazado.
        if (_rejectedCableRemovalPending)
        {
            ShowInstruction(GetRejectedCableInstruction(), Color.Red);
            _txtScanInput.Focus();
            return;
        }

        // Si aun no hay aprobacion de API, el boton primero revisa los seriales.
        // Si API responde OK, ReviewApiAndFinalizeAsync continua con el envio al PLC.
        if (_processState == StationProcessState.COGNEX_ACTIVE && !_apiApproved)
        {
            await ReviewApiAndFinalizeAsync();
            return;
        }

        // Si la API ya esta aprobada, el boton manda contador/memorias al PLC.
        await FinalizeStationCycleAsync();
    }

    private async Task ReleaseToPLCAsync()
    {
        // En modo compartido el PLC se libera desde la primera pieza aprobada para
        // permitir la secuencia escanear -> ensamblar en cada cavidad.
        if (IsSharedRecipeActive() && _plcCounterSent)
        {
            ShowInstruction(GetCableInsertionInstruction(), Color.DarkOrange);
            UpdateUI();
            return;
        }

        ShowInstruction("Enviando datos al PLC...", Color.DarkOrange);

        var connectorCode = GetConnectorPlcCode();
        LogService.Log($"[Station {_stationCode}] Connector code from recipe: {connectorCode}");

        if (!await WriteActiveStationAndConnectorSideAsync(connectorCode))
            return;

        // Primero se manda al PLC el total de cables aprobados para esta estacion.
        // Este valor se escribe en plcCounter.
        bool registerOk = await Task.Run(() => _plc.WriteHoldingRegister(_config.PlcCounter, _cableCount));
        if (!registerOk)
        {
            ShowError("PLC no conectado. No se pudo escribir el contador.");
            return;
        }

        // M61/M62/M63 y M55/M56/M57 son comandos de liberación para el PLC.
        if (!await WritePlcMemoryAsync(_config.PlcQuantityProcessMemory, true, "proceso con cantidad"))
            return;

        // PlcFinalProcessMemory usa polaridad inversa: OFF libera el pistón.
        if (!await WritePlcMemoryAsync(_config.PlcFinalProcessMemory, false, "abrir pistón para insertar"))
            return;

        _plcCounterSent = true;
        ShowInstruction(GetCableInsertionInstruction(), Color.DarkOrange);
        UpdateUI();
    }

    private async Task<bool> WritePlcMemoryAsync(int memoryAddress, bool value, string description)
    {
        if (memoryAddress <= 0)
            return true;

        var ok = await Task.Run(() => _plc.WriteCoil(memoryAddress, value));
        if (!ok)
        {
            ShowError($"PLC no conectado. No se pudo escribir memoria de {description}.");
            return false;
        }

        LogService.Log($"[Station {_stationCode}] PLC memory M{memoryAddress} = {value} ({description})");
        return true;
    }

    private string GetConnectorPlcCode()
    {
        var connectorGender = (_currentProcess?.ConnectorGender ?? string.Empty).Trim();
        return string.Equals(connectorGender, "B", StringComparison.OrdinalIgnoreCase)
            || string.Equals(connectorGender, "Male", StringComparison.OrdinalIgnoreCase)
            || string.Equals(connectorGender, _config.PlcMale, StringComparison.OrdinalIgnoreCase)
                ? "B"
                : "A";
    }

    private async Task<bool> WriteActiveStationAndConnectorSideAsync(string connectorCode)
    {
        if (_config.PlcActiveStationRegister > 0)
        {
            const int plcStationValue = 1;
            bool activeStationOk = await Task.Run(() =>
                _plc.WriteHoldingRegister(_config.PlcActiveStationRegister, plcStationValue));

            if (!activeStationOk)
            {
                ShowError("PLC no conectado. No se pudo escribir la estacion activa.");
                return false;
            }

            LogService.Log($"[Station {_stationCode}] Active station PLC value {plcStationValue} written to register D{_config.PlcActiveStationRegister}");
        }

        if (_config.PlcConnectorSideRegister >= 0)
        {
            // D0 esta configurado como string en el PLC: aqui se envia "A" o "B" como texto, no como ASCII 65/66.
            bool connectorSideOk = await Task.Run(() =>
                _plc.WriteHoldingRegisterString(_config.PlcConnectorSideRegister, connectorCode));

            if (!connectorSideOk)
            {
                ShowError("PLC no conectado. No se pudo escribir el lado del conector.");
                return false;
            }

            LogService.Log($"[Station {_stationCode}] Connector side {connectorCode} written to string register D{_config.PlcConnectorSideRegister}");
        }

        if (!await WriteBgPinsWithStationAndSideAsync(connectorCode))
            return false;

        return true;
    }

    private async Task<bool> WriteBgPinsWithStationAndSideAsync(string connectorCode)
    {
        // Da tiempo al PLC para procesar D2/D0 antes de escribir las M de pines.
        // La prueba manual desde PLC Debug funciona porque naturalmente existe
        // esta separacion entre ambas operaciones.
        await Task.Delay(500);

        var pinValues = _stationConfig?.BgPins is { Count: >= 4 }
            ? _stationConfig.BgPins
            : [0, 0, 0, 0];
        var useMalePins = string.Equals(connectorCode, "B", StringComparison.OrdinalIgnoreCase);
        var selectedSide = useMalePins ? "Male/B" : "Female/A";
        var selectedMemories = useMalePins
            ? _config.PlcMalePinMemories
            : _config.PlcFemalePinMemories;

        LogService.Log(
            $"[Station {_stationCode}] Sending D2/D0 BG pin payload: Connector={selectedSide}, " +
            $"Values=[{string.Join(",", pinValues.Take(4))}], " +
            $"Registers=[{string.Join(",", selectedMemories.Take(4).Select(memory => memory > 0 ? $"D{memory}" : "disabled"))}]");

        for (var pinIndex = 0; pinIndex < 4; pinIndex++)
        {
            var recipeValue = pinValues[pinIndex] == 1;
            if (!await WriteStationBgPinMemoryAsync("Female/A", pinIndex + 1, _config.PlcFemalePinMemories[pinIndex], !useMalePins && recipeValue))
                return false;
            if (!await WriteStationBgPinMemoryAsync("Male/B", pinIndex + 1, _config.PlcMalePinMemories[pinIndex], useMalePins && recipeValue))
                return false;

            await Task.Delay(100);
        }

        await Task.Delay(800);
        LogService.Log($"[Station {_stationCode}] BG pin configuration complete, PLC ready for cavity validation");

        return true;
    }

    private async Task<bool> WriteStationBgPinMemoryAsync(string connectorSide, int pinNumber, int memory, bool value)
    {
        if (memory <= 0)
        {
            LogService.Log($"[Station {_stationCode}] BG pin skipped: {connectorSide}, Pin {pinNumber}, register not configured");
            return true;
        }

        var numericValue = value ? 1 : 0;
        var ok = await Task.Run(() => _plc.WriteHoldingRegister(memory, numericValue));
        if (!ok)
        {
            ShowError($"No se pudo enviar {connectorSide}, Pin {pinNumber}, D{memory}.");
            LogService.Log($"[Station {_stationCode}] Failed BG pin write: {connectorSide}, Pin {pinNumber}, D{memory} = {numericValue}");
            return false;
        }

        LogService.Log($"[Station {_stationCode}] BG pin sent with D2/D0: {connectorSide}, Pin {pinNumber}, D{memory} = {numericValue}");

        await Task.Delay(100);
        var readBack = await Task.Run(() => _plc.ReadHoldingRegister(memory));
        if (!_plc.IsConnected || readBack != numericValue)
        {
            LogService.Log($"[Station {_stationCode}] BG pin verification FAILED: {connectorSide}, Pin {pinNumber}, D{memory}, sent={numericValue}, read={readBack}");
            // El registro puede ser consumido y limpiado por el PLC. Esta
            // verificacion es diagnostica y no debe bloquear contador,
            // PlcQuantityProcessMemory ni PlcFinalProcessMemory.
            return true;
        }

        LogService.Log($"[Station {_stationCode}] BG pin verified: {connectorSide}, Pin {pinNumber}, D{memory} = {readBack}");
        return true;
    }

    private void UpdateUI()
    {
        UpdateMasterStatusPanel();
        var statusText = $"{_lblStation.Text} | {_processState}";

        switch (_processState)
        {
            case StationProcessState.DISABLED:
                lblInfo.Text = "Estación deshabilitada";
                lblInfo.ForeColor = Color.Gray;
                break;

            case StationProcessState.WAITING_ZEBRA:
                lblInfo.Text = $"En espera... Escanea {_stationConfig?.Code} para liberar Zebra";
                lblInfo.ForeColor = Color.Orange;
                break;

            case StationProcessState.ZEBRA_IN_PROGRESS:
                switch (_zebraStep)
                {
                    case ZebraValidationStep.STATION:
                        lblInfo.Text = $"Escanear código de estación: {_stationConfig?.Code}";
                        break;
                    case ZebraValidationStep.BG:
                        lblInfo.Text = $"Escanear BG: {_stationConfig?.BG}";
                        break;
                    case ZebraValidationStep.TOOLS:
                        var tool = GetCurrentToolForDisplay();
                        lblInfo.Text = $"Escanear herramienta: {tool?.Name}";
                        break;
                    case ZebraValidationStep.HOUSING:
                        lblInfo.Text = $"Escanear Housing: {GetExpectedHousingBarcode()}";
                        break;
                    case ZebraValidationStep.COMPLETE:
                        lblInfo.Text = "✓ Zebra completado";
                        break;
                }
                lblInfo.ForeColor = Color.Blue;
                break;

            case StationProcessState.COGNEX_ACTIVE:
                if (!CognexService.IsStationConnected(_config.StationNumber))
                {
                    lblInfo.Text = $"Cognex no conectado. Revisar {_config.ComPort} en Settings.";
                    lblInfo.ForeColor = Color.Red;
                }
                else if (_waitingRejectedCableRemoval)
                {
                    lblInfo.Text = GetRejectedCableInstruction();
                    lblInfo.ForeColor = Color.Red;
                }
                else if (_rejectedCableRemovalPending)
                {
                    lblInfo.Text = "Presiona boton para retirar serial rechazado";
                    lblInfo.ForeColor = Color.Red;
                }
                else if (_apiApproved)
                {
                    lblInfo.Text = $"API OK - enviando {_cableCount} cables al PLC";
                    lblInfo.ForeColor = Color.DarkGreen;
                }
                else
                {
                    lblInfo.Text = GetCognexActiveInstruction();
                    lblInfo.ForeColor = Color.Green;
                }
                break;

            case StationProcessState.COGNEX_DONE:
                if (_plcCounterSent)
                {
                    lblInfo.Text = GetCableInsertionInstruction();
                    lblInfo.ForeColor = Color.DarkOrange;
                }
                else
                {
                    lblInfo.Text = "Presiona Finalizar";
                    lblInfo.ForeColor = Color.DarkGreen;
                }
                break;

            case StationProcessState.PLC_RELEASED:
                lblInfo.Text = "PLC liberado";
                lblInfo.ForeColor = Color.DarkGreen;
                break;

            case StationProcessState.DONE:
                lblInfo.Text = "✓ ¡Completado!";
                lblInfo.ForeColor = Color.Green;
                break;
        }

        if (_processState == StationProcessState.WAITING_ZEBRA)
        {
            lblInfo.Text = GetWaitingReleaseInstruction();
            lblInfo.ForeColor = Color.Orange;
        }

        if (_waitingForMaterialBoxInsertion)
        {
            lblInfo.Text = GetMaterialBoxInstruction();
            lblInfo.ForeColor = Color.Red;
            _txtScanInput.Focus();
        }

        if (_cognexAdminLocked)
        {
            lblInfo.Text = $"BLOQUEADA - ADMIN: {_adminLockMessage}";
            lblInfo.ForeColor = Color.Red;
        }

        _btnAdminRecoveryReset.Visible = true;
        _btnAdminRecoveryReset.Enabled = !_adminUnlockDialogOpen;
        if (_btnAdminRecoveryReset.Visible)
            _btnAdminRecoveryReset.BringToFront();


        UpdateFinalizeButtonVisualState();
        ShowInstruction(lblInfo.Text, lblInfo.ForeColor);
        this.Text = statusText;
    }

    private void UpdateFinalizeButtonVisualState()
    {
        if (_waitingRejectedCableRemoval || _rejectedCableRemovalPending || _bgMismatchActive || _cognexAdminLocked)
        {
            // Cambio solicitado: cuando hay scrap, el boton final se muestra en rojo.
            _btnFinalize.BackColor = Color.Red;
            _btnFinalize.ForeColor = Color.White;
            return;
        }

        _btnFinalize.BackColor = Color.LimeGreen;
        _btnFinalize.ForeColor = Color.Black;
    }

    private string GetCableInsertionInstruction()
    {
        if (IsSharedRecipeActive())
        {
            var expectedCavities = GetSharedRecipeExpectedCavityCount();
            var cavityNumber = Math.Min(_scannedCables.Count, expectedCavities);
            return $"Ensamblar pieza al conector {cavityNumber}/{expectedCavities}.";
        }

        return "Insertar cable en el conector.";
    }

    private Tool? GetCurrentToolForDisplay()
    {
        if (_currentProcess == null)
            return null;

        var stationIndex = _currentProcess.EnabledStations.FindIndex(station =>
            string.Equals(station, _stationCode, StringComparison.OrdinalIgnoreCase));

        if (stationIndex < 0 || stationIndex >= _currentProcess.Tools.Count)
            return null;

        var tool = _currentProcess.Tools[stationIndex];
        return string.IsNullOrWhiteSpace(tool.Barcode)
            ? null
            : tool;
    }

    private bool ShouldScanToolForCurrentVariant()
    {
        if (_currentProcess == null)
            return false;

        if (string.Equals(
            _toolValidatedPartNumber,
            _currentProcess.PartNumber,
            StringComparison.OrdinalIgnoreCase))
            return false;

        return GetCurrentToolForDisplay() is not null;
    }

    public void Reset()
    {
        _processState = StationProcessState.DISABLED;
        _zebraStep = ZebraValidationStep.STATION;
        _currentProcess = null;
        _stationConfig = null;
        _scannedCables.Clear();
        _rejectedCables.Clear();
        _blockedRejectedCables.Clear();
        _cableCount = 0;
        _apiApproved = false;
        _waitingRejectedCableRemoval = false;
        _rejectedCableRemovalPending = false;
        _operatorActionInProgress = false;
        _cognexSerialReviewInProgress = false;
        _pendingBookingSerial = string.Empty;
        _completedBookingState = null;
        _bookingInProgress = false;
        _materialSensorRecheckInProgress = false;
        _waitingForMaterialBoxInsertion = false;
        _lastMaterialBoxPresentSignal = null;
        _toolValidatedPartNumber = string.Empty;
        _zebraReleasedForActivePartNumber = false;
        _bgMismatchActive = false;
        _cognexAdminLocked = false;
        _adminUnlockDialogOpen = false;
        _adminLockRequiresRejectedRemoval = false;
        _adminLockRequiresPlcQualityPulse = false;
        _adminLockCode = string.Empty;
        _adminLockMessage = string.Empty;
        _plcCounterSent = false;
        _cognexReadBlockedUntil = DateTime.MinValue;
        _lstCables.Items.Clear();
        _txtApiInfo.Clear();
        lblBG.Text = "--";
        SetScrapVisualState(false);
        _txtScanInput.Clear();
        _txtScanInput.Focus();
        UpdateUI();
    }

    public async Task ResetStationFromMainAsync()
    {
        var activeProcess = ProcessManager.GetCurrentProcess();

        await ResetCableInsertionOutputsAsync();
        if (_config.PlcScrapMemory > 0)
            await WriteScrapMemoryAsync(active: false);
        await ResetConfiguredBgPinRegistersAsync();

        Reset();

        if (activeProcess != null && activeProcess.EnabledStations.Contains(_stationCode))
        {
            OnProcessStarted(activeProcess);
            // El reset individual debe devolver el Zebra a esta estación.
            // Solo cambiar ProcessManager a WAITING_ZEBRA deja el escáner
            // asignado al flujo anterior y rechaza incluso el código correcto.
            ZebraFlowService.RestartZebraAtStation(_stationCode);
        }

        LogService.Log(
            $"[Station {_stationCode}] Individual station reset requested from FrmMain. " +
            $"State={_processState}, ZebraOwner={ZebraFlowService.GetStationWaitingForZebra()}");
    }

    public async Task ResetAllStationsFromMainAsync()
    {
        await SendGlobalResetPulseAsync();
        ZebraFlowService.Reset();
        CognexFlowService.Reset();
        ProcessManager.ResetProcess();
        LogService.Log("Global reset requested from FrmMain");
    }

    public void StopCognex()
    {
        _operatorButtonPollTimer.Stop();
        _operatorButtonPollTimer.Tick -= OnOperatorButtonPollTimerTick;
        _operatorButtonPollTimer.Dispose();
        ProcessManager.ProcessStarted -= OnProcessStarted;
        ProcessManager.StationStateChanged -= OnStationStateChanged;
        ProcessManager.ProcessReset -= OnProcessReset;
        ZebraFlowService.ZebraReadyForStation -= OnZebraReadyForStation;
        ZebraFlowService.ZebraCompletedAtStation -= OnZebraCompletedAtStation;
        CognexService.CableReceived -= OnCognexDataReceived;
    }

    private void ShowError(string message)
    {
        lblInfo.Text = $"✗ {message}";
        lblInfo.ForeColor = Color.Red;
        ShowInstruction(lblInfo.Text, lblInfo.ForeColor);

        // Después de 2 segundos, vuelve a mostrar la instrucción correcta
        System.Threading.Timer? timer = null;
        timer = new System.Threading.Timer(_ =>
        {
            this.BeginInvoke(() =>
            {
                UpdateUI();
                timer?.Dispose();
            });
        }, null, 2000, System.Threading.Timeout.Infinite);
    }

    private void ShowSuccess(string message)
    {
        lblInfo.Text = message;
        lblInfo.ForeColor = Color.Green;
        ShowInstruction(lblInfo.Text, lblInfo.ForeColor);
    }

    private void ShowInstruction(string text, Color color)
    {
        lblInfo.Visible = true;
        lblInfo.Text = text;
        lblInfo.ForeColor = color;
        lblInfo.BringToFront();
    }

    public void SetRecipeInfo(Recipe recipe)
    {
    }

    public void StartCableScan()
    {
    }

    public void SetInternalHeaderVisible(bool visible)
    {
   
    }

    public void SetInternalScanInputVisible(bool visible)
    {
        _txtScanInput.Visible = visible;
    }

    private void UpdateMasterStatusPanel()
    {
        if (!pnlMasterStatus.Visible)
            return;

        lblMasterPartNumber.Text = $"PartNumber: {_currentProcess?.PartNumber ?? "--"}";
        lblMasterStatus.Text = $"Status: {_currentProcess?.Description ?? "--"}";
        lblMasterProcessState.Text = _processState == StationProcessState.DISABLED
            ? "Esperando proceso..."
            : _processState.ToString();
        lblMasterProcessState.ForeColor = _processState switch
        {
            StationProcessState.DISABLED => Color.Orange,
            StationProcessState.WAITING_ZEBRA => Color.Orange,
            StationProcessState.ZEBRA_IN_PROGRESS => Color.Blue,
            StationProcessState.COGNEX_ACTIVE => Color.Green,
            StationProcessState.COGNEX_DONE or StationProcessState.PLC_RELEASED => Color.DarkGreen,
            StationProcessState.DONE => Color.Green,
            _ => Color.White
        };
    }

    private async void _btnFinalize_Click(object sender, EventArgs e)
    {
        // Boton de pantalla: usa el mismo flujo que el boton fisico del PLC.
        await HandleOperatorButtonSignalAsync();
    }

    private async void _btnReviewApi_Click(object sender, EventArgs e)
    {
        // Boton de pantalla: revisa API/finaliza usando el mismo flujo central.
        await HandleOperatorButtonSignalAsync();
    }

    private async Task ReviewApiAndFinalizeAsync()
    {
        if (_waitingRejectedCableRemoval)
        {
            ShowInstruction(GetRejectedCableInstruction(), Color.Red);
            _txtScanInput.Focus();
            return;
        }

        if (!CanReviewApi())
        {
            ShowError("No hay informacion escaneada para revisar.");
            return;
        }

        var request = new StationInfoApiRequest
        {
            Station = GetApiStationNumber(),
            PartNumber = GetRecipePartNumber(),
            SerialNumbers = [.. _scannedCables]
        };
        var requestJson = JsonSerializer.Serialize(request, _apiInfoJsonOptions);
        ShowApiInfo(string.Empty);
        _operatorActionInProgress = true;

        try
        {
            var result = await StationInfoApiService.ReviewSerialsIndividuallyAsync(request);
            ShowApiResult(result);
            if (!result.Success)
            {
                LogService.Log($"[Station {_stationCode}] API batch ERROR request={requestJson} " +
                    $"status={result.StatusCode?.ToString() ?? "N/A"} " +
                    $"resultCode={result.ResultCode?.ToString() ?? "N/A"} " +
                    $"resultText={result.ResultText}: {result.ResponseBody}");
            }

            if (result.Success)
            {
                _apiApproved = true;
                _waitingRejectedCableRemoval = false;
                _rejectedCableRemovalPending = false;
                _rejectedCables.Clear();
                ShowSuccess("CHECK OK. Enviando conteo al PLC.");
                await FinalizeStationCycleAsync();
            }
            else
            {
                _apiApproved = false;
                var rejectedSerials = GetConfirmedMesRejectedSerials(result);
                if (rejectedSerials.Count == 0)
                {
                    ShowError($"No se pudo confirmar la pieza con MES. Se requiere autorización para liberar. {result.Message}");
                    LogService.Log($"[Station {_stationCode}] MES technical/unconfirmed batch failure. Scrap not incremented. Message={result.Message}");
                    LockForCognexAdmin(
                        "MES_TECHNICAL_ERROR",
                        string.IsNullOrWhiteSpace(result.Message) ? "No fue posible confirmar las piezas con MES." : result.Message,
                        requiresRejectedRemoval: false);
                }
                else
                {
                    BeginRejectedCableRemoval(rejectedSerials);
                    ShowError(result.Message);
                    LockForCognexAdmin(
                        "API_REJECTED",
                        string.IsNullOrWhiteSpace(result.Message) ? "MES rechazó una o más piezas." : result.Message,
                        requiresRejectedRemoval: true);
                }
            }
        }
        finally
        {
            _operatorActionInProgress = false;
            UpdateUI();
        }
    }

    private async void _btnReset_Click(object sender, EventArgs e)
    {
        await SendGlobalResetPulseAsync();
        Reset();
        ZebraFlowService.Reset();
        CognexFlowService.Reset();
        ProcessManager.ResetProcess();
    }

    private async void _btnAdminRecoveryReset_Click(object sender, EventArgs e)
    {
        if (_adminUnlockDialogOpen)
            return;

        _adminUnlockDialogOpen = true;
        _btnAdminRecoveryReset.Enabled = false;
        try
        {
            using var dialog = new StationErrorAlertForm(
                _config.Name,
                "RESET_GENERAL",
                "Autoriza el reset general. Se reiniciarán PLC, proceso, Zebra y Cognex.",
                _config.AdminPass);
            var result = dialog.ShowDialog(FindForm());
            if (result != DialogResult.OK || !dialog.Authorized)
                return;

            var discardedSerial = _pendingBookingSerial;
            LogService.Log($"[Station {_stationCode}] Quality authorized GLOBAL recovery reset. Discarded pending serial={discardedSerial}");
            SystemEventLogService.LogInfo(
                "QUALITY_RECOVERY_RESET",
                $"Station {_stationCode}",
                $"Global recovery reset authorized. Discarded pending serial: {discardedSerial}");

            await SendGlobalResetPulseAsync();
            Reset();
            ZebraFlowService.Reset();
            CognexFlowService.Reset();
            ProcessManager.ResetProcess();
        }
        finally
        {
            _adminUnlockDialogOpen = false;
            UpdateUI();
            _txtScanInput.Focus();
        }
    }

    private void _btnResetGoodCount_Click(object sender, EventArgs e)
    {
        ResetGoodPieceCounter(_currentProcess?.PartNumber ?? _goodPiecePartNumber, "manual reset");
    }

    private async Task SendGlobalResetPulseAsync()
    {
        if (_config.PlcResetMemory <= 0)
        {
            LogService.Log($"[Station {_stationCode}] Global PLC reset skipped because ResetMemory is not configured");
            return;
        }

        ShowInstruction($"Activando reset global PLC M{_config.PlcResetMemory}...", Color.DarkOrange);
        var writeOk = await Task.Run(() => _plc.WriteCoil(_config.PlcResetMemory, true));
        if (writeOk)
        {
            LogService.Log($"[Station {_stationCode}] Global PLC reset memory M{_config.PlcResetMemory} = true");
            return;
        }

        LogService.Log($"[Station {_stationCode}] Failed to write global PLC reset memory M{_config.PlcResetMemory}");
        ShowError($"No se pudo enviar reset PLC M{_config.PlcResetMemory}");
    }

    private void _txtScanInput_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Return)
        {
            e.Handled = true;
            SubmitScan();
        }
    }

    private bool CanReviewApi()
    {
        return _currentProcess != null
            && _stationConfig != null
            && _cableCount > 0
            && (!IsSharedRecipeActive() || HasSharedRecipeReachedExpectedCavities())
            && _processState == StationProcessState.COGNEX_ACTIVE
            && !_apiApproved;
    }

    private string GetReviewApiButtonText()
    {
        if (_waitingRejectedCableRemoval)
            return "Re-scan Scrap";

        if (_rejectedCableRemovalPending)
            return "Remove Scrap";

        if (_apiApproved)
            return "API OK";

        return "Check API";
    }

    private string GetApiStationNumber()
    {
        return !string.IsNullOrWhiteSpace(_config.SapStationNumber)
            ? _config.SapStationNumber.Trim()
            : $"STATION_{_config.StationNumber:00}";
    }

    private string GetRecipePartNumber()
    {
        return _currentProcess?.PartNumber ?? string.Empty;
    }

    private void ShowApiInfo(string text)
    {
        _txtApiInfo.Text = text;
        _txtApiInfo.SelectionStart = 0;
        _txtApiInfo.SelectionLength = 0;
    }

    private void ShowApiResult(StationInfoApiResult result)
    {
        if (result.Success)
        {
            ShowApiInfo(string.Empty);
            return;
        }

        var errorMessage = !string.IsNullOrWhiteSpace(result.Message)
            ? result.Message.Trim()
            : "No fue posible validar la pieza con MES/API.";
        var resultText = string.IsNullOrWhiteSpace(result.ResultText)
            ? "Sin detalle"
            : result.ResultText.Trim();
        var resultCode = result.ResultCode?.ToString() ?? "Sin código";

        ShowApiInfo(
            $"ERROR MES/API{Environment.NewLine}" +
            $"{errorMessage}{Environment.NewLine}" +
            $"RESULT_CODE: {resultCode}{Environment.NewLine}" +
            $"RESULT_TEXT: {resultText}");
    }

    private void BeginRejectedCableRemoval(List<string> rejectedSerialNumbers)
    {
        _rejectedCables = rejectedSerialNumbers.Count > 0
            ? new HashSet<string>(rejectedSerialNumbers, StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Si API rechaza una pieza, se pide reescanearla para eliminarla sin presionar boton.
        _waitingRejectedCableRemoval = true;
        _rejectedCableRemovalPending = false;
        _totalScrapCount += Math.Max(1, rejectedSerialNumbers.Count);
        UpdateTotalScrapCountLabel();
        SoundAlertService.PlayScrapSound();
        SetScrapVisualState(true);
        MarkRejectedCables();
        UpdateUI();
    }

    private async Task<bool> ResetCableInsertionOutputsAsync()
    {
        // PlcFinalProcessMemory (M55/M56/M57) usa polaridad inversa:
        // ON bloquea el pistón y OFF permite manipularlo.
        if (!await WritePlcMemoryAsync(_config.PlcFinalProcessMemory, true, "cerrar pistón por N=N"))
            return false;

        // Después se limpia la señal auxiliar de proceso con cantidad.
        if (!await WritePlcMemoryAsync(_config.PlcQuantityProcessMemory, false, "reset proceso con cantidad"))
            return false;

        LogService.Log($"[Station {_stationCode}] Piston closed and insertion commands reset after N=N");
        return true;
    }

    private static List<string> GetConfirmedMesRejectedSerials(StationInfoApiResult result)
    {
        // Un fallo de red, timeout, configuración o HTTP no exitoso no es Scrap.
        // Scrap requiere una respuesta HTTP exitosa con RESULT_CODE distinto de cero.
        var confirmed = result.SerialResults
            .Where(serialResult => !serialResult.Success
                && serialResult.StatusCode is >= 200 and <= 299
                && serialResult.ResultCode.HasValue
                && serialResult.ResultCode.Value != 0)
            .Select(serialResult => serialResult.SerialNumber)
            .Where(serial => !string.IsNullOrWhiteSpace(serial))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (confirmed.Count > 0)
            return confirmed;

        var aggregateIsConfirmedRejection = result.StatusCode is >= 200 and <= 299
            && result.ResultCode.HasValue
            && result.ResultCode.Value != 0;

        return aggregateIsConfirmedRejection
            ? result.RejectedSerialNumbers
                .Where(serial => !string.IsNullOrWhiteSpace(serial))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
            : [];
    }

    private void ProcessRejectedCableRemoval(string barcode)
    {
        if (!_scannedCables.Contains(barcode, StringComparer.OrdinalIgnoreCase))
        {
            ShowError($"Serial no esta en el conteo: {barcode}");
            return;
        }

        if (_rejectedCables.Count > 0 && !_rejectedCables.Contains(barcode))
        {
            ShowError($"Serial no esta marcado como rechazado: {barcode}");
            return;
        }

        var removed = _scannedCables.RemoveAll(s =>
            string.Equals(s, barcode, StringComparison.OrdinalIgnoreCase));

        if (removed <= 0)
            return;

        _rejectedCables.Remove(barcode);
        _blockedRejectedCables.Add(barcode);
        StartNokCognexCooldown(barcode);
        _totalScannedCount = Math.Max(0, _totalScannedCount - removed);
        RemoveCableListItem(barcode);
        RefreshCableCount();
        _apiApproved = _scannedCables.Count > 0 && _rejectedCables.Count == 0;
        MarkRejectedCables();

        LogService.Log($"[Station {_stationCode}] Rejected cable removed by rescan: {barcode}. Count={_cableCount}");

        if (_rejectedCables.Count == 0)
        {
            _waitingRejectedCableRemoval = false;
            _rejectedCableRemovalPending = false;
            SetScrapVisualState(false);
            ShowSuccess("Serial rechazado retirado. Continua escaneando.");
        }
        else
        {
            _waitingRejectedCableRemoval = true;
            _rejectedCableRemovalPending = false;
            ShowError(GetRejectedCableInstruction());
        }

        UpdateUI();
    }

    private void RemoveRejectedCablesAfterQualityAuthorization()
    {
        var rejectedSerials = _rejectedCables.ToList();
        foreach (var serial in rejectedSerials)
        {
            var removed = _scannedCables.RemoveAll(value =>
                string.Equals(value, serial, StringComparison.OrdinalIgnoreCase));
            if (removed <= 0)
                continue;

            _blockedRejectedCables.Add(serial);
            _totalScannedCount = Math.Max(0, _totalScannedCount - removed);
            RemoveCableListItem(serial);

            if (string.Equals(_pendingBookingSerial, serial, StringComparison.OrdinalIgnoreCase))
            {
                _pendingBookingSerial = string.Empty;
                _completedBookingState = null;
            }
        }

        _rejectedCables.Clear();
        _waitingRejectedCableRemoval = false;
        _rejectedCableRemovalPending = false;
        _apiApproved = false;
        RefreshCableCount();
        MarkRejectedCables();
        SetScrapVisualState(false);
        LogService.Log($"[Station {_stationCode}] Rejected serial removed automatically after Quality authorization: {string.Join(", ", rejectedSerials)}");
        ShowSuccess("Calidad autorizó el rechazo. Retira la pieza y escanea el siguiente serial.");
    }

    private void StartNokCognexCooldown(string serial)
    {
        _cognexReadBlockedUntil = DateTime.Now.Add(NokCognexCooldown);
        LogService.Log($"[Station {_stationCode}] NOK cooldown started for {NokCognexCooldown.TotalSeconds:0}s after removing rejected serial: {serial}");
    }

    private void SetScrapVisualState(bool active)
    {
        if (_config.PlcScrapMemory > 0)
        {
            // Cambio solicitado: memoria de scrap configurable; 0 la deja deshabilitada hasta definir la M real.
            _ = WriteScrapMemoryAsync(active);
        }

        if (active)
        {
            _btnFinalize.BackColor = Color.Red;
            _btnFinalize.ForeColor = Color.White;
            return;
        }

        _btnFinalize.BackColor = Color.LimeGreen;
        _btnFinalize.ForeColor = Color.Black;
    }

    private void SetBgMismatchState(bool active)
    {
        _bgMismatchActive = active;
        SetScrapVisualState(active);
    }

    private void LockForCognexAdmin(string errorCode, string message, bool requiresRejectedRemoval)
    {
        _cognexAdminLocked = true;
        _adminLockCode = errorCode;
        _adminLockMessage = message;
        _adminLockRequiresRejectedRemoval = requiresRejectedRemoval;
        _adminLockRequiresPlcQualityPulse = false;
        SetScrapVisualState(true);
        LogService.Log($"[Station {_stationCode}] ADMIN LOCK activated. Code={errorCode}, PN={GetRecipePartNumber()}, Message={message}");
        SystemEventLogService.LogError("COGNEX_ADMIN_LOCK", $"Station {_stationCode}", $"{errorCode}: {message}");
        UpdateUI();
        ShowAdminUnlockDialog();
    }

    private async void ShowAdminUnlockDialog()
    {
        if (!_cognexAdminLocked || _adminUnlockDialogOpen || IsDisposed)
            return;

        if (InvokeRequired)
        {
            BeginInvoke(ShowAdminUnlockDialog);
            return;
        }

        _adminUnlockDialogOpen = true;
        try
        {
            var restartCognexAfterQualityAuthorization = _adminLockRequiresPlcQualityPulse;
            var discardSerialAfterMesTechnicalError = string.Equals(_adminLockCode, "MES_TECHNICAL_ERROR", StringComparison.Ordinal);
            var completeAfterBookingError = string.Equals(_adminLockCode, "BOOKING_ERROR", StringComparison.Ordinal);
            using var dialog = new StationErrorAlertForm(
                _config.Name,
                _adminLockCode,
                _adminLockMessage,
                _config.AdminPass);
            var result = dialog.ShowDialog(FindForm());
            if (dialog.FailedAttempts > 0)
                LogService.Log($"[Station {_stationCode}] Admin unlock failed attempts: {dialog.FailedAttempts}. Code={_adminLockCode}");

            if (result != DialogResult.OK || !dialog.Authorized)
            {
                LogService.Log($"[Station {_stationCode}] Admin unlock cancelled. Code={_adminLockCode}");
                return;
            }

            if (_adminLockRequiresPlcQualityPulse)
            {
                if (!string.IsNullOrWhiteSpace(_pendingBookingSerial)
                    && _completedBookingState != 2
                    && !await BookPendingSerialAsync(2))
                {
                    // La autorizacion administrativa debe poder limpiar el serial aunque
                    // MES no permita registrar el scrap. El fallo queda en bitacora.
                    LogService.Log($"[Station {_stationCode}] Scrap booking failed during PLC error authorization. Continuing with manual serial discard. Serial={_pendingBookingSerial}");
                    SystemEventLogService.LogInfo("MES_SCRAP_BYPASS", $"Station {_stationCode}", $"Manual discard: {_pendingBookingSerial}");
                }

                var plcErrorStillActive = await Task.Run(() => _plc.ReadCoil(_config.PlcCavityErrorMemory));
                if (!_plc.IsConnected)
                {
                    ShowError("PLC no conectado. La estación permanece bloqueada.");
                    return;
                }

                if (plcErrorStillActive)
                {
                    ShowInstruction($"Enviando autorización de Calidad a M{_config.PlcQualityAuthorizationMemory}...", Color.DarkOrange);
                    var pulseOk = _config.PlcQualityAuthorizationMemory > 0
                        && await Task.Run(() => _plc.SendPulse(_config.PlcQualityAuthorizationMemory, 5000));
                    if (!pulseOk)
                    {
                        ShowError($"No se pudo enviar la autorización al PLC M{_config.PlcQualityAuthorizationMemory}.");
                        LogService.Log($"[Station {_stationCode}] Failed quality authorization pulse on M{_config.PlcQualityAuthorizationMemory}");
                        return;
                    }

                    var errorCleared = await WaitForCavityErrorToClearAsync();
                    if (!errorCleared)
                    {
                        ShowError($"El PLC no liberó el error M{_config.PlcCavityErrorMemory}. Presiona el botón para reintentar la autorización.");
                        LogService.Log($"[Station {_stationCode}] Cavity error remained active after quality pulse. M{_config.PlcCavityErrorMemory}=1");
                        return;
                    }
                }
                else
                {
                    LogService.Log($"[Station {_stationCode}] PLC cavity error was already inactive when admin authorization was accepted; quality pulse skipped");
                }

                // La pieza que produjo el error de cavidad ya fue declarada scrap.
                // Antes de liberar la estacion, vuelve el PLC a la condicion inicial:
                // piston bloqueado y comando de insercion anterior apagado.
                if (!await ResetCableInsertionOutputsAsync())
                {
                    ShowError("No se pudo reiniciar el ciclo de insercion. La estacion permanece bloqueada.");
                    return;
                }
            }

            if (completeAfterBookingError && !await ResetCableInsertionOutputsAsync())
            {
                ShowError("No se pudo reiniciar la estación en el PLC. Presiona el botón para reintentar.");
                return;
            }

            var restartCognexWithoutSerial = _adminLockRequiresPlcQualityPulse
                && string.IsNullOrWhiteSpace(_pendingBookingSerial);

            if (restartCognexWithoutSerial && !await ResetCableInsertionOutputsAsync())
            {
                ShowError("No se pudo reiniciar la estación en el PLC. Presiona el botón para reintentar.");
                return;
            }

            _cognexAdminLocked = false;
            LogService.Log($"[Station {_stationCode}] ADMIN LOCK released. Code={_adminLockCode}, PN={GetRecipePartNumber()}");
            SystemEventLogService.LogInfo("COGNEX_ADMIN_LOCK", $"Station {_stationCode}", $"Unlocked: {_adminLockCode}");

            if (_adminLockRequiresRejectedRemoval)
            {
                RemoveRejectedCablesAfterQualityAuthorization();
            }
            else if (restartCognexAfterQualityAuthorization)
            {
                // Descarta el serial que fue insertado incorrectamente y comienza
                // un ciclo completo nuevo desde Cognex/API para la siguiente pieza.
                RestartStationAfterCounterComplete();
                await SetYellowButtonAsync(active: true, "scrap autorizado");
                ProcessManager.RestartStationCognexActive(_stationCode);
                CognexFlowService.StartCognexAtStation(_stationCode);
                ShowSuccess("Scrap autorizado. Escanea un nuevo serial en Cognex.");
                LogService.Log($"[Station {_stationCode}] Scrap cycle restarted after Quality authorization; rejected serial and insertion state cleared");
            }
            else if (completeAfterBookingError)
            {
                var releasedSerial = _pendingBookingSerial;
                CountGoodPieces([releasedSerial]);
                MarkCableAsOk(releasedSerial);
                LogService.Log($"[Station {_stationCode}] MES booking bypass authorized by admin. Serial={releasedSerial}");
                SystemEventLogService.LogInfo("MES_BOOKING_BYPASS", $"Station {_stationCode}", $"Manual release: {releasedSerial}");
                RestartStationAfterCounterComplete();
                await SetYellowButtonAsync(active: true, "MES booking bypass");
                ProcessManager.RestartStationCognexActive(_stationCode);
                CognexFlowService.StartCognexAtStation(_stationCode);
                ShowSuccess("Autorización correcta. Ciclo liberado; escanea un nuevo serial.");
            }
            else if (discardSerialAfterMesTechnicalError)
            {
                var discardedSerial = _pendingBookingSerial;
                LogService.Log($"[Station {_stationCode}] MES technical error authorized by admin. Discarding serial={discardedSerial}");
                SystemEventLogService.LogInfo("MES_SERIAL_DISCARDED", $"Station {_stationCode}", $"Discarded after MES error: {discardedSerial}");
                RestartStationAfterCounterComplete();
                await SetYellowButtonAsync(active: true, "MES serial discarded");
                ProcessManager.RestartStationCognexActive(_stationCode);
                CognexFlowService.StartCognexAtStation(_stationCode);
                ShowSuccess("Autorización correcta. Serial eliminado; escanea uno nuevo.");
            }
            else
            {
                _bgMismatchActive = false;
                SetScrapVisualState(false);
                ShowSuccess("Autorizacion correcta. Continua escaneando.");
            }

            _adminLockCode = string.Empty;
            _adminLockMessage = string.Empty;
            _adminLockRequiresRejectedRemoval = false;
            _adminLockRequiresPlcQualityPulse = false;

            if (restartCognexWithoutSerial)
            {
                RestartStationAfterCounterComplete();
                await SetYellowButtonAsync(active: true, "cavity error without serial");
                ProcessManager.RestartStationCognexActive(_stationCode);
                CognexFlowService.StartCognexAtStation(_stationCode);
                ShowSuccess("Calidad autorizó. Esperando serial Cognex...");
            }

            UpdateUI();
            _txtScanInput.Focus();
        }
        finally
        {
            _adminUnlockDialogOpen = false;
        }
    }

    private async Task<bool> WaitForCavityErrorToClearAsync()
    {
        for (var attempt = 0; attempt < 50; attempt++)
        {
            var active = await Task.Run(() => _plc.ReadCoil(_config.PlcCavityErrorMemory));
            if (_plc.IsConnected && !active)
                return true;

            await Task.Delay(100);
        }

        return false;
    }

    private async Task WriteScrapMemoryAsync(bool active)
    {
        var ok = await Task.Run(() => _plc.WriteCoil(_config.PlcScrapMemory, active));
        if (ok)
        {
            LogService.Log($"[Station {_stationCode}] PLC scrap memory M{_config.PlcScrapMemory} = {active}");
            return;
        }

        LogService.Log($"[Station {_stationCode}] Failed to write PLC scrap memory M{_config.PlcScrapMemory} = {active}");
    }

    private void RefreshCableCount()
    {
        _cableCount = _scannedCables.Count;
        UpdateInsertedCountLabelText();
    }


    private void UpdateTotalScrapCountLabel()
    {
        _lblScrapCount.Text = $"Scrap: {_totalScrapCount}";
    }

    private void CountGoodPieces(IEnumerable<string> serials)
    {
        EnsureCurrentGoodPieceHour();
        var added = 0;
        foreach (var serial in serials.Where(serial => !string.IsNullOrWhiteSpace(serial)))
        {
            if (_countedGoodSerials.Add(serial))
                added++;
        }

        if (added == 0)
            return;

        _goodPieceCount += added;
        _bgGoodPieceCount += added;
        _hourlyGoodPieceCount += added;
        UpdateGoodPieceCountLabel();
        LogService.Log($"[Station {_stationCode}] Good piece counter +{added}. Total={_goodPieceCount}, PN={_goodPiecePartNumber}");
    }

    private void ResetGoodPieceCounter(string partNumber, string reason)
    {
        _goodPieceCount = 0;
        _bgGoodPieceCount = 0;
        _goodPieceBg = _stationConfig?.BG?.Trim() ?? _goodPieceBg;
        _hourlyGoodPieceCount = 0;
        _hourlyCounterStart = GetHourStart(DateTime.Now);
        _goodPiecePartNumber = partNumber;
        _countedGoodSerials.Clear();
        UpdateGoodPieceCountLabel();
        LogService.Log($"[Station {_stationCode}] Good piece counter reset. PN={partNumber}, Reason={reason}");
    }

    private void UpdateGoodPieceCountLabel()
    {
        _lblGoodPieceCount.Text =
            $"Total estación: {_goodPieceCount}{Environment.NewLine}" +
            $"BG {GetGoodPieceBgDisplay()}: {_bgGoodPieceCount}{Environment.NewLine}" +
            $"Hora {_hourlyCounterStart:HH}:00-{_hourlyCounterStart.AddHours(1):HH}:00: {_hourlyGoodPieceCount}";
    }

    private void SetGoodPieceBg(string bg)
    {
        var normalizedBg = bg.Trim();
        if (string.Equals(_goodPieceBg, normalizedBg, StringComparison.OrdinalIgnoreCase))
            return;

        if (!string.IsNullOrWhiteSpace(_goodPieceBg))
            LogService.Log($"[Station {_stationCode}] BG good piece counter closed. BG={_goodPieceBg}, Total={_bgGoodPieceCount}");

        _goodPieceBg = normalizedBg;
        _bgGoodPieceCount = 0;
        UpdateGoodPieceCountLabel();
    }

    private string GetGoodPieceBgDisplay() =>
        string.IsNullOrWhiteSpace(_goodPieceBg) ? "--" : _goodPieceBg;

    private void EnsureCurrentGoodPieceHour()
    {
        var currentHour = GetHourStart(DateTime.Now);
        if (currentHour == _hourlyCounterStart)
            return;

        LogService.Log($"[Station {_stationCode}] Hourly good piece counter closed. Hour={_hourlyCounterStart:yyyy-MM-dd HH}:00, Total={_hourlyGoodPieceCount}, PN={_goodPiecePartNumber}");
        _hourlyCounterStart = currentHour;
        _hourlyGoodPieceCount = 0;
        UpdateGoodPieceCountLabel();
    }

    private static DateTime GetHourStart(DateTime value) =>
        new(value.Year, value.Month, value.Day, value.Hour, 0, 0);

    public GoodPieceCounterSnapshot GetGoodPieceCounterSnapshot()
    {
        EnsureCurrentGoodPieceHour();
        return new GoodPieceCounterSnapshot(
            _config.StationNumber,
            _config.Name,
            _goodPieceCount,
            _goodPieceBg,
            _bgGoodPieceCount,
            _hourlyGoodPieceCount,
            _hourlyCounterStart);
    }

    public void ResetGoodPieceCountersFromMain()
    {
        ResetGoodPieceCounter(_currentProcess?.PartNumber ?? _goodPiecePartNumber, "central counter form");
    }

    public void ResetTotalGoodPieceCounterFromMain()
    {
        _goodPieceCount = 0;
        UpdateGoodPieceCountLabel();
        LogService.Log($"[Station {_stationCode}] Total good piece counter reset from central counter form.");
    }

    public void ResetBgGoodPieceCounterFromMain()
    {
        _bgGoodPieceCount = 0;
        UpdateGoodPieceCountLabel();
        LogService.Log($"[Station {_stationCode}] BG good piece counter reset from central counter form. BG={GetGoodPieceBgDisplay()}");
    }

    public void ResetHourlyGoodPieceCounterFromMain()
    {
        _hourlyGoodPieceCount = 0;
        _hourlyCounterStart = GetHourStart(DateTime.Now);
        UpdateGoodPieceCountLabel();
        LogService.Log($"[Station {_stationCode}] Hourly good piece counter reset from central counter form. Hour={_hourlyCounterStart:yyyy-MM-dd HH}:00");
    }

    public sealed record GoodPieceCounterSnapshot(
        int StationNumber,
        string StationName,
        int Total,
        string Bg,
        int BgTotal,
        int HourTotal,
        DateTime HourStart);

    private int _insertedCount = 0;

    private void UpdateInsertedCountLabel(int insertedCount)
    {
        _insertedCount = insertedCount;
        UpdateInsertedCountLabelText();
    }

    private void UpdateInsertedCountLabelText()
    {
        var expected = IsSharedRecipeActive()
            ? GetSharedRecipeExpectedCavityCount()
            : _cableCount;
        _lblInsertedCount.Text = $"Insertadas {_insertedCount}/{expected}";
    }

    private ListViewItem CreateCableListItem(string serial)
    {
        return new ListViewItem(serial);
    }

    private void RemoveCableListItem(string serial)
    {
        foreach (ListViewItem item in _lstCables.Items)
        {
            if (string.Equals(item.Text, serial, StringComparison.OrdinalIgnoreCase))
            {
                _lstCables.Items.Remove(item);
                return;
            }
        }
    }

    private void MarkRejectedCables()
    {
        foreach (ListViewItem item in _lstCables.Items)
        {
            if (_rejectedCables.Contains(item.Text))
            {
                item.BackColor = Color.MistyRose;
                item.ForeColor = Color.Red;
            }
            else
            {
                item.BackColor = Color.White;
                item.ForeColor = Color.Black;
            }
        }
    }

    private void MarkAllCablesAsOk()
    {
        foreach (ListViewItem item in _lstCables.Items)
        {
            item.BackColor = Color.Honeydew;
            item.ForeColor = Color.DarkGreen;
        }
    }

    private void MarkCableAsOk(string serial)
    {
        foreach (ListViewItem item in _lstCables.Items)
        {
            if (string.Equals(item.Text, serial, StringComparison.OrdinalIgnoreCase))
            {
                item.BackColor = Color.Honeydew;
                item.ForeColor = Color.DarkGreen;
                return;
            }
        }
    }

    private string GetRejectedCableInstruction()
    {
        if (_rejectedCables.Count == 0)
            return "Escanea el serial con error para retirarlo del contador.";

        return $"Retirar serial rechazado: {string.Join(", ", _rejectedCables)}";
    }

    private void lblBG_Click(object sender, EventArgs e)
    {

    }

    private void StationPanel_Load(object sender, EventArgs e)
    {

    }

    private void panel3_Paint(object sender, PaintEventArgs e)
    {

    }
}
