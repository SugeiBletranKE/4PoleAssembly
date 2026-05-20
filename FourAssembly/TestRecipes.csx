#!/usr/bin/env dotnet script

#load "FourAssembly/Models/Material.cs"
#load "FourAssembly/Models/Tool.cs"
#load "FourAssembly/Models/Recipe.cs"
#load "FourAssembly/Services/RecipeRepository.cs"

using FourAssembly.Models;
using FourAssembly.Services;
using System.Text.Json;

Console.WriteLine("=== Testing RecipeRepository ===\n");

RecipeRepository.Load();
Console.WriteLine($"Loaded recipes: {RecipeRepository.GetAll().Count}");

var recipe = new Recipe
{
    PartNumber = "PN-001",
    Materials =
    [
        new Material { Name = "Bracket", Barcode = "BC-MAT-001" },
        new Material { Name = "Washer", Barcode = "BC-MAT-002" }
    ],
    Tools =
    [
        new Tool { Name = "Torque Wrench", Barcode = "BC-TOOL-001" }
    ]
};

RecipeRepository.Upsert(recipe);
Console.WriteLine($"Added recipe: {recipe.PartNumber}");

var found = RecipeRepository.FindByPartNumber("PN-001");
Console.WriteLine($"Found recipe: {found?.PartNumber}");
Console.WriteLine($"Materials: {string.Join(", ", found?.Materials.Select(m => m.Name) ?? [])}");
Console.WriteLine($"Tools: {string.Join(", ", found?.Tools.Select(t => t.Name) ?? [])}");

var jsonPath = Path.Combine(AppContext.BaseDirectory, "recipes.json");
if (File.Exists(jsonPath))
{
    var json = File.ReadAllText(jsonPath);
    Console.WriteLine($"\n=== recipes.json content ===\n{json}");
}
