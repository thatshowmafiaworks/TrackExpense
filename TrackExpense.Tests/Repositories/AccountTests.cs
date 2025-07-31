using Microsoft.EntityFrameworkCore;
using TrackExpense.Domain.Entities;
using TrackExpense.Infrastructure.Data;
using TrackExpense.Infrastructure.Repositories;

namespace TrackExpense.Tests.Repositories
{
    public class AccountTests
    {
        private AppDbContext GetInMemoryContext()
        {
            var opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(opts);
        }

        [Fact]
        public async Task GetById_ReturnsAccount_WhenExists()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var acc = new Account
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                Name = "Card"
            };

            await dbContext.Accounts.AddAsync(acc);
            await dbContext.SaveChangesAsync();

            var repo = new AccountRepository(dbContext);
            // Act
            var result = await repo.Get(acc.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Card", result?.Name);
        }

        [Fact]
        public async Task GetAllAccounts()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var accs = new List<Account> {
                new Account
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                Name = "Bank"
            },
                new Account
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                Name = "Card"
            },
                new Account
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                Name = "Cash"
            }
            }.OrderBy(x => x.Id).ToList();

            await dbContext.Accounts.AddRangeAsync(accs);
            await dbContext.SaveChangesAsync();

            var repo = new AccountRepository(dbContext);
            // Act
            var results = (await repo.GetAll()).OrderBy(x => x.Id).ToList();
            // Assert

            Assert.NotEmpty(results);
            Assert.True(results.Count == accs.Count);
            for (int i = 0; i < accs.Count; i++)
            {
                Assert.NotNull(results[i]);
                Assert.Equal(accs[i].Name, results[i].Name);
            }
        }

        [Fact]
        public async Task GetAllAccountsForUserId()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var userId = Guid.NewGuid().ToString();
            var accs = new List<Account> {
                new Account
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Name = "Bank"
            },
                new Account
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Name = "Card"
            },
                new Account
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                Name = "Cash"
            }
            }.OrderBy(x => x.Id).ToList();

            await dbContext.Accounts.AddRangeAsync(accs);
            await dbContext.SaveChangesAsync();

            var repo = new AccountRepository(dbContext);
            // Act
            var results = (await repo.GetForUser(userId)).OrderBy(x => x.Id).ToList();
            // Assert
            accs = accs.Where(x => x.UserId == userId).ToList();
            Assert.NotEmpty(results);
            Assert.True(results.Count == accs.Count);
            for (int i = 0; i < results.Count; i++)
            {
                Assert.NotNull(results[i]);
                Assert.Equal(userId, results[i].UserId);
            }
        }

        [Fact]
        public async Task GetAllAccounts_ReturnsEmptyList()
        {
            // Arrange
            var dbContext = GetInMemoryContext();

            var repo = new AccountRepository(dbContext);
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
            var acc = new Account
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                Name = "Card"
            };

            await dbContext.Accounts.AddAsync(acc);
            await dbContext.SaveChangesAsync();

            var repo = new AccountRepository(dbContext);
            // Act
            var result = await repo.Get(Guid.NewGuid().ToString());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddsNewAccount()
        {
            // Arrange
            var dbContext = GetInMemoryContext();


            var repo = new AccountRepository(dbContext);
            // Act
            var acc = new Account
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                Name = "Card"
            };
            await repo.Add(acc);
            await repo.Save();

            var result = await repo.Get(acc.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Card", result.Name);
        }

        [Fact]
        public async Task UpdatesAccount()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var acc = new Account
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                Name = "Card"
            };
            await dbContext.Accounts.AddAsync(acc);
            await dbContext.SaveChangesAsync();

            var repo = new AccountRepository(dbContext);
            // Act
            var newGuid = Guid.NewGuid().ToString();
            var newName = "Bank";
            var toUpdate = await repo.Get(acc.Id);
            toUpdate.UserId = newGuid;
            toUpdate.Name = newName;

            repo.Update(toUpdate);
            await repo.Save();

            var updated = await repo.Get(acc.Id);
            // Assert
            Assert.NotNull(updated);
            Assert.Equal(newGuid, updated.UserId);
            Assert.Equal(newName, updated.Name);
        }

        [Fact]
        public async Task RemovesExistingAccount()
        {
            // Arrange
            var dbContext = GetInMemoryContext();

            var acc = new Account
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                Name = "Card"
            };
            await dbContext.Accounts.AddAsync(acc);
            await dbContext.SaveChangesAsync();

            var repo = new AccountRepository(dbContext);
            // Act
            var result = await repo.Get(acc.Id);
            repo.Remove(result);
            await repo.Save();

            var noCat = await repo.Get(acc.Id);
            // Assert
            Assert.Null(noCat);
            Assert.True(dbContext.Accounts.Count() == 0);
        }
    }
}
