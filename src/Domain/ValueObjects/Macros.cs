namespace Domain.ValueObjects
{
    public sealed record Macros(decimal Calories, decimal Protein, decimal Carbs, decimal Fat, decimal? Fiber = null, decimal? Sugar = null, decimal? SodiumMg = null)
    {
        public static Macros operator *(Macros m, decimal qty) =>
            new(m.Calories * qty, m.Protein * qty, m.Carbs * qty, m.Fat * qty,
                m.Fiber is null ? null : m.Fiber * qty,
                m.Sugar is null ? null : m.Sugar * qty,
                m.SodiumMg is null ? null : m.SodiumMg * qty);
    }
}
