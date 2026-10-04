namespace MotoHub.DAL.Entities
{
    public class Motorcycle
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public int Year { get; set; }

        public decimal Price { get; set; }

        public int EngineVolume { get; set; }

        public int Mileage { get; set; }

        public required string Description { get; set; }

        public string? Image { get; set; }

        public int BrandId { get; set; }

        public Brand Brand { get; set; } = null!;

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();

        public ICollection<Review> Reviews { get; set; }
            = new List<Review>();
    }
}