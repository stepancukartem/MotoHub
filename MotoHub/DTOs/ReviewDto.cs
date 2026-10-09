
namespace MotoHub.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int MotorcycleId { get; set; }
        public string MotorcycleName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
