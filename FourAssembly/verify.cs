using FourAssembly.Services;
using FourAssembly.Models;

class Verify {
    static void Main() {
        // Load recipes from JSON
        RecipeRepository.Load();
        var recipes = RecipeRepository.GetAll();
        System.Console.WriteLine($"✓ Loaded {recipes.Count} recipe(s)");
        
        // Test find
        var recipe = RecipeRepository.FindByPartNumber("PN-001");
        if (recipe != null) {
            System.Console.WriteLine($"✓ Found recipe: {recipe.PartNumber}");
            System.Console.WriteLine($"  - Materials: {recipe.Materials.Count}");
            System.Console.WriteLine($"  - Tools: {recipe.Tools.Count}");
        } else {
            System.Console.WriteLine("✗ Recipe PN-001 not found");
        }
        
        // Test station registry
        var stationName = StationRegistry.Resolve("STN-001");
        System.Console.WriteLine($"✓ StationRegistry.Resolve('STN-001') = {stationName}");
        
        var unknown = StationRegistry.Resolve("UNKNOWN");
        System.Console.WriteLine($"✓ StationRegistry.Resolve('UNKNOWN') = {(unknown == null ? "null" : unknown)}");
    }
}
