using Microsoft.EntityFrameworkCore;
using MotoHub.DAL.Entities;

namespace MotoHub.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Motorcycle> Motorcycles { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Service> Services { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Role
            builder.Entity<Role>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(x => x.Name)
                    .IsUnique();
            });

            // User
            builder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Email)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(x => x.Password)
                    .IsRequired();

                entity.HasIndex(x => x.Email)
                    .IsUnique();

                entity.HasOne(x => x.Role)
                    .WithMany(x => x.Users)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Brand
            builder.Entity<Brand>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(x => x.Name)
                    .IsUnique();
            });

            // Category
            builder.Entity<Category>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(x => x.Name)
                    .IsUnique();
            });

            // Motorcycle
            builder.Entity<Motorcycle>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasColumnType("text")
                    .IsRequired();

                entity.Property(x => x.Price)
                    .HasPrecision(12, 2);

                entity.Property(x => x.Image)
                    .HasMaxLength(500);

                entity.HasOne(x => x.Brand)
                    .WithMany(x => x.Motorcycles)
                    .HasForeignKey(x => x.BrandId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Category)
                    .WithMany(x => x.Motorcycles)
                    .HasForeignKey(x => x.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Order
            builder.Entity<Order>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Status)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.TotalPrice)
                    .HasPrecision(12, 2);

                entity.HasOne(x => x.User)
                    .WithMany(x => x.Orders)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // OrderItem
            builder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Price)
                    .HasPrecision(12, 2);

                entity.HasOne(x => x.Order)
                    .WithMany(x => x.Items)
                    .HasForeignKey(x => x.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Motorcycle)
                    .WithMany(x => x.OrderItems)
                    .HasForeignKey(x => x.MotorcycleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Review
            builder.Entity<Review>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Comment)
                    .HasMaxLength(1000)
                    .IsRequired();

                entity.HasOne(x => x.User)
                    .WithMany(x => x.Reviews)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Motorcycle)
                    .WithMany(x => x.Reviews)
                    .HasForeignKey(x => x.MotorcycleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Service
            builder.Entity<Service>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasMaxLength(1000)
                    .IsRequired();

                entity.Property(x => x.Price)
                    .HasPrecision(12, 2);
            });
        }
    }
}