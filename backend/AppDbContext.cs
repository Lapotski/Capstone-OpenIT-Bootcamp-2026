using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MealPlanner.Models;

namespace MealPlanner.Services;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<WeeklyPlan> WeeklyPlans => Set<WeeklyPlan>();
    public DbSet<MealPlanItem> MealPlanItems => Set<MealPlanItem>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<UserRecipe> UserRecipes => Set<UserRecipe>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connStr = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
                ?? "Host=localhost;Port=5432;Database=mealdb;Username=postgres;Password=ccms";

            optionsBuilder.UseNpgsql(connStr);
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<WeeklyPlan>(e =>
        {
            e.HasOne(w => w.User)
             .WithMany(u => u.WeeklyPlans)
             .HasForeignKey(w => w.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<MealPlanItem>(e =>
        {
            e.HasOne(m => m.WeeklyPlan)
             .WithMany(w => w.MealPlanItems)
             .HasForeignKey(m => m.WeeklyPlanId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(m => m.Recipe)
             .WithMany(r => r.MealPlanItems)
             .HasForeignKey(m => m.RecipeId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<RecipeIngredient>(e =>
        {
            e.HasOne(ri => ri.Recipe)
             .WithMany(r => r.RecipeIngredients)
             .HasForeignKey(ri => ri.RecipeId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(ri => ri.Ingredient)
             .WithMany(i => i.RecipeIngredients)
             .HasForeignKey(ri => ri.IngredientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.Property(ri => ri.Quantity).HasColumnType("numeric(10,3)");
        });

        // ── UserRecipe (composite PK, no surrogate Id needed) ────────────────
        builder.Entity<UserRecipe>(e =>
        {
            e.HasKey(ur => new { ur.UserId, ur.RecipeId });

            e.HasOne(ur => ur.User)
             .WithMany(u => u.UserRecipes)
             .HasForeignKey(ur => ur.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            // Deleting a recipe removes its UserRecipe rows too
            e.HasOne(ur => ur.Recipe)
             .WithMany(r => r.UserRecipes)
             .HasForeignKey(ur => ur.RecipeId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Recipe>(e =>
        {
            e.Property(r => r.CostPerServing).HasColumnType("numeric(10,2)");
        });

        builder.Entity<Ingredient>(e =>
        {
            e.Property(i => i.PricePerUnit).HasColumnType("numeric(10,2)");
        });

        builder.Entity<ApplicationUser>(e =>
        {
            e.Property(u => u.WeeklyBudget).HasColumnType("numeric(10,2)");
        });
    }
}