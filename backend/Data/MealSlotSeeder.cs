using MealPlanner.Models;
using MealPlanner.Services;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Data;

public static class MealSlotSeeder
{
    public static async Task SeedMealSlotsAsync(AppDbContext db)
    {
        if (await db.Recipes.AnyAsync(r => r.IsGlobal))
            return;

        var ingredients = await SeedIngredientsAsync(db);
        
        var recipes = GetMealSlotRecipes();
        db.Recipes.AddRange(recipes);
        await db.SaveChangesAsync();

        var recipeIngredients = GetRecipeIngredients(recipes, ingredients);
        db.RecipeIngredients.AddRange(recipeIngredients);
        await db.SaveChangesAsync();
    }

    // ── Ingredients ────────────────────────────────────────────────────────────

    private static async Task<Dictionary<string, Ingredient>> SeedIngredientsAsync(AppDbContext db)
    {
        var ingredients = new List<Ingredient>
        {
            // Grains & Basics
            new Ingredient { Name = "Rice", Unit = "kg", PricePerUnit = 50m },
            new Ingredient { Name = "Flour", Unit = "kg", PricePerUnit = 40m },
            new Ingredient { Name = "Pandesal", Unit = "pcs", PricePerUnit = 5m },

            // Proteins
            new Ingredient { Name = "Chicken Breast", Unit = "kg", PricePerUnit = 180m },
            new Ingredient { Name = "Pork Belly", Unit = "kg", PricePerUnit = 200m },
            new Ingredient { Name = "Pork Ribs", Unit = "kg", PricePerUnit = 180m },
            new Ingredient { Name = "Beef", Unit = "kg", PricePerUnit = 300m },
            new Ingredient { Name = "Longganisa", Unit = "pcs", PricePerUnit = 30m },
            new Ingredient { Name = "Tocino", Unit = "kg", PricePerUnit = 250m },
            new Ingredient { Name = "Pork Offal Mix", Unit = "kg", PricePerUnit = 100m },
            new Ingredient { Name = "Fish Fillet", Unit = "kg", PricePerUnit = 200m },
            new Ingredient { Name = "Tilapia", Unit = "pcs", PricePerUnit = 150m },

            // Eggs & Dairy
            new Ingredient { Name = "Egg", Unit = "pcs", PricePerUnit = 10m },
            new Ingredient { Name = "Duck Egg", Unit = "pcs", PricePerUnit = 15m },
            new Ingredient { Name = "Quail Egg", Unit = "pcs", PricePerUnit = 3m },
            new Ingredient { Name = "Butter", Unit = "kg", PricePerUnit = 400m },
            new Ingredient { Name = "Evaporated Milk", Unit = "can", PricePerUnit = 50m },

            // Vegetables
            new Ingredient { Name = "Onion", Unit = "kg", PricePerUnit = 40m },
            new Ingredient { Name = "Garlic", Unit = "kg", PricePerUnit = 150m },
            new Ingredient { Name = "Ginger", Unit = "kg", PricePerUnit = 80m },
            new Ingredient { Name = "Tomato", Unit = "kg", PricePerUnit = 60m },
            new Ingredient { Name = "Green Papaya", Unit = "pcs", PricePerUnit = 40m },
            new Ingredient { Name = "Bok Choy", Unit = "kg", PricePerUnit = 50m },
            new Ingredient { Name = "Radish (Labanos)", Unit = "kg", PricePerUnit = 30m },
            new Ingredient { Name = "Potato", Unit = "kg", PricePerUnit = 45m },
            new Ingredient { Name = "Cabbage", Unit = "kg", PricePerUnit = 35m },

            // Seasonings & Condiments
            new Ingredient { Name = "Soy Sauce", Unit = "bottle", PricePerUnit = 60m },
            new Ingredient { Name = "Vinegar", Unit = "bottle", PricePerUnit = 50m },
            new Ingredient { Name = "Fish Sauce", Unit = "bottle", PricePerUnit = 70m },
            new Ingredient { Name = "Peanut Butter", Unit = "jar", PricePerUnit = 120m },
            new Ingredient { Name = "Salt", Unit = "kg", PricePerUnit = 25m },
            new Ingredient { Name = "Sugar", Unit = "kg", PricePerUnit = 55m },
            new Ingredient { Name = "Jam", Unit = "jar", PricePerUnit = 80m },

            // Other
            new Ingredient { Name = "Oil", Unit = "liter", PricePerUnit = 100m },
            new Ingredient { Name = "Tamarind Paste", Unit = "pack", PricePerUnit = 40m },
            new Ingredient { Name = "Spring Roll Wrapper", Unit = "pack", PricePerUnit = 60m },
            new Ingredient { Name = "Olives", Unit = "can", PricePerUnit = 100m },
            new Ingredient { Name = "Shaved Ice", Unit = "kg", PricePerUnit = 20m },
            new Ingredient { Name = "Beans (cooked)", Unit = "can", PricePerUnit = 40m },
        };

        db.Ingredients.AddRange(ingredients);
        await db.SaveChangesAsync();

        return ingredients.ToDictionary(i => i.Name, i => i);
    }

    // ── Recipes ────────────────────────────────────────────────────────────────

    private static List<Recipe> GetMealSlotRecipes()
    {
        return new List<Recipe>
        {
            // ── Breakfast (5) ──────────────────────────────────────────
            new Recipe
            {
                Name = "Silog with Longganisa",
                Category = "Breakfast",
                Description = "Crispy fried rice, fried egg, and Filipino sausage",
                Instructions = "1. Fry rice until crispy. 2. Fry egg sunny-side up. 3. Grill longganisa. 4. Serve together.",
                CostPerServing = 150m,
                IsGlobal = true
            },
            new Recipe
            {
                Name = "Tapa with Fried Rice",
                Category = "Breakfast",
                Description = "Cured beef strips served with garlic fried rice and egg",
                Instructions = "1. Pan-fry marinated beef until cooked. 2. Fry rice with garlic. 3. Fry egg. 4. Serve together.",
                CostPerServing = 180m,
                IsGlobal = true
            },
            new Recipe
            {
                Name = "Tosilog",
                Category = "Breakfast",
                Description = "Tocino (sweet cured meat), sinangag (garlic fried rice), and egg",
                Instructions = "1. Pan-fry tocino. 2. Fry rice with garlic. 3. Fry egg. 4. Serve together.",
                CostPerServing = 160m,
                IsGlobal = true
            },
            new Recipe
            {
                Name = "Pandesal with Butter and Jam",
                Category = "Breakfast",
                Description = "Soft Filipino bread roll with butter and jam spread",
                Instructions = "1. Toast pandesal slightly. 2. Spread butter generously. 3. Add jam. 4. Serve warm.",
                CostPerServing = 40m,
                IsGlobal = true
            },
            new Recipe
            {
                Name = "Arroz Caldo",
                Category = "Breakfast",
                Description = "Chicken and ginger rice porridge comfort meal",
                Instructions = "1. Boil chicken in broth. 2. Add rice and ginger. 3. Simmer until creamy. 4. Finish with green onions.",
                CostPerServing = 130m,
                IsGlobal = true
            },

            // ── Lunch (5) ──────────────────────────────────────────────
            new Recipe
            {
                Name = "Chicken Adobo",
                Category = "Lunch",
                Description = "Chicken braised in vinegar, soy sauce, and garlic",
                Instructions = "1. Sauté garlic. 2. Add chicken and brown. 3. Add vinegar and soy sauce. 4. Simmer until tender.",
                CostPerServing = 250m,
                IsGlobal = true
            },
            new Recipe
            {
                Name = "Lumpia Shanghai",
                Category = "Lunch",
                Description = "Crispy spring rolls with pork and vegetable filling",
                Instructions = "1. Prepare filling. 2. Roll in wrappers. 3. Deep fry until golden. 4. Serve with sweet and spicy sauce.",
                CostPerServing = 190m,
                IsGlobal = true
            },
            new Recipe
            {
                Name = "Sinigang",
                Category = "Lunch",
                Description = "Tamarind-based stew with pork and vegetables",
                Instructions = "1. Boil pork until tender. 2. Add tamarind broth. 3. Add vegetables. 4. Simmer and season to taste.",
                CostPerServing = 220m,
                IsGlobal = true
            },
            new Recipe
            {
                Name = "Kare-Kare",
                Category = "Lunch",
                Description = "Peanut-based stew with beef and vegetables",
                Instructions = "1. Sauté beef. 2. Prepare peanut sauce. 3. Combine beef with vegetables and sauce. 4. Simmer until creamy.",
                CostPerServing = 300m,
                IsGlobal = true
            },
            new Recipe
            {
                Name = "Tinola",
                Category = "Lunch",
                Description = "Ginger chicken soup with papaya and leafy greens",
                Instructions = "1. Boil chicken in broth. 2. Add ginger. 3. Add papaya and vegetables. 4. Simmer and season.",
                CostPerServing = 210m,
                IsGlobal = true
            },

            // ── Snack (5) ──────────────────────────────────────────────
            new Recipe
            {
                Name = "Empanada",
                Category = "Snack",
                Description = "Fried pastry pockets filled with meat and vegetables",
                Instructions = "1. Prepare filling. 2. Fold dough. 3. Deep fry until golden. 4. Serve warm.",
                CostPerServing = 50m,
                IsGlobal = true
            },
            new Recipe
            {
                Name = "Balut",
                Category = "Snack",
                Description = "Boiled duck egg with embryo, served with salt and vinegar",
                Instructions = "1. Boil egg. 2. Crack and eat with salt and vinegar dip. 3. Enjoy with beer.",
                CostPerServing = 35m,
                IsGlobal = true
            },
            new Recipe
            {
                Name = "Kwek-Kwek",
                Category = "Snack",
                Description = "Battered and fried quail eggs coated in orange batter",
                Instructions = "1. Prepare orange batter. 2. Coat boiled eggs. 3. Deep fry. 4. Serve with sweet sauce.",
                CostPerServing = 45m,
                IsGlobal = true
            },
            new Recipe
            {
                Name = "Fish Ball",
                Category = "Snack",
                Description = "Seasoned fish paste formed into balls and deep fried",
                Instructions = "1. Form fish paste into balls. 2. Deep fry until golden. 3. Serve with spicy sauce.",
                CostPerServing = 30m,
                IsGlobal = true
            },
            new Recipe
            {
                Name = "Halo-Halo",
                Category = "Snack",
                Description = "Shaved ice with sweetened beans, fruits, and evaporated milk",
                Instructions = "1. Mix beans, fruits, and toppings in glass. 2. Add shaved ice. 3. Pour evaporated milk. 4. Stir and enjoy.",
                CostPerServing = 80m,
                IsGlobal = true
            },

            // ── Dinner (5) ──────────────────────────────────────────────
            new Recipe
            {
                Name = "Lechon Kawali",
                Category = "Dinner",
                Description = "Crispy braised pork belly served with liver sauce",
                Instructions = "1. Braise pork belly. 2. Cool and deep fry until crispy. 3. Prepare liver sauce. 4. Serve together.",
                CostPerServing = 350m,
                IsGlobal = true
            },
            new Recipe
            {
                Name = "Grilled Fish Estilo",
                Category = "Dinner",
                Description = "Whole fish grilled and topped with tomato-ginger sauce",
                Instructions = "1. Stuff fish with aromatics. 2. Grill over charcoal. 3. Prepare sauce. 4. Pour over grilled fish.",
                CostPerServing = 290m,
                IsGlobal = true
            },
            new Recipe
            {
                Name = "Dinuguan",
                Category = "Dinner",
                Description = "Pork offal stew in rich blood gravy with chilies",
                Instructions = "1. Sauté aromatics. 2. Add meat and blood. 3. Simmer in vinegar broth. 4. Finish with chilies.",
                CostPerServing = 220m,
                IsGlobal = true
            },
            new Recipe
            {
                Name = "Caldereta",
                Category = "Dinner",
                Description = "Tomato-based stew with beef, potatoes, and olives",
                Instructions = "1. Brown beef. 2. Sauté onions and garlic. 3. Add tomatoes and broth. 4. Add potatoes and simmer.",
                CostPerServing = 270m,
                IsGlobal = true
            },
            new Recipe
            {
                Name = "Baked Tilapia",
                Category = "Dinner",
                Description = "Tilapia baked in foil with vegetables and herbs",
                Instructions = "1. Clean and season fish. 2. Add vegetables inside. 3. Wrap in foil and bake. 4. Serve in foil.",
                CostPerServing = 260m,
                IsGlobal = true
            }
        };
    }

    // ── Recipe Ingredients ─────────────────────────────────────────────────────

    private static List<RecipeIngredient> GetRecipeIngredients(
        List<Recipe> recipes,
        Dictionary<string, Ingredient> ingredients)
    {
        var recipeIngredients = new List<RecipeIngredient>();

        // Helper to add ingredients to a recipe
        void AddToRecipe(int recipeId, (string name, decimal qty, string unit)[] items)
        {
            foreach (var (name, qty, unit) in items)
            {
                if (ingredients.TryGetValue(name, out var ing))
                {
                    recipeIngredients.Add(new RecipeIngredient
                    {
                        RecipeId = recipeId,
                        IngredientId = ing.Id,
                        Quantity = qty,
                        Unit = unit
                    });
                }
            }
        }

        // Breakfast recipes
        var silog = recipes.First(r => r.Name == "Silog with Longganisa");
        AddToRecipe(silog.Id, new[] {
            ("Rice", 1m, "cup"),
            ("Egg", 1m, "pcs"),
            ("Longganisa", 2m, "pcs"),
            ("Oil", 0.1m, "liter"),
            ("Garlic", 0.05m, "kg")
        });

        var tapa = recipes.First(r => r.Name == "Tapa with Fried Rice");
        AddToRecipe(tapa.Id, new[] {
            ("Beef", 0.2m, "kg"),
            ("Rice", 1m, "cup"),
            ("Egg", 1m, "pcs"),
            ("Garlic", 0.05m, "kg"),
            ("Soy Sauce", 0.05m, "bottle")
        });

        var tosilog = recipes.First(r => r.Name == "Tosilog");
        AddToRecipe(tosilog.Id, new[] {
            ("Tocino", 0.15m, "kg"),
            ("Rice", 1m, "cup"),
            ("Egg", 1m, "pcs"),
            ("Garlic", 0.05m, "kg")
        });

        var pandesal = recipes.First(r => r.Name == "Pandesal with Butter and Jam");
        AddToRecipe(pandesal.Id, new[] {
            ("Pandesal", 2m, "pcs"),
            ("Butter", 0.05m, "kg"),
            ("Jam", 0.05m, "jar")
        });

        var arrozCaldo = recipes.First(r => r.Name == "Arroz Caldo");
        AddToRecipe(arrozCaldo.Id, new[] {
            ("Chicken Breast", 0.3m, "kg"),
            ("Rice", 1m, "cup"),
            ("Ginger", 0.1m, "kg"),
            ("Garlic", 0.05m, "kg"),
            ("Onion", 0.1m, "kg")
        });

        // Lunch recipes
        var adobo = recipes.First(r => r.Name == "Chicken Adobo");
        AddToRecipe(adobo.Id, new[] {
            ("Chicken Breast", 0.5m, "kg"),
            ("Soy Sauce", 0.1m, "bottle"),
            ("Vinegar", 0.1m, "bottle"),
            ("Garlic", 0.1m, "kg"),
            ("Onion", 0.15m, "kg")
        });

        var lumpia = recipes.First(r => r.Name == "Lumpia Shanghai");
        AddToRecipe(lumpia.Id, new[] {
            ("Pork Belly", 0.3m, "kg"),
            ("Spring Roll Wrapper", 0.1m, "pack"),
            ("Cabbage", 0.2m, "kg"),
            ("Onion", 0.1m, "kg"),
            ("Garlic", 0.05m, "kg")
        });

        var sinigang = recipes.First(r => r.Name == "Sinigang");
        AddToRecipe(sinigang.Id, new[] {
            ("Pork Ribs", 0.5m, "kg"),
            ("Tamarind Paste", 0.2m, "pack"),
            ("Radish (Labanos)", 0.3m, "kg"),
            ("Bok Choy", 0.2m, "kg"),
            ("Onion", 0.1m, "kg")
        });

        var kareKare = recipes.First(r => r.Name == "Kare-Kare");
        AddToRecipe(kareKare.Id, new[] {
            ("Beef", 0.5m, "kg"),
            ("Peanut Butter", 0.15m, "jar"),
            ("Onion", 0.2m, "kg"),
            ("Garlic", 0.1m, "kg"),
            ("Potato", 0.3m, "kg")
        });

        var tinola = recipes.First(r => r.Name == "Tinola");
        AddToRecipe(tinola.Id, new[] {
            ("Chicken Breast", 0.4m, "kg"),
            ("Green Papaya", 1m, "pcs"),
            ("Ginger", 0.1m, "kg"),
            ("Garlic", 0.05m, "kg"),
            ("Bok Choy", 0.2m, "kg")
        });

        // Snack recipes
        var empanada = recipes.First(r => r.Name == "Empanada");
        AddToRecipe(empanada.Id, new[] {
            ("Flour", 0.3m, "kg"),
            ("Pork Belly", 0.2m, "kg"),
            ("Onion", 0.1m, "kg"),
            ("Potato", 0.2m, "kg")
        });

        var balut = recipes.First(r => r.Name == "Balut");
        AddToRecipe(balut.Id, new[] {
            ("Duck Egg", 1m, "pcs"),
            ("Salt", 0.05m, "kg"),
            ("Vinegar", 0.05m, "bottle")
        });

        var kwekKwek = recipes.First(r => r.Name == "Kwek-Kwek");
        AddToRecipe(kwekKwek.Id, new[] {
            ("Quail Egg", 8m, "pcs"),
            ("Flour", 0.1m, "kg"),
            ("Salt", 0.02m, "kg")
        });

        var fishBall = recipes.First(r => r.Name == "Fish Ball");
        AddToRecipe(fishBall.Id, new[] {
            ("Fish Fillet", 0.3m, "kg"),
            ("Flour", 0.1m, "kg"),
            ("Onion", 0.05m, "kg"),
            ("Garlic", 0.03m, "kg")
        });

        var haloHalo = recipes.First(r => r.Name == "Halo-Halo");
        AddToRecipe(haloHalo.Id, new[] {
            ("Shaved Ice", 0.3m, "kg"),
            ("Beans (cooked)", 0.1m, "can"),
            ("Evaporated Milk", 0.2m, "can"),
            ("Sugar", 0.05m, "kg")
        });

        // Dinner recipes
        var lechon = recipes.First(r => r.Name == "Lechon Kawali");
        AddToRecipe(lechon.Id, new[] {
            ("Pork Belly", 0.8m, "kg"),
            ("Garlic", 0.1m, "kg"),
            ("Onion", 0.1m, "kg"),
            ("Soy Sauce", 0.05m, "bottle"),
            ("Vinegar", 0.05m, "bottle")
        });

        var grilledFish = recipes.First(r => r.Name == "Grilled Fish Estilo");
        AddToRecipe(grilledFish.Id, new[] {
            ("Tilapia", 1m, "pcs"),
            ("Tomato", 0.3m, "kg"),
            ("Ginger", 0.1m, "kg"),
            ("Garlic", 0.1m, "kg"),
            ("Onion", 0.1m, "kg")
        });

        var dinuguan = recipes.First(r => r.Name == "Dinuguan");
        AddToRecipe(dinuguan.Id, new[] {
            ("Pork Offal Mix", 0.4m, "kg"),
            ("Vinegar", 0.1m, "bottle"),
            ("Garlic", 0.1m, "kg"),
            ("Onion", 0.15m, "kg"),
            ("Fish Sauce", 0.05m, "bottle")
        });

        var caldereta = recipes.First(r => r.Name == "Caldereta");
        AddToRecipe(caldereta.Id, new[] {
            ("Beef", 0.6m, "kg"),
            ("Tomato", 0.3m, "kg"),
            ("Potato", 0.4m, "kg"),
            ("Garlic", 0.1m, "kg"),
            ("Onion", 0.2m, "kg"),
            ("Olives", 0.05m, "can")
        });

        var bakedTilapia = recipes.First(r => r.Name == "Baked Tilapia");
        AddToRecipe(bakedTilapia.Id, new[] {
            ("Tilapia", 1m, "pcs"),
            ("Tomato", 0.2m, "kg"),
            ("Ginger", 0.05m, "kg"),
            ("Garlic", 0.05m, "kg"),
            ("Butter", 0.05m, "kg")
        });

        return recipeIngredients;
    }
}
