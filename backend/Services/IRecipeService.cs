using MealPlanner.DTOs;

namespace MealPlanner.Services.Interfaces;

public interface IRecipeService
{
    // ── Query ────────────────────────────────────────────────────────────
    Task<List<RecipeResponseDto>> GetAllAsync(
        string userId,
        string? search = null,
        string? category = null,
        bool sortByCostAsc = false);

    Task<RecipeResponseDto?> GetByIdAsync(int id, string userId);

    // ── Write ────────────────────────────────────────────────────────────
    Task<RecipeResponseDto> CreateAsync(RecipeCreateDto dto, string userId);

    Task<RecipeResponseDto?> PatchAsync(int id, RecipePatchDto dto, string userId);

    Task<bool> DeleteAsync(int id, string userId);

    // ── Save/Unsave (bookmarking) ──────────────────────────────────────
    Task<RecipeResponseDto> SaveRecipeAsync(int recipeId, string userId);

    Task<bool> UnsaveRecipeAsync(int recipeId, string userId);
}