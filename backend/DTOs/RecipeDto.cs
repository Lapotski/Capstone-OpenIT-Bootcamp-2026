namespace MealPlanner.DTOs;

// GET response — what the server returns
public class RecipeResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public decimal CostPerServing { get; set; }   // computed server-side from ingredients
    public List<RecipeIngredientResponseDto> Ingredients { get; set; } = [];
    
    // Ownership and availability info
    public bool IsGlobal { get; set; }    // seeded recipes are global (no owner)
    public bool IsSaved { get; set; }     // user has bookmarked this recipe
    public bool IsOwner { get; set; }     // current user created this recipe
}

// POST /api/recipes — what the client sends to create a recipe
// CostPerServing is NOT accepted from client — always computed from ingredient prices * quantities
public class RecipeCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public List<RecipeIngredientCreateDto> Ingredients { get; set; } = [];
}

// PATCH /api/recipes/{id} — all fields optional
// Sending Ingredients replaces the full ingredient list for that recipe
public class RecipePatchDto
{
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public List<RecipeIngredientCreateDto>? Ingredients { get; set; }  // full replace if provided
}