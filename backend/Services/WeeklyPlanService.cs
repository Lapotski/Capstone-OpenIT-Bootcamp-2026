using MealPlanner.DTOs;
using MealPlanner.Models;
using MealPlanner.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Services;

public class WeeklyPlanService(AppDbContext db) : IWeeklyPlanService
{
    public async Task<List<WeeklyPlanResponseDto>> GetAllByUserAsync(string userId)
    {
        var plans = await PlansForUser(userId)
            .OrderByDescending(p => p.WeekStart)
            .ToListAsync();

        return plans.Select(ToDto).ToList();
    }

    public async Task<WeeklyPlanResponseDto?> GetByIdAsync(int id, string userId)
    {
        var plan = await PlansForUser(userId)
            .FirstOrDefaultAsync(p => p.Id == id);

        return plan is null ? null : ToDto(plan);
    }

    public async Task<WeeklyPlanResponseDto> CreateAsync(WeeklyPlanCreateDto dto, string userId)
    {
        var plan = new WeeklyPlan
        {
            UserId = userId,
            Name = dto.Name.Trim(),
            WeekStart = dto.WeekStart,
            IsActive = true
        };

        db.WeeklyPlans.Add(plan);
        await db.SaveChangesAsync();

        plan.MealPlanItems = [];
        return ToDto(plan);
    }

    public async Task<WeeklyPlanResponseDto?> PatchAsync(int id, WeeklyPlanPatchDto dto, string userId)
    {
        var plan = await PlansForUser(userId)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plan is null) return null;

        if (dto.Name is not null) plan.Name = dto.Name.Trim();
        if (dto.IsActive.HasValue) plan.IsActive = dto.IsActive.Value;

        await db.SaveChangesAsync();
        return ToDto(plan);
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var plan = await db.WeeklyPlans
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (plan is null) return false;

        db.WeeklyPlans.Remove(plan);
        await db.SaveChangesAsync();
        return true;
    }

    // ── Meal item management ─────────────────────────────────────────────────

    public async Task<MealPlanItemResponseDto?> AddItemAsync(
        int planId, MealPlanItemCreateDto dto, string userId)
    {
        var planExists = await db.WeeklyPlans
            .AnyAsync(p => p.Id == planId && p.UserId == userId);

        if (!planExists) return null;

        var recipe = await db.Recipes.FindAsync(dto.RecipeId)
            ?? throw new KeyNotFoundException($"Recipe {dto.RecipeId} not found.");

        var item = new MealPlanItem
        {
            WeeklyPlanId = planId,
            RecipeId = dto.RecipeId,
            DayOfWeek = dto.DayOfWeek,
            MealSlot = dto.MealSlot.Trim(),
            Servings = dto.Servings
        };

        db.MealPlanItems.Add(item);
        await db.SaveChangesAsync();

        item.Recipe = recipe;
        return ToItemDto(item);
    }

    public async Task<MealPlanItemResponseDto?> PatchItemAsync(
        int planId, int itemId, MealPlanItemPatchDto dto, string userId)
    {
        var item = await db.MealPlanItems
            .Include(m => m.Recipe)
                .ThenInclude(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
            .Where(m => m.Id == itemId && m.WeeklyPlanId == planId)
            .Where(m => m.WeeklyPlan.UserId == userId)
            .FirstOrDefaultAsync();

        if (item is null) return null;

        if (dto.MealSlot is not null) item.MealSlot = dto.MealSlot.Trim();
        if (dto.Servings.HasValue) item.Servings  = dto.Servings.Value;

        if (dto.RecipeId.HasValue)
        {
            var recipe = await db.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == dto.RecipeId.Value)
                    ?? throw new KeyNotFoundException($"Recipe {dto.RecipeId.Value} not found.");

            item.RecipeId = recipe.Id;
            item.Recipe = recipe;
        }

        await db.SaveChangesAsync();
        return ToItemDto(item);
    }

    public async Task<bool> RemoveItemAsync(int planId, int itemId, string userId)
    {
        var item = await db.MealPlanItems
            .Where(m => m.Id == itemId && m.WeeklyPlanId == planId)
            .Where(m => m.WeeklyPlan.UserId == userId)
            .FirstOrDefaultAsync();

        if (item is null) return false;

        db.MealPlanItems.Remove(item);
        await db.SaveChangesAsync();
        return true;
    }

    // ── Query helpers ────────────────────────────────────────────────────────

    private IQueryable<WeeklyPlan> PlansForUser(string userId) =>
        db.WeeklyPlans
            .Include(p => p.MealPlanItems)
                .ThenInclude(m => m.Recipe)
                    .ThenInclude(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
            .Where(p => p.UserId == userId);

    // ── Mapping ──────────────────────────────────────────────────────────────

    private static WeeklyPlanResponseDto ToDto(WeeklyPlan p)
    {
        var items = p.MealPlanItems.Select(ToItemDto).ToList();

        return new WeeklyPlanResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            WeekStart = p.WeekStart,
            IsActive = p.IsActive,
            TotalWeeklyCost = items.Sum(i => i.TotalCost),
            MealPlanItems = items
        };
    }

    private static MealPlanItemResponseDto ToItemDto(MealPlanItem m) => new()
    {
        Id        = m.Id,
        DayOfWeek = m.DayOfWeek,
        MealSlot  = m.MealSlot,
        Servings  = m.Servings,
        TotalCost = m.Recipe.CostPerServing * m.Servings,
        Recipe    = new RecipeResponseDto
        {
            Id = m.Recipe.Id,
            Name = m.Recipe.Name,
            Category = m.Recipe.Category,
            Description = m.Recipe.Description,
            Instructions = m.Recipe.Instructions,
            CostPerServing = m.Recipe.CostPerServing,
            Ingredients = m.Recipe.RecipeIngredients.Select(ri => new RecipeIngredientResponseDto
            {
                IngredientId = ri.IngredientId,
                Name = ri.Ingredient.Name,
                Quantity = ri.Quantity,
                Unit = ri.Unit,
                PricePerUnit = ri.Ingredient.PricePerUnit,
                LineTotal = ri.Quantity * ri.Ingredient.PricePerUnit
            }).ToList()
        }
    };
}