namespace MotoHub.DAL.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; } = "New";

        public decimal TotalPrice { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public ICollection<OrderItem> Items { get; set; }
            = new List<OrderItem>();
    }
}