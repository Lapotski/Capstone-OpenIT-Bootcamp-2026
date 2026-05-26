namespace MealPlanner.DTOs;

// GET response — what the server returns
public class MealPlanItemResponseDto
{
    public int Id { get; set; }
    public int DayOfWeek { get; set; }       // 0 = Monday, 6 = Sunday
    public string MealSlot { get; set; } = string.Empty;
    public int Servings { get; set; }
    public decimal TotalCost { get; set; }
    public RecipeResponseDto Recipe { get; set; } = null!;
}

// POST /api/weekly-plans/{planId}/items — what the client sends to add a meal to a plan
public class MealPlanItemCreateDto
{
    public int DayOfWeek { get; set; }
    public string MealSlot { get; set; } = string.Empty;
    public int RecipeId { get; set; }
    public int Servings { get; set; } = 1;
}

// PATCH /api/weekly-plans/{planId}/items/{itemId}
public class MealPlanItemPatchDto
{
    public string? MealSlot { get; set; }
    public int? RecipeId { get; set; }
    public int? Servings { get; set; }
}