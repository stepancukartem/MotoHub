namespace MotoHub.DAL.Entities
{
    public class Review
    {
        public int Id { get; set; }

        public int Rating { get; set; }

        public required string Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public int MotorcycleId { get; set; }

        public Motorcycle Motorcycle { get; set; } = null!;
    }
}