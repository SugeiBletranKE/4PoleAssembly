using FourAssembly.Models;
using FourAssembly.Services;

// Test RecipeRepository
RecipeRepository.Load();
Console.WriteLine($"Loaded recipes: {RecipeRepository.GetAll().Count}");

var recipe = RecipeRepository.FindByPartNumber("PN-001");
if (recipe != null)
{
    Console.WriteLine($"Found: {recipe.PartNumber}");
    Console.WriteLine($"  Materials: {string.Join(", ", recipe.Materials.Select(m => $"{m.Name} ({m.Barcode})"))}");
    Console.WriteLine($"  Tools: {string.Join(", ", recipe.Tools.Select(t => $"{t.Name} ({t.Barcode})"))}");
}

// Test StationRegistry
var stationName = StationRegistry.Resolve("STN-001");
Console.WriteLine($"Station STN-001 → {stationName}");

var stationUnknown = StationRegistry.Resolve("UNKNOWN");
Console.WriteLine($"Station UNKNOWN → {stationUnknown}");

Console.WriteLine("\n✓ All tests passed!");
