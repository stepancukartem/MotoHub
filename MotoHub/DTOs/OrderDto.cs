
namespace MotoHub.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public int UserId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
