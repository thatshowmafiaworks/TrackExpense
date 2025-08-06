using Microsoft.AspNetCore.Identity;
using TrackExpense.Application.Interfaces;
using TrackExpense.Domain.Entities;

namespace TrackExpense.Api.Services.Seed
{
    public class CategoriesSeeder
    {
        public async static Task SeedCategories(IServiceProvider services)
        {
            var userMgr = services.GetRequiredService<UserManager<IdentityUser>>();
            var catRepo = services.GetRequiredService<ICategoryRepository>();
            var admin = await userMgr.FindByEmailAsync("admin@gmail.com");
            var categories = new List<Category>
            {
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = admin?.Id ?? "",
                    Name = "Health",
                    Description = "Hospitals, medicines etc."
                },
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = admin?.Id ?? "",
                    Name = "Home",
                    Description = "Towels, soaps etc."
                },
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = admin?.Id ?? "",
                    Name = "Rent",
                    Description = "Rent"
                },
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = admin?.Id ?? "",
                    Name = "Hobby",
                    Description = "Hobby"
                },
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = admin?.Id ?? "",
                    Name = "Restaurants",
                    Description = "Cafe, restaurants, street food"
                },
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = admin?.Id ?? "",
                    Name = "Sport",
                    Description = "Training, supplies"
                },
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = admin?.Id ?? "",
                    Name = "Transport",
                    Description = "Bus, taxi, subway"
                }
            };
            var existingCategories = await catRepo.GetAll();
            foreach (var cat in categories)
            {
                var exist = existingCategories.Find(x => x.Name == cat.Name);
                if (exist == null)
                    await catRepo.Add(cat);
            }
            await catRepo.Save();
        }
    }
}
