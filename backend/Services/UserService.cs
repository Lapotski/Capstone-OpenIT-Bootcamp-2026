using MealPlanner.DTOs;
using MealPlanner.Models;
using MealPlanner.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
 
namespace MealPlanner.Services;
 
public class UserService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager
) : IUserService
{
    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var user = new ApplicationUser
        {
            UserName     = dto.Email,
            Email        = dto.Email,
            DisplayName  = dto.DisplayName.Trim(),
            WeeklyBudget = dto.WeeklyBudget
        };
 
        var result = await userManager.CreateAsync(user, dto.Password);
 
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }
 
        return ToRegisterDto(user);
    }
 
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");
 
        var result = await signInManager.PasswordSignInAsync(
            user,
            dto.Password,
            isPersistent: true,
            lockoutOnFailure: false
        );
 
        if (!result.Succeeded)
            throw new UnauthorizedAccessException("Invalid email or password.");
 
        return ToLoginDto(user);
    }
 
    public async Task LogoutAsync()
    {
        await signInManager.SignOutAsync();
    }
 
    public async Task<RegisterResponseDto?> GetProfileAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user is null ? null : ToRegisterDto(user);
    }
 
    public async Task<RegisterResponseDto?> PatchProfileAsync(string userId, UserPatchDto dto)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return null;
 
        if (dto.DisplayName is not null) user.DisplayName = dto.DisplayName.Trim();
        if (dto.WeeklyBudget.HasValue)   user.WeeklyBudget = dto.WeeklyBudget.Value;
 
        await userManager.UpdateAsync(user);
        return ToRegisterDto(user);
    }
 
    // ── Mapping ───────────────────────────────────────────────────────────────
 
    private static RegisterResponseDto ToRegisterDto(ApplicationUser u) => new()
    {
        Id           = u.Id,
        DisplayName  = u.DisplayName,
        Email        = u.Email ?? string.Empty,
        WeeklyBudget = u.WeeklyBudget
    };
 
    private static LoginResponseDto ToLoginDto(ApplicationUser u) => new()
    {
        Id           = u.Id,
        DisplayName  = u.DisplayName,
        Email        = u.Email ?? string.Empty,
        WeeklyBudget = u.WeeklyBudget
    };
}