using Microsoft.EntityFrameworkCore;
using TrackExpense.Domain.Entities;
using TrackExpense.Infrastructure.Data;
using TrackExpense.Infrastructure.Repositories;

namespace TrackExpense.Tests.Repositories
{
    public class TransactionTests
    {
        private AppDbContext GetInMemoryContext()
        {
            var opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(opts);
        }

        [Fact]
        public async Task GetById_ReturnsTransaction_WhenExists()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var trans = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                CategoryId = Guid.NewGuid().ToString(),
                AccountId = Guid.NewGuid().ToString()
            };

            await dbContext.Transactions.AddAsync(trans);
            await dbContext.SaveChangesAsync();

            var repo = new TransactionRepository(dbContext);
            // Act
            var result = await repo.Get(trans.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(trans.CategoryId, result?.CategoryId);
        }

        [Fact]
        public async Task GetAllTransactions()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var trans = new List<Transaction> {
                new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                CategoryId = Guid.NewGuid().ToString(),
                AccountId = Guid.NewGuid().ToString()
            },
                new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                CategoryId = Guid.NewGuid().ToString(),
                AccountId = Guid.NewGuid().ToString()
            },
                new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                CategoryId = Guid.NewGuid().ToString(),
                AccountId = Guid.NewGuid().ToString()
            }
            }.OrderBy(x => x.Id).ToList();

            await dbContext.Transactions.AddRangeAsync(trans);
            await dbContext.SaveChangesAsync();

            var repo = new TransactionRepository(dbContext);
            // Act
            var results = (await repo.GetAll()).OrderBy(x => x.Id).ToList();
            // Assert

            Assert.NotEmpty(results);
            Assert.True(results.Count() == trans.Count);
            for (int i = 0; i < trans.Count; i++)
            {
                Assert.NotNull(results[i]);
                Assert.Equal(trans[i].CategoryId, results[i].CategoryId);
            }
        }

        [Fact]
        public async Task GetAllTransactionsForUserId()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var userId = Guid.NewGuid().ToString();
            var trans = new List<Transaction> {
                new Transaction
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    CategoryId = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString()
                },
                new Transaction
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    CategoryId = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString()
                },
                new Transaction
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    CategoryId = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString()
                },
                new Transaction
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = Guid.NewGuid().ToString(),
                    CategoryId = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString()
                }
            }.OrderBy(x => x.Id).ToList();

            await dbContext.Transactions.AddRangeAsync(trans);
            await dbContext.SaveChangesAsync();

            var repo = new TransactionRepository(dbContext);
            // Act
            var results = (await repo.GetAllForUser(userId)).OrderBy(x => x.Id).ToList();
            // Assert
            trans = trans.Where(x => x.UserId == userId).ToList();
            Assert.NotEmpty(results);
            Assert.True(results.Count() == trans.Count);
            for (int i = 0; i < results.Count; i++)
            {
                Assert.NotNull(results[i]);
                Assert.Equal(userId, results[i].UserId);
            }
        }

        [Fact]
        public async Task GetAllTransactionsForAccount()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var accountId = Guid.NewGuid().ToString();
            var trans = new List<Transaction> {
                new Transaction
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = Guid.NewGuid().ToString(),
                    CategoryId = Guid.NewGuid().ToString(),
                    AccountId = accountId,
                },
                new Transaction
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = Guid.NewGuid().ToString(),
                    CategoryId = Guid.NewGuid().ToString(),
                    AccountId = accountId,
                },
                new Transaction
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = Guid.NewGuid().ToString(),
                    CategoryId = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString(),
                },
                new Transaction
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = Guid.NewGuid().ToString(),
                    CategoryId = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString(),
                }
            }.OrderBy(x => x.Id).ToList();

            await dbContext.Transactions.AddRangeAsync(trans);
            await dbContext.SaveChangesAsync();

            var repo = new TransactionRepository(dbContext);
            // Act
            var results = (await repo.GetAllForAccount(accountId)).OrderBy(x => x.Id).ToList();
            // Assert
            trans = trans.Where(x => x.AccountId == accountId).ToList();
            Assert.NotEmpty(results);
            Assert.True(results.Count() == trans.Count);
            for (int i = 0; i < results.Count; i++)
            {
                Assert.NotNull(results[i]);
                Assert.Equal(accountId, results[i].AccountId);
            }
        }

        [Fact]
        public async Task GetAllTransactions_ReturnsEmptyList()
        {
            // Arrange
            var dbContext = GetInMemoryContext();

            var repo = new TransactionRepository(dbContext);
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
            var trans = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                CategoryId = Guid.NewGuid().ToString(),
                AccountId = Guid.NewGuid().ToString()
            };

            await dbContext.Transactions.AddAsync(trans);
            await dbContext.SaveChangesAsync();

            var repo = new TransactionRepository(dbContext);
            // Act
            var result = await repo.Get(Guid.NewGuid().ToString());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddsNewTransaction()
        {
            // Arrange
            var dbContext = GetInMemoryContext();


            var repo = new TransactionRepository(dbContext);
            // Act
            var trans = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                CategoryId = Guid.NewGuid().ToString(),
                AccountId = Guid.NewGuid().ToString()
            };
            await repo.Add(trans);
            await repo.Save();

            var result = await repo.Get(trans.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(trans.CategoryId, result.CategoryId);
        }

        [Fact]
        public async Task UpdatesTransaction()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var oldTrans = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                CategoryId = Guid.NewGuid().ToString(),
                AccountId = Guid.NewGuid().ToString()
            };
            await dbContext.Transactions.AddAsync(oldTrans);
            await dbContext.SaveChangesAsync();

            var repo = new TransactionRepository(dbContext);
            // Act
            var newGuid = Guid.NewGuid().ToString();
            var toUpdate = await repo.Get(oldTrans.Id);
            toUpdate.UserId = newGuid;
            toUpdate.CategoryId = newGuid;

            repo.Update(toUpdate);
            await repo.Save();

            var updated = await repo.Get(oldTrans.Id);
            // Assert
            Assert.NotNull(updated);
            Assert.Equal(newGuid, updated.CategoryId);
            Assert.Equal(newGuid, updated.UserId);
        }

        [Fact]
        public async Task RemovesExistingTransaction()
        {
            // Arrange
            var dbContext = GetInMemoryContext();

            var trans = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                CategoryId = Guid.NewGuid().ToString(),
                AccountId = Guid.NewGuid().ToString()
            };
            await dbContext.Transactions.AddAsync(trans);
            await dbContext.SaveChangesAsync();

            var repo = new TransactionRepository(dbContext);
            // Act
            var result = await repo.Get(trans.Id);
            repo.Remove(result);
            await repo.Save();

            var noCat = await repo.Get(trans.Id);
            // Assert
            Assert.Null(noCat);
            Assert.True(dbContext.Transactions.Count() == 0);
        }
    }
}
