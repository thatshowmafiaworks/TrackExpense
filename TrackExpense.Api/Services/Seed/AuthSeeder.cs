using Microsoft.AspNetCore.Identity;

namespace TrackExpense.Api.Seed
{
    public static class AuthSeeder
    {
        public async static Task SeedAdmin(IServiceProvider services)
        {
            var userMgr = services.GetRequiredService<UserManager<IdentityUser>>();
            var admin = await userMgr.FindByEmailAsync("admin@gmail.com");
            if (admin is null)
            {
                admin = new IdentityUser
                {
                    Email = "admin@gmail.com",
                    UserName = "admin"
                };
                var adminResult = await userMgr.CreateAsync(admin, "Admin_123");
                await userMgr.AddToRoleAsync(admin, "Admin");
                await userMgr.AddToRoleAsync(admin, "User");
            }
            if (admin != null)
            {
                if (!await userMgr.IsInRoleAsync(admin, "User")) await userMgr.AddToRoleAsync(admin, "User");
                if (!await userMgr.IsInRoleAsync(admin, "Admin")) await userMgr.AddToRoleAsync(admin, "Admin");
            }
        }
        public async static Task SeedRoles(IServiceProvider services)
        {
            var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = {
                "User",
                "Admin"
            };
            foreach (var role in roles)
            {
                if (!await roleMgr.RoleExistsAsync(role))
                {
                    var result = await roleMgr.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
