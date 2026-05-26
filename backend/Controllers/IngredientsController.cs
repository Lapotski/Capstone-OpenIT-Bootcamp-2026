using MealPlanner.DTOs;
using MealPlanner.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.Controllers;

[ApiController]
[Route("api/ingredients")]
[Authorize]
public class IngredientsController(IIngredientService ingredientService) : ControllerBase
{
    // GET /api/ingredients?search=xxx
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        var result = await ingredientService.GetAllAsync(search);
        return Ok(result);
    }

    // GET /api/ingredients/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await ingredientService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    // POST /api/ingredients
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] IngredientCreateDto dto)
    {
        var result = await ingredientService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // PATCH /api/ingredients/{id}
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Patch(int id, [FromBody] IngredientPatchDto dto)
    {
        var result = await ingredientService.PatchAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    // DELETE /api/ingredients/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await ingredientService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}