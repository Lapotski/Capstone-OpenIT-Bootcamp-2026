using System.ComponentModel.DataAnnotations;
namespace MealPlanner.Models;

public class Ingredient
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;

    // e.g. "kg", "pcs", "tbsp", "ml"
    public string Unit { get; set; } = string.Empty;
    public decimal PricePerUnit { get; set; }

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];
}

public class RecipeIngredient
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;

    public Recipe Recipe { get; set; } = null!;
    public Ingredient Ingredient { get; set; } = null!;
}
