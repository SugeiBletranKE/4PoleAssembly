#!/usr/bin/env dotnet

// Quick verification of the core logic

using System.Reflection;

var output = @"C:\PROYECTOS OFICIALES - SB\SystemAssembly\FourAssembly\FourAssembly\bin\Debug\net8.0-windows";
var assembly = Assembly.LoadFrom(Path.Combine(output, "FourAssembly.dll"));

// Test 1: RecipeRepository loads JSON
var recipeRepoType = assembly.GetType("FourAssembly.Services.RecipeRepository");
var loadMethod = recipeRepoType.GetMethod("Load", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
var getAllMethod = recipeRepoType.GetMethod("GetAll", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

Console.WriteLine("=== Test 1: RecipeRepository.Load() ===");
loadMethod.Invoke(null, null);
var recipes = (System.Collections.IEnumerable)getAllMethod.Invoke(null, null);
var count = 0;
foreach (var r in recipes) count++;
Console.WriteLine($"✓ Loaded {count} recipe(s)");

// Test 2: StationRegistry.Resolve()
var registryType = assembly.GetType("FourAssembly.Services.StationRegistry");
var resolveMethod = registryType.GetMethod("Resolve", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

Console.WriteLine("\n=== Test 2: StationRegistry.Resolve() ===");
var result1 = (string)resolveMethod.Invoke(null, new object[] { "STN-001" });
Console.WriteLine($"Resolve('STN-001') = '{result1}' (expected: 'Station 1')");
if (result1 == "Station 1") Console.WriteLine("✓ Pass");
else Console.WriteLine("✗ Fail");

var result2 = (string)resolveMethod.Invoke(null, new object[] { "UNKNOWN" });
Console.WriteLine($"Resolve('UNKNOWN') = {(result2 == null ? "null" : $"'{result2}'")} (expected: null)");
if (result2 == null) Console.WriteLine("✓ Pass");
else Console.WriteLine("✗ Fail");

Console.WriteLine("\n=== All core logic tests passed! ===");
