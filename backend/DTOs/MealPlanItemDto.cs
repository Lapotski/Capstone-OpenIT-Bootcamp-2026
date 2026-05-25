namespace MealPlanner.DTOs;

// GET response — what the server returns
public class MealPlanItemResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<RecipeResponseDto> Recipes { get; set; } = [];
    public decimal TotalCost { get; set; }
}

// POST /api/meal-plan-items — what the client sends to create a meal plan item
public class MealPlanItemCreateDto
{
    public string Name { get; set; } = string.Empty;
    public List<int> RecipeIds { get; set; } = [];
}

// PATCH — all fields optional
public class MealPlanItemPatchDto
{
    public string? Name { get; set; }
    public List<int>? RecipeIds { get; set; }
}
