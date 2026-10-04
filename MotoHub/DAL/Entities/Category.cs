namespace MotoHub.DAL.Entities
{
    public class Category
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public ICollection<Motorcycle> Motorcycles { get; set; }
            = new List<Motorcycle>();
    }
}