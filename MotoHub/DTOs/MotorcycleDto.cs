
namespace MotoHub.DTOs
{
    public class MotorcycleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Price { get; set; }
        public int EngineVolume { get; set; }
        public int Mileage { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Image { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
