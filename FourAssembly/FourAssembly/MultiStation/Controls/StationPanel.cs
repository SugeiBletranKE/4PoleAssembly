namespace FourAssembly.MultiStation.Controls;

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
    private bool _apiApproved = false;
    private bool _waitingRejectedCableRemoval = false;
    private bool _rejectedCableRemovalPending = false;
    private bool _operatorActionInProgress = false;
    private bool _cognexSerialReviewInProgress = false;
    private bool _plcCounterSent = false;
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
        _lblStation.Text = _config.Name;
        _lblCableCount.BringToFront();
        _lblScrapCount.BringToFront();
        _lblInsertedCount.BringToFront();
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
            && _config.PlcMaterialRemovedMemory <= 0)
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
            await CheckPlcCounterCompleteSignalAsync();
            await UpdateInsertedCountFromPlcAsync();
        }
        finally
        {
            _operatorButtonPollInProgress = false;
        }
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
        if (!_plcCounterSent
            || _processState != StationProcessState.COGNEX_DONE
            || _config.PlcCounterCompleteMemory <= 0
            || _materialSensorRecheckInProgress
            || _waitingForMaterialBoxInsertion)
            return;

        // Cambio solicitado: la estacion solo se bloquea cuando PLC confirma N=N en M44/M49/M54.
        var signal = await Task.Run(() => _plc.ReadCoil(_config.PlcCounterCompleteMemory));
        if (!_plc.IsConnected || !signal)
            return;

        LogService.Log($"[Station {_stationCode}] PLC counter complete signal received on M{_config.PlcCounterCompleteMemory}");
        RestartStationAfterCounterComplete();
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

        UpdateInsertedCountLabel(insertedCount);
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
        _materialSensorRecheckInProgress = false;
        _waitingForMaterialBoxInsertion = false;
        _plcCounterSent = false;
        _cognexReadBlockedUntil = DateTime.MinValue;
        _lstCables.Items.Clear();
        UpdateTotalScannedCountLabel();
        UpdateTotalScrapCountLabel();
        UpdateInsertedCountLabel(0);
        _btnReviewApi.Enabled = false;
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
        }

        _scannedCables.Clear();
        _rejectedCables.Clear();
        _blockedRejectedCables.Clear();
        _cableCount = 0;
        _apiApproved = false;
        _waitingRejectedCableRemoval = false;
        _rejectedCableRemovalPending = false;
        _plcCounterSent = false;
        _materialSensorRecheckInProgress = false;
        _waitingForMaterialBoxInsertion = false;
        _bgMismatchActive = false;
        _cognexReadBlockedUntil = DateTime.MinValue;
        _lstCables.Items.Clear();
        _txtApiInfo.Clear();
        _btnReviewApi.Enabled = false;
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
        LogService.Log($"[Station {_stationCode}] Housing re-scan completed after sensor signal");
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
        return IsFirstEnabledStation()
            && !string.IsNullOrWhiteSpace(GetExpectedHousingBarcode());
    }

    private bool HasHousingForActiveProcess()
    {
        return !string.IsNullOrWhiteSpace(GetExpectedHousingBarcode());
    }

    private bool CanRequestHousingFromMaterialSensor()
    {
        // Una vez liberado Zebra para el PN activo, la memoria de la caja debe
        // vigilarse durante todo el proceso: Cognex, espera de N=N y batches
        // posteriores. Solo el cambio de PN desactiva esta vigilancia hasta
        // completar nuevamente la validacion Zebra.
        return _zebraReleasedForActivePartNumber
            && _currentProcess != null
            && IsHousingEnabled();
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
            return true;

        var scanned = NormalizeHousingCode(barcode);
        var expected = NormalizeHousingCode(expectedHousing);
        return !string.IsNullOrWhiteSpace(expected)
            && scanned.Contains(expected, StringComparison.OrdinalIgnoreCase);
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
        return IsFirstEnabledStation();
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

        _scannedCables.Add(data);
        _totalScannedCount++;
        _lstCables.Items.Add(CreateCableListItem(data));
        RefreshCableCount();
        UpdateTotalScannedCountLabel();
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
        ShowError($"BG Cognex incorrecto. El serial debe terminar en: {expectedBg}");
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
        ShowApiInfo($"REQUEST{Environment.NewLine}{requestJson}");
        _btnReviewApi.Enabled = false;
        _cognexSerialReviewInProgress = true;

        try
        {
            var result = await StationInfoApiService.ReviewSerialsIndividuallyAsync(request);
            ShowApiResult(requestJson, result);
            LogService.Log($"[Station {_stationCode}] API single serial response serial={serial} success={result.Success} resultCode={result.ResultCode?.ToString() ?? "N/A"} resultText={result.ResultText}: {result.ResponseBody}");
            LogMesPieceReview(serial, result);

            if (result.Success)
            {
                _apiApproved = true;
                MarkCableAsOk(serial);
                ShowSuccess($"API OK: {serial}");
                return;
            }

            _apiApproved = false;
            var rejectedSerials = result.RejectedSerialNumbers.Count > 0
                ? result.RejectedSerialNumbers
                : [serial];
            BeginRejectedCableRemoval(rejectedSerials);
            ShowError(result.Message);
        }
        finally
        {
            _cognexSerialReviewInProgress = false;
            _btnReviewApi.Enabled = CanReviewApi();
            UpdateUI();
        }
    }

    private void LogMesPieceReview(string serial, StationInfoApiResult result)
    {
        var serialResult = result.SerialResults.FirstOrDefault(r =>
            string.Equals(r.SerialNumber, serial, StringComparison.OrdinalIgnoreCase));

        // Cambio solicitado: guardar trazabilidad MES de cada pieza revisada por Cognex/API.
        MesPieceLogService.LogPieceReview(new MesPieceLogEntry
        {
            Station = GetApiStationNumber(),
            PartNumber = GetRecipePartNumber(),
            SerialNumber = serial,
            CableCount = _cableCount,
            Success = result.Success,
            StatusCode = serialResult?.StatusCode ?? result.StatusCode,
            ResultCode = serialResult?.ResultCode ?? result.ResultCode,
            ResultText = serialResult?.ResultText ?? result.ResultText,
            Message = serialResult?.Message ?? result.Message,
            ResponseBody = serialResult?.ResponseBody ?? result.ResponseBody
        });
    }

    private async Task FinalizeStationCycleAsync()
    {
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

        // Cambio solicitado: al presionar el boton final, se prende M61/M62/M63 para avisar que se mando la cantidad.
        if (!await WritePlcMemoryAsync(_config.PlcQuantityProcessMemory, true, "proceso con cantidad"))
            return;

        // Cambio solicitado: se manda M55/M56/M57, pero la estacion NO se marca DONE hasta leer M44/M49/M54.
        if (!await WritePlcMemoryAsync(_config.PlcFinalProcessMemory, true, "proceso final"))
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
        return string.Equals((_currentProcess?.ConnectorGender ?? string.Empty).Trim(), "B", StringComparison.OrdinalIgnoreCase)
            ? "B"
            : "A";
    }

    private async Task<bool> WriteActiveStationAndConnectorSideAsync(string connectorCode)
    {
        if (_config.PlcActiveStationRegister > 0)
        {
            var plcStationValue = _config.PlcStation > 0
                ? _config.PlcStation
                : _config.StationNumber;
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
                if (_waitingRejectedCableRemoval)
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
                    lblInfo.Text = $"Cognex activo - {_cableCount} cables. Revisar API.";
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

        _btnReviewApi.Enabled = CanReviewApi();
        _btnReviewApi.Text = GetReviewApiButtonText();
        UpdateFinalizeButtonVisualState();
        ShowInstruction(lblInfo.Text, lblInfo.ForeColor);
        this.Text = statusText;
    }

    private void UpdateFinalizeButtonVisualState()
    {
        if (_waitingRejectedCableRemoval || _rejectedCableRemovalPending || _bgMismatchActive)
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
        _materialSensorRecheckInProgress = false;
        _waitingForMaterialBoxInsertion = false;
        _lastMaterialBoxPresentSignal = null;
        _zebraReleasedForActivePartNumber = false;
        _bgMismatchActive = false;
        _plcCounterSent = false;
        _cognexReadBlockedUntil = DateTime.MinValue;
        _lstCables.Items.Clear();
        UpdateTotalScannedCountLabel();
        _btnReviewApi.Enabled = false;
        _txtApiInfo.Clear();
        lblBG.Text = "--";
        SetScrapVisualState(false);
        _txtScanInput.Clear();
        _txtScanInput.Focus();
        UpdateUI();
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
        ShowApiInfo($"REQUEST{Environment.NewLine}{requestJson}");
        LogService.Log($"[Station {_stationCode}] API request: {requestJson}");

        _btnReviewApi.Enabled = false;
        var originalText = _btnReviewApi.Text;
        _btnReviewApi.Text = "Review...";
        _operatorActionInProgress = true;

        try
        {
            var result = await StationInfoApiService.ReviewSerialsIndividuallyAsync(request);
            ShowApiResult(requestJson, result);
            LogService.Log($"[Station {_stationCode}] API response status={result.StatusCode?.ToString() ?? "N/A"} success={result.Success} resultCode={result.ResultCode?.ToString() ?? "N/A"} resultText={result.ResultText}: {result.ResponseBody}");

            if (result.Success)
            {
                _apiApproved = true;
                _waitingRejectedCableRemoval = false;
                _rejectedCableRemovalPending = false;
                _rejectedCables.Clear();
                MarkAllCablesAsOk();
                ShowSuccess("API OK. Enviando conteo al PLC.");
                await FinalizeStationCycleAsync();
            }
            else
            {
                _apiApproved = false;
                BeginRejectedCableRemoval(result.RejectedSerialNumbers);
                ShowError(result.Message);
            }
        }
        finally
        {
            _operatorActionInProgress = false;
            _btnReviewApi.Text = originalText;
            _btnReviewApi.Enabled = CanReviewApi();
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

    private void ShowApiResult(string requestJson, StationInfoApiResult result)
    {
        var responseBody = string.IsNullOrWhiteSpace(result.ResponseBody)
            ? "(sin cuerpo de respuesta)"
            : result.ResponseBody;
        var serialDetails = result.SerialResults.Count == 0
            ? "(sin revision individual)"
            : string.Join(Environment.NewLine, result.SerialResults.Select(r =>
                $"{r.SerialNumber}: {(r.Success ? "OK" : "NOK")} | RESULT_CODE: {r.ResultCode?.ToString() ?? "N/A"} | {r.ResultText}"));

        ShowApiInfo(
            $"REQUEST{Environment.NewLine}{requestJson}{Environment.NewLine}{Environment.NewLine}" +
            $"RESPONSE{Environment.NewLine}" +
            $"Success: {result.Success}{Environment.NewLine}" +
            $"StatusCode: {result.StatusCode?.ToString() ?? "N/A"}{Environment.NewLine}" +
            $"RESULT_CODE: {result.ResultCode?.ToString() ?? "N/A"}{Environment.NewLine}" +
            $"RESULT_TEXT: {result.ResultText}{Environment.NewLine}" +
            $"RejectedSerialNumbers: {string.Join(", ", result.RejectedSerialNumbers)}{Environment.NewLine}" +
            $"Message: {result.Message}{Environment.NewLine}" +
            $"SerialResults:{Environment.NewLine}{serialDetails}{Environment.NewLine}" +
            $"Body:{Environment.NewLine}{responseBody}");
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
        UpdateTotalScannedCountLabel();
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

    private void UpdateTotalScannedCountLabel()
    {
        _lblCableCount.Text = $"Escaneadas: {_totalScannedCount}";
    }

    private void UpdateTotalScrapCountLabel()
    {
        _lblScrapCount.Text = $"Scrap: {_totalScrapCount}";
    }

    private int _insertedCount = 0;

    private void UpdateInsertedCountLabel(int insertedCount)
    {
        _insertedCount = insertedCount;
        UpdateInsertedCountLabelText();
    }

    private void UpdateInsertedCountLabelText()
    {
        _lblInsertedCount.Text = $"Insertadas {_insertedCount}/{_cableCount}";
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
}
