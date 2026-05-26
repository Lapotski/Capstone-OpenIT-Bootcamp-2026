using Microsoft.AspNetCore.Identity;
using MealPlanner.Services;
<<<<<<< HEAD
using MealPlanner.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
=======
using MealPlanner.Services.Interfaces;
using MealPlanner.Models;
using MealPlanner.Data;
>>>>>>> da5ce6a0aaa48d478bbaf19a2cf0900ed15d07e0
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddControllers();

<<<<<<< HEAD
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend", policy => {
        policy.WithOrigins(
                "http://127.0.0.1:5500", 
                "http://localhost:5500", 
                "http://localhost:3000" ,
                "http://localhost:5173",
                "http://localhost:8080"
            )
            .AllowAnyHeader()  
=======
// ── Identity ──────────────────────────────────────────────────────────────────
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<AppDbContext>();

// ── App Services ──────────────────────────────────────────────────────────────
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IWeeklyPlanService, WeeklyPlanService>();

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://127.0.0.1:5500",
                "http://localhost:5500",
                "http://localhost:3000",
                "http://localhost:5173",
                "http://localhost:8080"
            )
            .AllowAnyHeader()
>>>>>>> da5ce6a0aaa48d478bbaf19a2cf0900ed15d07e0
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
<<<<<<< HEAD


var app = builder.Build();

app.UseStaticFiles(); 
app.UseCors("AllowFrontend");

app.MapIdentityApi<IdentityUser>();
=======

var app = builder.Build();

app.UseStaticFiles();
app.UseCors("AllowFrontend");

app.MapIdentityApi<ApplicationUser>();
>>>>>>> da5ce6a0aaa48d478bbaf19a2cf0900ed15d07e0

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
<<<<<<< HEAD
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
=======
    
    // Seed meal slot recipes
    await MealSlotSeeder.SeedMealSlotsAsync(db);
}

app.Run();
>>>>>>> da5ce6a0aaa48d478bbaf19a2cf0900ed15d07e0
