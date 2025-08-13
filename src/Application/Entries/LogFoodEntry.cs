using Application.Abstractions;
using Application.Contracts;
using Domain.Entities;
using Domain.Events;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Entries
{
    public sealed record LogFoodEntryCommand(DateOnly Date, string MealType, Guid FoodId, decimal Quantity, string? Notes) : IRequest<NutritionEntryDto>;

    public sealed class LogFoodEntryValidator : AbstractValidator<LogFoodEntryCommand>
    {
        public LogFoodEntryValidator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0);
            RuleFor(x => x.MealType).NotEmpty();
        }
    }

    public sealed class LogFoodEntryHandler : IRequestHandler<LogFoodEntryCommand, NutritionEntryDto>
    {
        private readonly INutritionDbContext _db;
        private readonly IUserContext _user;
        private readonly IEventPublisher _publisher;

        public LogFoodEntryHandler(INutritionDbContext db, IUserContext user, IEventPublisher publisher)
        { _db = db; _user = user; _publisher = publisher; }

        public async Task<NutritionEntryDto> Handle(LogFoodEntryCommand r, CancellationToken ct)
        {
            var meal = Enum.Parse<MealType>(r.MealType, ignoreCase: true);
            var food = await _db.Foods.AsNoTracking().FirstOrDefaultAsync(f => f.Id == r.FoodId, ct)
                       ?? throw new InvalidOperationException("Food not found");
            var entry = NutritionEntry.FromFood(_user.UserId, r.Date, meal, r.FoodId, r.Quantity, r.Notes);

            _db.Entries.Add(entry);
            await _db.SaveChangesAsync(ct);

            // publish integration event (to Redis via Infrastructure)
            await _publisher.PublishAsync(new NutritionEntryLogged(entry.Id, entry.UserId, entry.Date, entry.MealType.ToString()), ct);

            var totals = food.MacrosPerServing * r.Quantity;
            return new NutritionEntryDto(entry.Id, entry.MealType.ToString(), entry.Date,
                entry.FoodId, entry.Quantity,
                totals.Calories, totals.Protein, totals.Carbs, totals.Fat, totals.Fiber, totals.Sugar, totals.SodiumMg,
                entry.Notes);
        }
    }
}
