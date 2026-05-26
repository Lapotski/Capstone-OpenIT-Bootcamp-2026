using MealPlanner.DTOs;
using MealPlanner.Models;
using MealPlanner.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Services;

public class RecipeService(AppDbContext db) : IRecipeService
{
    // ── Query Methods ──────────────────────────────────────────────────────────

    public async Task<List<RecipeResponseDto>> GetAllAsync(
        string userId,
        string? search = null,
        string? category = null,
        bool sortByCostAsc = false)
    {
        var query = db.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.UserRecipes)
            .Where(r =>
                r.IsGlobal ||  
                r.UserRecipes.Any(ur => ur.UserId == userId)) 
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(r => r.Name.ToLower().Contains(search.ToLower()));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(r => r.Category.ToLower() == category.ToLower());

        query = sortByCostAsc
            ? query.OrderBy(r => r.CostPerServing)
            : query.OrderBy(r => r.Name);

        var recipes = await query.ToListAsync();
        return recipes.Select(r => ToDto(r, userId)).ToList();
    }

    public async Task<RecipeResponseDto?> GetByIdAsync(int id, string userId)
    {
        var recipe = await db.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.UserRecipes)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe is null)
            return null;

        bool isAccessible = recipe.IsGlobal || 
                        recipe.UserRecipes.Any(ur => ur.UserId == userId);

        return isAccessible ? ToDto(recipe, userId) : null;
    }

    // ── Write Methods ──────────────────────────────────────────────────────────

    public async Task<RecipeResponseDto> CreateAsync(RecipeCreateDto dto, string userId)
    {
        // Load and validate all referenced ingredients up front
        var ingredientIds = dto.Ingredients.Select(i => i.IngredientId).Distinct().ToList();
        var ingredients = await db.Ingredients
            .Where(i => ingredientIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id);

        var missing = ingredientIds.Except(ingredients.Keys).ToList();
        if (missing.Count > 0)
            throw new KeyNotFoundException(
                $"Ingredient(s) not found: {string.Join(", ", missing)}");

        var recipe = new Recipe
        {
            Name = dto.Name.Trim(),
            Category = dto.Category.Trim(),
            Description = dto.Description.Trim(),
            Instructions = dto.Instructions.Trim(),
            IsGlobal = false
        };

        recipe.RecipeIngredients = dto.Ingredients.Select(i => new RecipeIngredient
        {
            IngredientId = i.IngredientId,
            Quantity = i.Quantity,
            Unit = i.Unit ?? ingredients[i.IngredientId].Unit,
            Recipe = recipe
        }).ToList();

        recipe.CostPerServing = ComputeCostPerServing(recipe.RecipeIngredients, ingredients);

        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();

        var userRecipe = new UserRecipe
        {
            UserId = userId,
            RecipeId = recipe.Id,
            IsOwner = true
        };
        db.UserRecipes.Add(userRecipe);
        await db.SaveChangesAsync();

        foreach (var ri in recipe.RecipeIngredients)
            ri.Ingredient = ingredients[ri.IngredientId];
        
        recipe.UserRecipes = [userRecipe];

        return ToDto(recipe, userId);
    }

    public async Task<RecipeResponseDto?> PatchAsync(int id, RecipePatchDto dto, string userId)
    {
        var recipe = await db.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.UserRecipes)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe is null)
            return null;

        bool isOwner = recipe.UserRecipes.Any(ur => ur.UserId == userId && ur.IsOwner);
        if (!isOwner)
            throw new UnauthorizedAccessException(
                $"You do not have permission to edit recipe '{recipe.Name}'.");

        if (dto.Name is not null) recipe.Name = dto.Name.Trim();
        if (dto.Category is not null) recipe.Category = dto.Category.Trim();
        if (dto.Description is not null) recipe.Description = dto.Description.Trim();
        if (dto.Instructions is not null) recipe.Instructions = dto.Instructions.Trim();

        if (dto.Ingredients is not null)
        {
            var ingredientIds = dto.Ingredients.Select(i => i.IngredientId).Distinct().ToList();
            var ingredients = await db.Ingredients
                .Where(i => ingredientIds.Contains(i.Id))
                .ToDictionaryAsync(i => i.Id);

            var missing = ingredientIds.Except(ingredients.Keys).ToList();
            if (missing.Count > 0)
                throw new KeyNotFoundException(
                    $"Ingredient(s) not found: {string.Join(", ", missing)}");

            db.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);

            recipe.RecipeIngredients = dto.Ingredients.Select(i => new RecipeIngredient
            {
                RecipeId = recipe.Id,
                IngredientId = i.IngredientId,
                Quantity = i.Quantity,
                Unit = i.Unit ?? ingredients[i.IngredientId].Unit,
                Ingredient = ingredients[i.IngredientId]
            }).ToList();

            recipe.CostPerServing = ComputeCostPerServing(recipe.RecipeIngredients, ingredients);
        }

        await db.SaveChangesAsync();
        return ToDto(recipe, userId);
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var recipe = await db.Recipes
            .Include(r => r.UserRecipes)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe is null)
            return false;

        bool isOwner = recipe.UserRecipes.Any(ur => ur.UserId == userId && ur.IsOwner);
        if (!isOwner)
            throw new UnauthorizedAccessException(
                $"You do not have permission to delete recipe '{recipe.Name}'.");

        bool isUsedInPlan = await db.MealPlanItems.AnyAsync(m => m.RecipeId == id);
        if (isUsedInPlan)
            throw new InvalidOperationException(
                $"Recipe '{recipe.Name}' is part of an active meal plan and cannot be deleted.");

        db.Recipes.Remove(recipe);
        await db.SaveChangesAsync();
        return true;
    }

    // ── Bookmarking (Save/Unsave) ──────────────────────────────────────────────

    public async Task<RecipeResponseDto> SaveRecipeAsync(int recipeId, string userId)
    {
        var recipe = await db.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.UserRecipes)
            .FirstOrDefaultAsync(r => r.Id == recipeId);

        if (recipe is null)
            throw new KeyNotFoundException($"Recipe with ID {recipeId} not found.");

        var existingLink = recipe.UserRecipes.FirstOrDefault(ur => ur.UserId == userId);

        if (existingLink is null)
        {
            existingLink = new UserRecipe
            {
                UserId = userId,
                RecipeId = recipeId,
                IsOwner = false
            };
            db.UserRecipes.Add(existingLink);
            recipe.UserRecipes.Add(existingLink);
        }

        await db.SaveChangesAsync();
        return ToDto(recipe, userId);
    }

    public async Task<bool> UnsaveRecipeAsync(int recipeId, string userId)
    {
        var userRecipe = await db.UserRecipes
            .FirstOrDefaultAsync(ur => ur.RecipeId == recipeId && ur.UserId == userId);

        if (userRecipe is null)
            return false;

        if (userRecipe.IsOwner)
            return false;

        db.UserRecipes.Remove(userRecipe);
        await db.SaveChangesAsync();
        return true;
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static decimal ComputeCostPerServing(
        IEnumerable<RecipeIngredient> recipeIngredients,
        Dictionary<int, Ingredient> ingredientLookup)
    {
        return recipeIngredients.Sum(ri =>
            ri.Quantity * ingredientLookup[ri.IngredientId].PricePerUnit);
    }

    // ── Mapping ────────────────────────────────────────────────────────────────

    private static RecipeResponseDto ToDto(Recipe r, string userId)
    {
        var userLink = r.UserRecipes.FirstOrDefault(ur => ur.UserId == userId);
        bool isOwner = userLink?.IsOwner ?? false;
        bool isSaved = userLink is not null;

        return new RecipeResponseDto
        {
            Id = r.Id,
            Name = r.Name,
            Category = r.Category,
            Description = r.Description,
            Instructions = r.Instructions,
            CostPerServing = r.CostPerServing,
            IsGlobal = r.IsGlobal,
            IsOwner = isOwner,
            IsSaved = isSaved,
            Ingredients = r.RecipeIngredients.Select(ri => new RecipeIngredientResponseDto
            {
                IngredientId = ri.IngredientId,
                Name = ri.Ingredient.Name,
                Quantity = ri.Quantity,
                Unit = ri.Unit,
                PricePerUnit = ri.Ingredient.PricePerUnit,
                LineTotal = ri.Quantity * ri.Ingredient.PricePerUnit
            }).ToList()
        };
    }
}