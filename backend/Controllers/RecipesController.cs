using MealPlanner.DTOs;
using MealPlanner.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MealPlanner.Controllers;

[ApiController]
[Route("api/recipes")]
[Authorize]
public class RecipesController(IRecipeService recipeService) : ControllerBase
{
    // ── Query Methods ──────────────────────────────────────────────────────────

    // GET /api/recipes?search=xxx&category=Ulam&sortByCostAsc=true
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] bool sortByCostAsc = false)
    {
        var userId = GetUserId();
        var result = await recipeService.GetAllAsync(userId, search, category, sortByCostAsc);
        return Ok(result);
    }

    // GET /api/recipes/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetUserId();
        var result = await recipeService.GetByIdAsync(id, userId);
        return result is null ? NotFound() : Ok(result);
    }

    // ── Write Methods ──────────────────────────────────────────────────────────

    // POST /api/recipes — create a new private recipe
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RecipeCreateDto dto)
    {
        try
        {
            var userId = GetUserId();
            var result = await recipeService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // PATCH /api/recipes/{id} — update a recipe (must be owned by user)
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Patch(int id, [FromBody] RecipePatchDto dto)
    {
        try
        {
            var userId = GetUserId();
            var result = await recipeService.PatchAsync(id, dto, userId);
            return result is null ? NotFound() : Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    // DELETE /api/recipes/{id} — delete a recipe (must be owned by user)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userId = GetUserId();
            var deleted = await recipeService.DeleteAsync(id, userId);
            return deleted ? NoContent() : NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    // ── Bookmarking (Save/Unsave) ──────────────────────────────────────────────

    // POST /api/recipes/{id}/save — bookmark a recipe to user's collection
    [HttpPost("{id:int}/save")]
    public async Task<IActionResult> SaveRecipe(int id)
    {
        try
        {
            var userId = GetUserId();
            var result = await recipeService.SaveRecipeAsync(id, userId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // DELETE /api/recipes/{id}/save — remove recipe from user's saved collection
    [HttpDelete("{id:int}/save")]
    public async Task<IActionResult> UnsaveRecipe(int id)
    {
        try
        {
            var userId = GetUserId();
            var removed = await recipeService.UnsaveRecipeAsync(id, userId);
            return removed ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private string GetUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User ID not found in claims.");
        return userId;
    }
}