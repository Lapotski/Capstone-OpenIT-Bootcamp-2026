using MealPlanner.DTOs;

namespace MealPlanner.Services.Interfaces;

public interface IUserService
{
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto);
    Task<RegisterResponseDto?> GetProfileAsync(string userId);
    Task<RegisterResponseDto?> PatchProfileAsync(string userId, UserPatchDto dto);
}