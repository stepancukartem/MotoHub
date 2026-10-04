namespace MotoHub.DAL.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; } = null!;

        public int MotorcycleId { get; set; }

        public Motorcycle Motorcycle { get; set; } = null!;
    }
}
