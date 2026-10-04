namespace MotoHub.DAL.Entities
{
    public class User
    {
        public int Id { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public int RoleId { get; set; }

        public Role Role { get; set; } = null!;

        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}