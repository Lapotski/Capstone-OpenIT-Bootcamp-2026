namespace MealPlanner.Models;

public class MealPlanItem
{
    public int Id { get; set; }
    public int WeeklyPlanId { get; set; }
    public int RecipeId { get; set; }

    // 0 = Monday, 6 = Sunday
    public int DayOfWeek { get; set; }

    public string MealSlot { get; set; } = string.Empty;
    public int Servings { get; set; } = 1;

    public WeeklyPlan WeeklyPlan { get; set; } = null!;
    public Recipe Recipe { get; set; } = null!;
}
