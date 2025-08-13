using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Food
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid OwnerUserId { get; private set; }           // สำหรับ Custom Food ต่อผู้ใช้ (หรือใช้ null = global)
        public string Name { get; private set; }
        public string? Brand { get; private set; }
        public Serving Serving { get; private set; }
        public Macros MacrosPerServing { get; private set; }
        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

        private Food() { }

        public Food(Guid ownerUserId, string name, Serving serving, Macros macrosPerServing, string? brand = null)
        {
            OwnerUserId = ownerUserId;
            Name = name;
            Serving = serving;
            MacrosPerServing = macrosPerServing;
            Brand = brand;
        }
    }
}
