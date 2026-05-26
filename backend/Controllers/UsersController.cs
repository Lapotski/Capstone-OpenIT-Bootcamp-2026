using MealPlanner.DTOs;
using MealPlanner.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
 
namespace MealPlanner.Controllers;
 
[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    // POST /api/users/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        try
        {
            var result = await userService.RegisterAsync(dto);
            return CreatedAtAction(nameof(GetProfile), result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
 
    // POST /api/users/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        try
        {
            var result = await userService.LoginAsync(dto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
 
    // POST /api/users/logout
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await userService.LogoutAsync();
        return Ok(new { message = "Logged out successfully." });
    }
 
    // GET /api/users/me
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();
 
        var profile = await userService.GetProfileAsync(userId);
        return profile is null ? NotFound() : Ok(profile);
    }
 
    // PATCH /api/users/me
    [Authorize]
    [HttpPatch("me")]
    public async Task<IActionResult> PatchProfile([FromBody] UserPatchDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();
 
        var result = await userService.PatchProfileAsync(userId, dto);
        return result is null ? NotFound() : Ok(result);
    }
}