namespace FourAssembly.Services;

using System.Text.Json;
using FourAssembly.Models;

public static class RecipeRepository
{
    private static readonly string _filePath =
        Path.Combine(AppContext.BaseDirectory, "recipes.json");

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
        if (!File.Exists(_filePath)) { _cache = []; return; }
        var json = File.ReadAllText(_filePath);
        _cache = JsonSerializer.Deserialize<List<Recipe>>(json, _jsonOptions) ?? [];
    }

    public static void Save()
    {
        var json = JsonSerializer.Serialize(_cache, _jsonOptions);
        File.WriteAllText(_filePath, json);
    }

    public static void Upsert(Recipe recipe)
    {
        var existing = _cache.FirstOrDefault(r =>
            string.Equals(r.PartNumber, recipe.PartNumber, StringComparison.OrdinalIgnoreCase));
        if (existing is not null) _cache.Remove(existing);
        _cache.Add(recipe);
        Save();
    }

    public static void Delete(string partNumber)
    {
        _cache.RemoveAll(r =>
            string.Equals(r.PartNumber, partNumber, StringComparison.OrdinalIgnoreCase));
        Save();
    }
}
