namespace MealPlanner.Models;

public class Recipe
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // e.g. "Silog", "Ulam", "Sabaw" — stored as plain text, no lookup table needed
    public string Category { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal CostPerServing { get; set; }

    public ICollection<MealPlanItem> MealPlanItems { get; set; } = [];
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];
}
