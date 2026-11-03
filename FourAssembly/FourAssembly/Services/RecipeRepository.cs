namespace FourAssembly.Services;

using System.Text.Json;
using FourAssembly.Models;

public static class RecipeRepository
{
    private static readonly string _dirPath = @"C:\ke\FourPole\Recipes";

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    private static List<Recipe> _cache = [];

    public static IReadOnlyList<Recipe> GetAll() => _cache;

    public static Recipe? FindByPartNumber(string pn) =>
        _cache.FirstOrDefault(r =>
            string.Equals(r.PartNumber, pn, StringComparison.OrdinalIgnoreCase));

    public static Recipe? FindByScannedPartNumber(string scannedCode)
    {
        var code = scannedCode.Trim();
        if (string.IsNullOrWhiteSpace(code))
            return null;

        var exactMatch = FindByPartNumber(code);
        if (exactMatch is not null)
            return exactMatch;

        return _cache
            .Where(r =>
                !string.IsNullOrWhiteSpace(r.PartNumber)
                && code.Contains(r.PartNumber, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(r => r.PartNumber.Length)
            .FirstOrDefault();
    }

    public static void Load()
    {
        EnsureDirectoryExists();
        _cache.Clear();

        var files = Directory.GetFiles(_dirPath, "*.json");
        foreach (var file in files)
        {
            var json = File.ReadAllText(file);
            var recipe = JsonSerializer.Deserialize<Recipe>(json, _jsonOptions);
            if (recipe is not null)
            {
                NormalizeRecipe(recipe);
                _cache.Add(recipe);
            }
        }
    }

    public static void Save()
    {
        EnsureDirectoryExists();
        foreach (var recipe in _cache)
        {
            SaveRecipe(recipe);
        }
    }

    private static void SaveRecipe(Recipe recipe)
    {
        EnsureDirectoryExists();
        var fileName = GetFileName(recipe.PartNumber);
        var json = JsonSerializer.Serialize(recipe, _jsonOptions);
        File.WriteAllText(fileName, json);
    }

    private static string GetFileName(string partNumber) =>
        Path.Combine(_dirPath, $"{partNumber}.json");

    private static void EnsureDirectoryExists()
    {
        if (!Directory.Exists(_dirPath))
            Directory.CreateDirectory(_dirPath);
    }

    public static void Upsert(Recipe recipe)
    {
        NormalizeRecipe(recipe);
        var existing = _cache.FirstOrDefault(r =>
            string.Equals(r.PartNumber, recipe.PartNumber, StringComparison.OrdinalIgnoreCase));
        if (existing is not null) _cache.Remove(existing);
        _cache.Add(recipe);
        SaveRecipe(recipe);
    }

    private static void NormalizeRecipe(Recipe recipe)
    {
        var connectorGender = (recipe.ConnectorGender ?? string.Empty).Trim();
        recipe.ConnectorGender = string.Equals(connectorGender, "B", StringComparison.OrdinalIgnoreCase)
            || string.Equals(connectorGender, "Male", StringComparison.OrdinalIgnoreCase)
            ? "B"
            : "A";

        if (recipe.Stations is null)
            recipe.Stations = [];
        recipe.Tools ??= [];

        var defaultStations = new Dictionary<string, StationConfig>
        {
            ["1"] = new() { Code = "S01", Name = "Station01", Enable = "FALSE" },
            ["2"] = new() { Code = "S02", Name = "Station02", Enable = "FALSE" },
            ["3"] = new() { Code = "S03", Name = "Station03", Enable = "FALSE" },
            ["4"] = new() { Code = "S04", Name = "Station04", Enable = "FALSE" }
        };

        foreach (var key in defaultStations.Keys)
        {
            if (!recipe.Stations.ContainsKey(key))
            {
                recipe.Stations[key] = defaultStations[key];
            }
            else
            {
                var station = recipe.Stations[key];
                if (string.IsNullOrEmpty(station.Code))
                    station.Code = defaultStations[key].Code;
                if (string.IsNullOrEmpty(station.Name))
                    station.Name = defaultStations[key].Name;
                if (key == "4")
                    station.Type = string.Empty;
            }
        }

        var keysToRemove = recipe.Stations.Keys.Where(k => !defaultStations.ContainsKey(k)).ToList();
        foreach (var key in keysToRemove)
            recipe.Stations.Remove(key);

    }

    private static int GetBGCountFromRecipe(Recipe recipe)
    {
        return recipe.Stations.Count(kvp =>
            kvp.Value.Enable == "TRUE"
            && int.TryParse(kvp.Key, out var station)
            && station >= 1
            && station <= 4);
    }

    public static void Delete(string partNumber)
    {
        _cache.RemoveAll(r =>
            string.Equals(r.PartNumber, partNumber, StringComparison.OrdinalIgnoreCase));

        var fileName = GetFileName(partNumber);
        if (File.Exists(fileName))
            File.Delete(fileName);
    }

    public static void NormalizeAllRecipes()
    {
        EnsureDirectoryExists();
        var files = Directory.GetFiles(_dirPath, "*.json");
        foreach (var file in files)
        {
            var json = File.ReadAllText(file);
            var recipe = JsonSerializer.Deserialize<Recipe>(json, _jsonOptions);
            if (recipe is not null)
            {
                NormalizeRecipe(recipe);
                var normalizedJson = JsonSerializer.Serialize(recipe, _jsonOptions);
                File.WriteAllText(file, normalizedJson);
            }
        }
    }
}
