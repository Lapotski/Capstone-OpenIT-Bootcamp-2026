namespace MealPlanner.DTOs;

public class WeeklyPlanResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly WeekStart { get; set; }
    public bool IsActive { get; set; }
    public decimal TotalWeeklyCost { get; set; } 
    public List<MealPlanItemResponseDto> MealPlanItems { get; set; } = [];
}

// POST /api/weekly-plans — client just sends name + week start date
// Items are added separately via POST /api/weekly-plans/{planId}/items
public class WeeklyPlanCreateDto
{
    public string Name { get; set; } = string.Empty;
    public DateOnly WeekStart { get; set; }
}

// PATCH /api/weekly-plans/{id}
public class WeeklyPlanPatchDto
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
}