namespace MealPlanner.DTOs;

// GET response — what the server returns
public class RegisterResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set;} = string.Empty;
    public string Email { get; set;} = string.Empty;
    public decimal WeeklyBudget { get; set; }
}

// POST /api/register — what the client sends to register a new user
public class RegisterRequestDto
{
    public string DisplayName { get; set;} = string.Empty;
    public string Email { get; set;} = string.Empty;
    public string Password { get; set;} = string.Empty;
    public decimal WeeklyBudget { get; set; }
}

// GET response — what the server returns
public class LoginResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set;} = string.Empty;
    public string Email { get; set;} = string.Empty;
    public decimal WeeklyBudget { get; set; }
    public string Token { get; set; } = string.Empty;
}

// POST /api/login — what the client sends to log in
public class LoginRequestDto
{
    public string Email { get; set;} = string.Empty;
    public string Password { get; set;} = string.Empty;
}

public class UserPatchDto
{
    public string? DisplayName { get; set;} = null;
    public decimal? WeeklyBudget { get; set; }
}