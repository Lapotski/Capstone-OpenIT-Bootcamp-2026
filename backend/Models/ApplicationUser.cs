using Microsoft.AspNetCore.Identity;

namespace MealPlanner.Models;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public decimal WeeklyBudget { get; set; }

    public ICollection<WeeklyPlan> WeeklyPlans { get; set; } = [];
    public ICollection<UserRecipe> UserRecipes { get; set; } = [];
}