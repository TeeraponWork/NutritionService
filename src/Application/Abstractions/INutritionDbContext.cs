using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions
{
    public interface INutritionDbContext
    {
        DbSet<Food> Foods { get; }
        DbSet<NutritionEntry> Entries { get; }
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
