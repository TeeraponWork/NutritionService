using Application.Abstractions;
using Application.Contracts;
using Domain.Entities;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Application.Foods
{
    public sealed record CreateFoodCommand(string Name, string? Brand, decimal ServingSize, string ServingUnit,
    decimal Calories, decimal Protein, decimal Carbs, decimal Fat, decimal? Fiber, decimal? Sugar, decimal? SodiumMg) : IRequest<FoodDto>;

    public sealed class CreateFoodValidator : AbstractValidator<CreateFoodCommand>
    {
        public CreateFoodValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.ServingSize).GreaterThan(0);
            RuleFor(x => x.Calories).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Protein).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Carbs).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Fat).GreaterThanOrEqualTo(0);
        }
    }

    public sealed class CreateFoodHandler : IRequestHandler<CreateFoodCommand, FoodDto>
    {
        private readonly INutritionDbContext _db;
        private readonly IUserContext _user;

        public CreateFoodHandler(INutritionDbContext db, IUserContext user) { _db = db; _user = user; }

        public async Task<FoodDto> Handle(CreateFoodCommand r, CancellationToken ct)
        {
            var unit = Enum.Parse<ServingUnit>(r.ServingUnit, ignoreCase: true);
            var food = new Food(
                ownerUserId: _user.UserId,
                name: r.Name,
                serving: new Serving(r.ServingSize, unit),
                macrosPerServing: new Macros(r.Calories, r.Protein, r.Carbs, r.Fat, r.Fiber, r.Sugar, r.SodiumMg),
                brand: r.Brand
            );

            _db.Foods.Add(food);
            await _db.SaveChangesAsync(ct);

            return new FoodDto(food.Id, food.Name, food.Brand, food.Serving.Size, food.Serving.Unit.ToString(),
                food.MacrosPerServing.Calories, food.MacrosPerServing.Protein, food.MacrosPerServing.Carbs, food.MacrosPerServing.Fat,
                food.MacrosPerServing.Fiber, food.MacrosPerServing.Sugar, food.MacrosPerServing.SodiumMg);
        }
    }
}
