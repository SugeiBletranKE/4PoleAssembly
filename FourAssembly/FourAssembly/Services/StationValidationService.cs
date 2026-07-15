namespace FourAssembly.Services;

using FourAssembly.Models;

public static class StationValidationService
{
    public static bool ValidateStationCode(string scannedCode, string stationCode)
    {
        return scannedCode == stationCode;
    }

    public static bool ValidateBG(string scannedBG, StationConfig stationConfig)
    {
        var expectedBG = stationConfig.BG.Trim();
        if (string.IsNullOrWhiteSpace(expectedBG))
            return true;

        return scannedBG
            .Trim()
            .Contains(expectedBG, StringComparison.OrdinalIgnoreCase);
    }

    public static ValidationResult ValidateTool(
        string barcode,
        int toolIndex,
        List<Tool> tools)
    {
        if (toolIndex >= tools.Count)
            return new ValidationResult(false, "No more tools to scan");

        var expectedTool = tools[toolIndex];
        if (expectedTool.Barcode != barcode)
            return new ValidationResult(
                false,
                $"Expected: {expectedTool.Name} ({expectedTool.Barcode})");

        return new ValidationResult(true, "Tool validated");
    }

    public static ValidationResult ValidateTool(string barcode, Tool expectedTool)
    {
        if (expectedTool.Barcode != barcode)
            return new ValidationResult(
                false,
                $"Expected: {expectedTool.Name} ({expectedTool.Barcode})");

        return new ValidationResult(true, "Tool validated");
    }

    public static bool ValidateHousing(string scannedHousing, StationConfig stationConfig)
    {
        if (!string.Equals(stationConfig.HousingEnable, "TRUE", StringComparison.OrdinalIgnoreCase))
            return true;

        var expectedHousing = stationConfig.Housing.Trim();
        if (string.IsNullOrWhiteSpace(expectedHousing))
            return true;

        return scannedHousing
            .Trim()
            .Contains(expectedHousing, StringComparison.OrdinalIgnoreCase);
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; }

    public ValidationResult(bool isValid, string message)
    {
        IsValid = isValid;
        Message = message;
    }
}
