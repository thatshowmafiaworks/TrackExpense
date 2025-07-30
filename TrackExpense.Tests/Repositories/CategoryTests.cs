using Microsoft.EntityFrameworkCore;
using TrackExpense.Domain.Entities;
using TrackExpense.Infrastructure.Data;
using TrackExpense.Infrastructure.Repositories;

namespace TrackExpense.Tests.Repositories
{
    public class CategoryTests
    {
        private AppDbContext GetInMemoryContext()
        {
            var opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(opts);
        }

        [Fact]
        public async Task GetById_ReturnsCategory_WhenExists()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var cat = new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Food",
                UserId = Guid.NewGuid().ToString()
            };

            await dbContext.Categories.AddAsync(cat);
            await dbContext.SaveChangesAsync();

            var repo = new CategoryRepository(dbContext);
            // Act
            var result = await repo.Get(cat.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Food", result?.Name);
        }

        [Fact]
        public async Task GetAllCategories()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var cats = new List<Category> {
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Food",
                    UserId = Guid.NewGuid().ToString()
                },
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Drinks",
                    UserId = Guid.NewGuid().ToString()
                },
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Sweets",
                    UserId = Guid.NewGuid().ToString()
                }
            }.OrderBy(x => x.Id).ToList();

            await dbContext.Categories.AddRangeAsync(cats);
            await dbContext.SaveChangesAsync();

            var repo = new CategoryRepository(dbContext);
            // Act
            var results = (await repo.GetAll()).OrderBy(x => x.Id).ToList();
            // Assert
            Assert.NotEmpty(results);
            Assert.True(results.Count() == cats.Count);
            for (int i = 0; i < cats.Count; i++)
            {
                Assert.NotNull(results[i]);
                Assert.Equal(cats[i].Name, results[i].Name);
            }
        }

        [Fact]
        public async Task GetAllCategories_ReturnsEmptyList()
        {
            // Arrange
            var dbContext = GetInMemoryContext();

            var repo = new CategoryRepository(dbContext);
            // Act
            var results = await repo.GetAll();
            // Assert
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        [Fact]
        public async Task GetById_ReturnsNull_WhenNotExists()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var cat = new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Food",
                UserId = Guid.NewGuid().ToString()
            };

            await dbContext.Categories.AddAsync(cat);
            await dbContext.SaveChangesAsync();

            var repo = new CategoryRepository(dbContext);
            // Act
            var result = await repo.Get(Guid.NewGuid().ToString());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddsNewCategory()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var cat = new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Food",
                UserId = Guid.NewGuid().ToString()
            };

            var repo = new CategoryRepository(dbContext);
            // Act

            await repo.Add(cat);
            await repo.Save();

            var result = await repo.Get(cat.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Food", result.Name);
        }

        [Fact]
        public async Task UpdatesCategory()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var oldCat = new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Food",
                UserId = Guid.NewGuid().ToString()
            };

            await dbContext.Categories.AddAsync(oldCat);
            await dbContext.SaveChangesAsync();

            var repo = new CategoryRepository(dbContext);
            // Act
            var cat = await repo.Get(oldCat.Id);
            cat.Name = "Drink";
            cat.Description = "Description";

            repo.Update(cat);
            await repo.Save();

            var newCat = await repo.Get(cat.Id);
            // Assert
            Assert.NotNull(newCat);
            Assert.Equal("Drink", newCat.Name);
            Assert.Equal("Description", newCat.Description);
        }

        [Fact]
        public async Task RemovesExistingCategory()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var cat = new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Food",
                UserId = Guid.NewGuid().ToString()
            };

            await dbContext.Categories.AddAsync(cat);
            await dbContext.SaveChangesAsync();

            var repo = new CategoryRepository(dbContext);
            // Act
            var result = await repo.Get(cat.Id);
            repo.Remove(result);
            await repo.Save();

            var noCat = await repo.Get(cat.Id);
            // Assert
            Assert.Null(noCat);
            Assert.True(dbContext.Categories.Count() == 0);
        }
    }
}
