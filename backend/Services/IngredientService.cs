using MealPlanner.DTOs;
using MealPlanner.Models;
using MealPlanner.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Services;

public class IngredientService(AppDbContext db) : IIngredientService
{
    public async Task<List<IngredientResponseDto>> GetAllAsync(string? search = null)
    {
        var query = db.Ingredients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(i => i.Name.ToLower().Contains(search.ToLower()));

        return await query
            .OrderBy(i => i.Name)
            .Select(i => ToDto(i))
            .ToListAsync();
    }

    public async Task<IngredientResponseDto?> GetByIdAsync(int id)
    {
        var ingredient = await db.Ingredients.FindAsync(id);
        return ingredient is null ? null : ToDto(ingredient);
    }

    public async Task<IngredientResponseDto> CreateAsync(IngredientCreateDto dto)
    {
        var ingredient = new Ingredient
        {
            Name = dto.Name.Trim(),
            Unit = dto.Unit.Trim(),
            PricePerUnit = dto.PricePerUnit
        };

        db.Ingredients.Add(ingredient);
        await db.SaveChangesAsync();

        return ToDto(ingredient);
    }

    public async Task<IngredientResponseDto?> PatchAsync(int id, IngredientPatchDto dto)
    {
        var ingredient = await db.Ingredients.FindAsync(id);
        if (ingredient is null) return null;

        if (dto.Name is not null) ingredient.Name = dto.Name.Trim();
        if (dto.Unit is not null) ingredient.Unit = dto.Unit.Trim();
        if (dto.PricePerUnit.HasValue) ingredient.PricePerUnit = dto.PricePerUnit.Value;

        await db.SaveChangesAsync();
        return ToDto(ingredient);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var ingredient = await db.Ingredients.FindAsync(id);
        if (ingredient is null) return false;

        bool isUsed = await db.RecipeIngredients.AnyAsync(ri => ri.IngredientId == id);
        if (isUsed)
            throw new InvalidOperationException(
                $"Ingredient '{ingredient.Name}' is used in one or more recipes and cannot be deleted.");

        db.Ingredients.Remove(ingredient);
        await db.SaveChangesAsync();
        return true;
    }

    // ── Mapping ──────────────────────────────────────────────────────────────

    private static IngredientResponseDto ToDto(Ingredient i) => new()
    {
        Id = i.Id,
        Name = i.Name,
        Unit = i.Unit,
        PricePerUnit = i.PricePerUnit
    };
}