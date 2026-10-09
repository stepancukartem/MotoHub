
namespace MotoHub.DTOs
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int MotorcycleId { get; set; }
        public string MotorcycleName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
