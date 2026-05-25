namespace MealPlanner.DTOs;

public class WeeklyPlanResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public List<MealPlanItemResponseDto> MealPlanItems { get; set; } = [];
}

public class WeeklyPlanCreateDto
{
    public string Name { get; set; } = string.Empty;
    public List<int> MealPlanItemIds { get; set; } = [];
}

public class WeeklyPlanPatchDto
{
    public string? Name { get; set; }
    public List<int>? MealPlanItemIds { get; set; }
}
