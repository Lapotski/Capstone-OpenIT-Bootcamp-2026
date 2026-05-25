namespace MealPlanner.Models;

public class WeeklyPlan
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateOnly WeekStart { get; set; }
    public bool IsActive { get; set; } = true;

    public ApplicationUser User { get; set; } = null!;
    public ICollection<MealPlanItem> MealPlanItems { get; set; } = [];
}
