
namespace MotoHub.DTOs
{
    public class CreateReviewDto
    {
        public int MotorcycleId { get; set; }
        public int Rating { get; set; }
        public required string Comment { get; set; }
    }
}
