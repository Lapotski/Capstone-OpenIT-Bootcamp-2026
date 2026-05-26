using Microsoft.AspNetCore.Identity;
using MealPlanner.Services;
using MealPlanner.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddControllers();

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
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();

app.UseStaticFiles(); 
app.UseCors("AllowFrontend");

app.MapIdentityApi<IdentityUser>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
