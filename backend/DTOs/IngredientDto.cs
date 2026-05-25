namespace MealPlanner.DTOs;

// GET response — what the server returns
public class IngredientResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal PricePerUnit { get; set; }
}

// POST /api/ingredients — what the client sends to create an ingredient
public class IngredientCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal PricePerUnit { get; set; }
}

// PATCH — all fields optional
public class IngredientPatchDto
{
    public string? Name { get; set; }
    public string? Unit { get; set; }
    public decimal? PricePerUnit { get; set; }
}

// GET response — what the server returns
public class RecipeIngredientResponseDto
{
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal PricePerUnit { get; set; }
    public decimal Quantity { get; set; }
}

// POST /api/recipe-ingredients — what the client sends to create a recipe ingredient
public class RecipeIngredientCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal PricePerUnit { get; set; }
    public decimal Quantity { get; set; }
}

// PATCH — all fields optional
public class RecipeIngredientPatchDto
{
    public string? Name { get; set; }
    public string? Unit { get; set; }
    public decimal? PricePerUnit { get; set; }
    public decimal? Quantity { get; set; }
}