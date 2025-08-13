namespace Application.Contracts
{
    public sealed record NutritionEntryDto(
    Guid Id, string MealType, DateOnly Date,
    Guid? FoodId, decimal? Quantity,
    decimal Calories, decimal Protein, decimal Carbs, decimal Fat,
    decimal? Fiber, decimal? Sugar, decimal? SodiumMg,
    string? Notes
    );
}
