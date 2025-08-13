namespace Application.Contracts
{
    public sealed record DailySummaryDto(
    DateOnly Date,
    decimal Calories, decimal Protein, decimal Carbs, decimal Fat,
    decimal? Fiber, decimal? Sugar, decimal? SodiumMg
    );
}
