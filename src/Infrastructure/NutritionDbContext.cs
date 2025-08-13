using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class NutritionDbContext : DbContext, INutritionDbContext
    {
        public NutritionDbContext(DbContextOptions<NutritionDbContext> options) : base(options) { }

        public DbSet<Food> Foods => Set<Food>();
        public DbSet<NutritionEntry> Entries => Set<NutritionEntry>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.HasPostgresExtension("uuid-ossp");

            b.Entity<Food>(e =>
            {
                e.ToTable("foods");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                e.Property(x => x.OwnerUserId).HasColumnName("owner_user_id").IsRequired();
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
                e.Property(x => x.Brand).HasColumnName("brand");

                e.OwnsOne(x => x.Serving, sb =>
                {
                    sb.Property(p => p.Size).HasColumnName("serving_size").HasColumnType("numeric(10,2)");
                    sb.Property(p => p.Unit).HasColumnName("serving_unit");
                });

                e.OwnsOne(x => x.MacrosPerServing, mb =>
                {
                    mb.Property(p => p.Calories).HasColumnName("calories").HasColumnType("numeric(10,2)");
                    mb.Property(p => p.Protein).HasColumnName("protein").HasColumnType("numeric(10,2)");
                    mb.Property(p => p.Carbs).HasColumnName("carbs").HasColumnType("numeric(10,2)");
                    mb.Property(p => p.Fat).HasColumnName("fat").HasColumnType("numeric(10,2)");
                    mb.Property(p => p.Fiber).HasColumnName("fiber").HasColumnType("numeric(10,2)");
                    mb.Property(p => p.Sugar).HasColumnName("sugar").HasColumnType("numeric(10,2)");
                    mb.Property(p => p.SodiumMg).HasColumnName("sodium_mg").HasColumnType("numeric(10,2)");
                });

                e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            });

            b.Entity<NutritionEntry>(e =>
            {
                e.ToTable("nutrition_entries");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                e.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
                e.Property(x => x.Date).HasColumnName("date").HasColumnType("date").IsRequired();
                e.Property(x => x.MealType).HasColumnName("meal_type").IsRequired();
                e.Property(x => x.FoodId).HasColumnName("food_id");
                e.Property(x => x.Quantity).HasColumnName("quantity").HasColumnType("numeric(10,2)");
                e.Property(x => x.Notes).HasColumnName("notes");

                e.OwnsOne(x => x.QuickAddMacros, mb =>
                {
                    mb.Property(p => p.Calories).HasColumnName("qa_calories").HasColumnType("numeric(10,2)");
                    mb.Property(p => p.Protein).HasColumnName("qa_protein").HasColumnType("numeric(10,2)");
                    mb.Property(p => p.Carbs).HasColumnName("qa_carbs").HasColumnType("numeric(10,2)");
                    mb.Property(p => p.Fat).HasColumnName("qa_fat").HasColumnType("numeric(10,2)");
                    mb.Property(p => p.Fiber).HasColumnName("qa_fiber").HasColumnType("numeric(10,2)");
                    mb.Property(p => p.Sugar).HasColumnName("qa_sugar").HasColumnType("numeric(10,2)");
                    mb.Property(p => p.SodiumMg).HasColumnName("qa_sodium_mg").HasColumnType("numeric(10,2)");
                });

                e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
                e.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc");
                e.HasIndex(x => new { x.UserId, x.Date });
            });
        }
    }
}
