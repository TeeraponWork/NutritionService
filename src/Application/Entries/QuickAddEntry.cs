using Application.Abstractions;
using Application.Contracts;
using Domain.Entities;
using Domain.Events;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Application.Entries
{
    public sealed record QuickAddEntryCommand(DateOnly Date, string MealType,
    decimal Calories, decimal Protein, decimal Carbs, decimal Fat,
    decimal? Fiber, decimal? Sugar, decimal? SodiumMg, string? Notes) : IRequest<NutritionEntryDto>;

    public sealed class QuickAddEntryValidator : AbstractValidator<QuickAddEntryCommand>
    {
        public QuickAddEntryValidator()
        {
            RuleFor(x => x.Calories).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Protein).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Carbs).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Fat).GreaterThanOrEqualTo(0);
        }
    }

    public sealed class QuickAddEntryHandler : IRequestHandler<QuickAddEntryCommand, NutritionEntryDto>
    {
        private readonly INutritionDbContext _db;
        private readonly IUserContext _user;
        private readonly IEventPublisher _publisher;

        public QuickAddEntryHandler(INutritionDbContext db, IUserContext user, IEventPublisher publisher)
        { _db = db; _user = user; _publisher = publisher; }

        public async Task<NutritionEntryDto> Handle(QuickAddEntryCommand r, CancellationToken ct)
        {
            var meal = Enum.Parse<MealType>(r.MealType, ignoreCase: true);
            var macros = new Macros(r.Calories, r.Protein, r.Carbs, r.Fat, r.Fiber, r.Sugar, r.SodiumMg);
            var entry = NutritionEntry.QuickAdd(_user.UserId, r.Date, meal, macros, r.Notes);

            _db.Entries.Add(entry);
            await _db.SaveChangesAsync(ct);
            await _publisher.PublishAsync(new NutritionEntryLogged(entry.Id, entry.UserId, entry.Date, entry.MealType.ToString()), ct);

            return new NutritionEntryDto(entry.Id, entry.MealType.ToString(), entry.Date, null, null,
                macros.Calories, macros.Protein, macros.Carbs, macros.Fat, macros.Fiber, macros.Sugar, macros.SodiumMg,
                entry.Notes);
        }
    }
}
