using MealPlanner.DTOs;

namespace MealPlanner.Services.Interfaces;

public interface IRecipeService
{
    // ── Query ────────────────────────────────────────────────────────────
    /// <summary>
    /// Get all recipes accessible by the user (global recipes + user's own + user's saved).
    /// </summary>
    Task<List<RecipeResponseDto>> GetAllAsync(
        string userId,
        string? search = null,
        string? category = null,
        bool sortByCostAsc = false);

    /// <summary>
    /// Get a single recipe by ID (if accessible to user).
    /// </summary>
    Task<RecipeResponseDto?> GetByIdAsync(int id, string userId);

    // ── Write ────────────────────────────────────────────────────────────
    /// <summary>
    /// Create a new private recipe owned by the user.
    /// Creates a UserRecipe row with IsOwner = true.
    /// </summary>
    Task<RecipeResponseDto> CreateAsync(RecipeCreateDto dto, string userId);

    /// <summary>
    /// Update a recipe (must be owned by the user or global).
    /// </summary>
    Task<RecipeResponseDto?> PatchAsync(int id, RecipePatchDto dto, string userId);

    /// <summary>
    /// Delete a recipe (must be owned by the user; prevents deletion if used in meal plans).
    /// </summary>
    Task<bool> DeleteAsync(int id, string userId);

    // ── Save/Unsave (bookmarking) ──────────────────────────────────────
    /// <summary>
    /// Add a recipe to user's saved collection (global or other user's recipe).
    /// </summary>
    Task<RecipeResponseDto> SaveRecipeAsync(int recipeId, string userId);

    /// <summary>
    /// Remove a recipe from user's saved collection.
    /// </summary>
    Task<bool> UnsaveRecipeAsync(int recipeId, string userId);
}