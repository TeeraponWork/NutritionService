namespace Application.Contracts
{
    public sealed record FoodDto(Guid Id, string Name, string? Brand, decimal ServingSize, string ServingUnit, decimal Calories, decimal Protein, decimal Carbs, decimal Fat, decimal? Fiber, decimal? Sugar, decimal? SodiumMg);
}
