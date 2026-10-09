
namespace MotoHub.DTOs
{
    public class UpdateMotorcycleDto
    {
        public required string Name { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public int EngineVolume { get; set; }
        public int Mileage { get; set; }
        public required string Description { get; set; }
        public string? Image { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
    }
}
