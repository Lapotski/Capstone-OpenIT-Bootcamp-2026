namespace MealPlanner.Models;

public class UserRecipe
{
    public string UserId { get; set; } = string.Empty;
    public int RecipeId { get; set; }

    public bool IsOwner { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Recipe Recipe { get; set; } = null!;
}