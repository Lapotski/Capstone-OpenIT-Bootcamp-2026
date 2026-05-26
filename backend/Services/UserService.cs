using MealPlanner.DTOs;
using MealPlanner.Models;
using MealPlanner.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace MealPlanner.Services;

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var user = new ApplicationUser
        {
            UserName      = dto.Email,
            Email         = dto.Email,
            DisplayName   = dto.DisplayName.Trim(),
            WeeklyBudget  = dto.WeeklyBudget
        };

        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }

        return ToDto(user);
    }

    public async Task<RegisterResponseDto?> GetProfileAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user is null ? null : ToDto(user);
    }

    public async Task<RegisterResponseDto?> PatchProfileAsync(string userId, UserPatchDto dto)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return null;

        if (dto.DisplayName is not null) user.DisplayName = dto.DisplayName.Trim();
        if (dto.WeeklyBudget.HasValue)   user.WeeklyBudget = dto.WeeklyBudget.Value;

        await userManager.UpdateAsync(user);
        return ToDto(user);
    }

    private static RegisterResponseDto ToDto(ApplicationUser u) => new()
    {
        Id           = u.Id,
        DisplayName  = u.DisplayName,
        Email        = u.Email ?? string.Empty,
        WeeklyBudget = u.WeeklyBudget
    };
}