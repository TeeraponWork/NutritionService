namespace Domain.Events
{
    public sealed record NutritionEntryLogged(Guid EntryId, Guid UserId, DateOnly Date, string Meal);
}
