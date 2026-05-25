namespace MealPlanner.DTOs;

// GET response — what the server returns
public class RecipeResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal CostPerServing { get; set; }
    public decimal TotalCost { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<RecipeIngredientResponseDto> Ingredients { get; set; } = [];
}

// POST /api/recipes — what the client sends to create a recipe
public class RecipeCreateDto
{
    public string Name { get; set; } = string.Empty;
    public decimal CostPerServing { get; set; }
    public decimal TotalCost { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<RecipeIngredientCreateDto> Ingredients { get; set; } = [];
}

// PATCH — all fields optional
public class RecipePatchDto
{
    public string? Name { get; set; }
    public decimal? CostPerServing { get; set; }
    public decimal? TotalCost { get; set; }
    public string? Category { get; set; }
    public string? Instructions { get; set; }
    public string? Description { get; set; }
    public List<RecipeIngredientPatchDto>? Ingredients { get; set; }
}
