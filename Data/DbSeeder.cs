using InsuranceManagementSystem.Enums;
using InsuranceManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagementSystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync();

            bool adminExists = await context.Users.AnyAsync(u => u.Role == UserRole.Admin);

            if (adminExists)
                return;

            var adminUser = new User
            {
                FullName = "System Admin",
                Email = "admin@insurance.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                //Password = "Admin@123" : no safety 
                MobileNumber = "9999999999",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }
    }
}