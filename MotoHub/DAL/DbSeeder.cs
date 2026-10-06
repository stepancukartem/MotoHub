using Microsoft.EntityFrameworkCore;
using MotoHub.DAL.Entities;

namespace MotoHub.DAL
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Створюємо роль admin
            var adminRole = await context.Roles
                .FirstOrDefaultAsync(r => r.Name == "admin");

            if (adminRole == null)
            {
                adminRole = new Role
                {
                    Name = "admin"
                };

                await context.Roles.AddAsync(adminRole);
            }

            // Створюємо роль user
            var userRole = await context.Roles
                .FirstOrDefaultAsync(r => r.Name == "user");

            if (userRole == null)
            {
                userRole = new Role
                {
                    Name = "user"
                };

                await context.Roles.AddAsync(userRole);
            }

            await context.SaveChangesAsync();

            // Створюємо тестового адміністратора
            var adminEmail = "admin@moto.com";

            var adminUser = await context.Users
                .FirstOrDefaultAsync(u => u.Email == adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    Email = adminEmail,
                    Password = "Admin123!",
                    RoleId = adminRole.Id
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}