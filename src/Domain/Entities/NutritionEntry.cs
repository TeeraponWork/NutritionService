using Domain.ValueObjects;

namespace Domain.Entities
{
    public enum MealType { Breakfast = 0, Lunch = 1, Dinner = 2, Snack = 3 }

    public class NutritionEntry
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid UserId { get; private set; }
        public DateOnly Date { get; private set; }
        public MealType MealType { get; private set; }

        // Either FoodId or QuickAddMacros
        public Guid? FoodId { get; private set; }
        public decimal? Quantity { get; private set; } // จำนวนเสิร์ฟ (เทียบตาม Serving ใน Food)
        public Macros? QuickAddMacros { get; private set; }

        public string? Notes { get; private set; }
        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; private set; }

        private NutritionEntry() { }

        public static NutritionEntry FromFood(Guid userId, DateOnly date, MealType meal, Guid foodId, decimal quantity, string? notes = null)
            => new()
            {
                UserId = userId,
                Date = date,
                MealType = meal,
                FoodId = foodId,
                Quantity = quantity,
                QuickAddMacros = null,
                Notes = notes
            };

        public static NutritionEntry QuickAdd(Guid userId, DateOnly date, MealType meal, Macros macros, string? notes = null)
            => new()
            {
                UserId = userId,
                Date = date,
                MealType = meal,
                FoodId = null,
                Quantity = null,
                QuickAddMacros = macros,
                Notes = notes
            };

        public void UpdateNotes(string? notes) { Notes = notes; UpdatedAtUtc = DateTime.UtcNow; }
    }
}
