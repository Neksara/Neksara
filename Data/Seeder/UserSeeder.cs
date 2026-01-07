using Neksara.Models;
using BCrypt.Net;

namespace Neksara.Data.Seeders
{
    public static class UserSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Users.Any()) return;

            context.Users.Add(new User
            {
                UserName = "admin",
                Password = BCrypt.Net.BCrypt.HashPassword("admin123"), // otomatis hash
                Role = "Admin"
            });

            context.SaveChanges();
        }
    }
}
