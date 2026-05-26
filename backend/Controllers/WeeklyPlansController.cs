using MealPlanner.DTOs;
using MealPlanner.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MealPlanner.Controllers;

[ApiController]
[Route("api/weekly-plans")]
[Authorize]
public class WeeklyPlansController(IWeeklyPlanService weeklyPlanService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // ── Weekly Plan CRUD ─────────────────────────────────────────────────────

    // GET /api/weekly-plans
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await weeklyPlanService.GetAllByUserAsync(UserId);
        return Ok(result);
    }

    // GET /api/weekly-plans/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await weeklyPlanService.GetByIdAsync(id, UserId);
        return result is null ? NotFound() : Ok(result);
    }

    // POST /api/weekly-plans
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WeeklyPlanCreateDto dto)
    {
        var result = await weeklyPlanService.CreateAsync(dto, UserId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // PATCH /api/weekly-plans/{id}
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Patch(int id, [FromBody] WeeklyPlanPatchDto dto)
    {
        var result = await weeklyPlanService.PatchAsync(id, dto, UserId);
        return result is null ? NotFound() : Ok(result);
    }

    // DELETE /api/weekly-plans/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await weeklyPlanService.DeleteAsync(id, UserId);
        return deleted ? NoContent() : NotFound();
    }

    // ── Meal Items ───────────────────────────────────────────────────────────

    // POST /api/weekly-plans/{planId}/items
    [HttpPost("{planId:int}/items")]
    public async Task<IActionResult> AddItem(int planId, [FromBody] MealPlanItemCreateDto dto)
    {
        try
        {
            var result = await weeklyPlanService.AddItemAsync(planId, dto, UserId);
            return result is null ? NotFound() : CreatedAtAction(nameof(GetById), new { id = planId }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // PATCH /api/weekly-plans/{planId}/items/{itemId}
    [HttpPatch("{planId:int}/items/{itemId:int}")]
    public async Task<IActionResult> PatchItem(int planId, int itemId, [FromBody] MealPlanItemPatchDto dto)
    {
        try
        {
            var result = await weeklyPlanService.PatchItemAsync(planId, itemId, dto, UserId);
            return result is null ? NotFound() : Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE /api/weekly-plans/{planId}/items/{itemId}
    [HttpDelete("{planId:int}/items/{itemId:int}")]
    public async Task<IActionResult> RemoveItem(int planId, int itemId)
    {
        var deleted = await weeklyPlanService.RemoveItemAsync(planId, itemId, UserId);
        return deleted ? NoContent() : NotFound();
    }
}