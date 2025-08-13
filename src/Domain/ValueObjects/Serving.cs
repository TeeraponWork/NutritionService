namespace Domain.ValueObjects
{
    public enum ServingUnit { Gram = 0, Milliliter = 1, Piece = 2 }

    public sealed record Serving(decimal Size, ServingUnit Unit);
}
