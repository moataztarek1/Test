using KidZone.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace KidZone.API.Seeders
{
    public static class UserSeeder
    {
        public static async Task<int> SeedParentUserAsync(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            string email = "parent@example.com";
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    FullName = "Parent User",
                    UserName = "parentuser",
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Parent@123");
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                await userManager.AddToRoleAsync(user, "Parent");
            }

            // تأكد أنك عملت Fetch للمستخدم تاني علشان تتأكد إن الـ Id محفوظ
            user = await userManager.FindByEmailAsync(email);
            return user.Id;
        }
    }
}
