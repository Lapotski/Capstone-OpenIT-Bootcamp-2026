using MealPlanner.DTOs;

namespace MealPlanner.Services.Interfaces;

public interface IWeeklyPlanService
{
    Task<List<WeeklyPlanResponseDto>> GetAllByUserAsync(string userId);
    Task<WeeklyPlanResponseDto?> GetByIdAsync(int id, string userId);
    Task<WeeklyPlanResponseDto> CreateAsync(WeeklyPlanCreateDto dto, string userId);
    Task<WeeklyPlanResponseDto?> PatchAsync(int id, WeeklyPlanPatchDto dto, string userId);
    Task<bool> DeleteAsync(int id, string userId);

    // ── Meal item management ─────────────────────────────────────────────────
    Task<MealPlanItemResponseDto?> AddItemAsync(int planId, MealPlanItemCreateDto dto, string userId);
    Task<MealPlanItemResponseDto?> PatchItemAsync(int planId, int itemId, MealPlanItemPatchDto dto, string userId);
    Task<bool> RemoveItemAsync(int planId, int itemId, string userId);
}