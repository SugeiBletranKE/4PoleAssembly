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
                _cache.Add(recipe);
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
        var existing = _cache.FirstOrDefault(r =>
            string.Equals(r.PartNumber, recipe.PartNumber, StringComparison.OrdinalIgnoreCase));
        if (existing is not null) _cache.Remove(existing);
        _cache.Add(recipe);
        SaveRecipe(recipe);
    }

    public static void Delete(string partNumber)
    {
        _cache.RemoveAll(r =>
            string.Equals(r.PartNumber, partNumber, StringComparison.OrdinalIgnoreCase));

        var fileName = GetFileName(partNumber);
        if (File.Exists(fileName))
            File.Delete(fileName);
    }
}
