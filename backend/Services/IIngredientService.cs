using MealPlanner.DTOs;

namespace MealPlanner.Services.Interfaces;

public interface IIngredientService
{
    Task<List<IngredientResponseDto>> GetAllAsync(string? search = null);
    Task<IngredientResponseDto?> GetByIdAsync(int id);
    Task<IngredientResponseDto> CreateAsync(IngredientCreateDto dto);
    Task<IngredientResponseDto?> PatchAsync(int id, IngredientPatchDto dto);
    Task<bool> DeleteAsync(int id);
}