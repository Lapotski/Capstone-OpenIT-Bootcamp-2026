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

// PATCH /api/ingredients/{id} — all fields optional
public class IngredientPatchDto
{
    public string? Name { get; set; }
    public string? Unit { get; set; }
    public decimal? PricePerUnit { get; set; }
}

// Embedded in RecipeResponseDto — shows ingredient detail with quantity used in this recipe
public class RecipeIngredientResponseDto
{
    public int IngredientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;    // per-recipe unit override
    public decimal PricePerUnit { get; set; }
    public decimal LineTotal { get; set; }              // Quantity * PricePerUnit, computed server-side
}

public class RecipeIngredientCreateDto
{
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }   // optional override; falls back to Ingredient.Unit if null
}