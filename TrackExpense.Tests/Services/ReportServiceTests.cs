using Microsoft.EntityFrameworkCore;
using TrackExpense.Application.Interfaces;
using TrackExpense.Domain.Entities;
using TrackExpense.Infrastructure.Data;
using TrackExpense.Infrastructure.Repositories;
using TrackExpense.Infrastructure.Services;

namespace TrackExpense.Tests.Services
{
    public class ReportServiceTests
    {
        private AppDbContext GetInMemoryContext()
        {
            var opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(opts);
        }

        [Fact]
        public async Task ExpensesPerCategory_ByAllAccounts()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var userId = Guid.NewGuid().ToString();
            var cats = new List<Category>
            {
                new Category
                {
                    Id = 1.ToString(),
                    Name = "Food",
                    UserId = userId
                },
                new Category
                {
                    Id = 2.ToString(),
                    Name = "Drink",
                    UserId = userId
                },
                new Category
                {
                    Id = 3.ToString(),
                    Name = "Internet",
                    UserId = userId
                }
            };
            await dbContext.Categories.AddRangeAsync(cats);

            var account = new Account
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Card",
                UserId = userId
            };
            await dbContext.Accounts.AddAsync(account);

            var transactions = new List<Transaction>();
            for (int i = 1; i <= 15; i++)
            {
                var transaction = new Transaction
                {
                    Id = i.ToString(),
                    UserId = userId,
                    Amount = i % 3 + 1,
                    TransactionType = TransactionType.Expense,
                    Date = DateTime.Today.AddDays(-i),
                    CategoryId = cats[i % 3].Id,
                    AccountId = account.Id
                };
                transactions.Add(transaction);
            }
            await dbContext.Transactions.AddRangeAsync(transactions);

            await dbContext.SaveChangesAsync();


            IReportService reportService = new ReportService(
                new CategoryRepository(dbContext),
                new AccountRepository(dbContext),
                new TransactionRepository(dbContext)
                );
            // Act
            var result = await reportService.ExpensesPerCategories(userId);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(cats.Count, result.Count);
            foreach (var categorySum in result)
            {
                Assert.NotNull(categorySum);
                var sum = transactions.Where(x => x.CategoryId == categorySum.CategoryId).Sum(x => x.Amount);
                Assert.Equal(sum, categorySum.Amount);
            }
        }

        [Fact]
        public async Task ExpensesPerCategory_ByOneAccount()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var userId = Guid.NewGuid().ToString();
            var cats = new List<Category>
            {
                new Category
                {
                    Id = 1.ToString(),
                    Name = "Food",
                    UserId = userId
                },
                new Category
                {
                    Id = 2.ToString(),
                    Name = "Drink",
                    UserId = userId
                },
                new Category
                {
                    Id = 3.ToString(),
                    Name = "Internet",
                    UserId = userId
                }
            };
            await dbContext.Categories.AddRangeAsync(cats);

            var accounts = new List<Account>
            {
                new Account
                {
                    Id = 1.ToString(),
                    Name = "Card",
                    UserId = userId
                },
                new Account
                {
                    Id = 2.ToString(),
                    Name = "Cash",
                    UserId = userId
                }
            };
            await dbContext.Accounts.AddRangeAsync(accounts);

            var transactions = new List<Transaction>();
            for (int i = 1; i <= 15; i++)
            {
                var transaction = new Transaction
                {
                    Id = i.ToString(),
                    UserId = userId,
                    Amount = i % 3 + 1,
                    TransactionType = TransactionType.Expense,
                    Date = DateTime.Today.AddDays(-i),
                    CategoryId = cats[i % 3].Id,
                    AccountId = accounts[i % 2].Id
                };
                transactions.Add(transaction);
            }
            await dbContext.Transactions.AddRangeAsync(transactions);

            await dbContext.SaveChangesAsync();


            IReportService reportService = new ReportService(
                new CategoryRepository(dbContext),
                new AccountRepository(dbContext),
                new TransactionRepository(dbContext)
                );
            // Act
            var result = await reportService.ExpensesPerCategories(userId, "1");

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(cats.Count, result.Count);
            foreach (var categorySum in result)
            {
                Assert.NotNull(categorySum);
                var sum = transactions.Where(x => x.AccountId == "1").Where(x => x.CategoryId == categorySum.CategoryId).Sum(x => x.Amount);
                Assert.Equal(sum, categorySum.Amount);
            }
        }

        [Fact]
        public async Task IncomeAndExpenseByMonths()
        {
            // Arrange
            var dbContext = GetInMemoryContext();
            var userId = Guid.NewGuid().ToString();
            var cat = new Category
            {
                Id = 1.ToString(),
                Name = "Food",
                UserId = userId
            };
            await dbContext.Categories.AddAsync(cat);

            var account = new Account
            {
                Id = 1.ToString(),
                Name = "Card",
                UserId = userId
            };
            await dbContext.Accounts.AddAsync(account);

            var transactions = new List<Transaction>();
            for (int i = 1; i <= 12; i++)
            {
                var transaction = new Transaction
                {
                    Id = i.ToString(),
                    UserId = userId,
                    Amount = (i % 12) + 1,
                    TransactionType = TransactionType.Income,
                    Date = DateTime.Today.AddMonths(-i + 1),
                    CategoryId = cat.Id,
                    AccountId = account.Id
                };
                transactions.Add(transaction);
            }
            for (int i = 1; i <= 12; i++)
            {
                var transaction = new Transaction
                {
                    Id = (i + 12).ToString(),
                    UserId = userId,
                    Amount = (i % 12) + 1,
                    TransactionType = TransactionType.Expense,
                    Date = DateTime.Today.AddMonths(-i + 1),
                    CategoryId = cat.Id,
                    AccountId = account.Id
                };
                transactions.Add(transaction);
            }
            await dbContext.Transactions.AddRangeAsync(transactions);

            await dbContext.SaveChangesAsync();


            IReportService reportService = new ReportService(
                new CategoryRepository(dbContext),
                new AccountRepository(dbContext),
                new TransactionRepository(dbContext)
                );
            // Act
            var results = await reportService.IncomeAndExpensesByMonths(userId);

            // Assert
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(12, results.Count);

            foreach (var result in results)
            {
                Assert.NotNull(result);
                var incomeSum = transactions
                    .Where(x => x.TransactionType == TransactionType.Income)
                    .Where(x => InSameMonth(x.Date, result.Date))
                    .Sum(x => x.Amount);
                var expenseSum = transactions
                    .Where(x => x.TransactionType == TransactionType.Expense)
                    .Where(x => InSameMonth(x.Date, result.Date))
                    .Sum(x => x.Amount);
                Assert.Equal(incomeSum, result.Income);
                Assert.Equal(expenseSum, result.Expense);
            }
        }

        private bool InSameMonth(DateTime a, DateOnly b)
        {
            var aFirstDayOfMonth = a.AddDays(-a.Day + 1);
            var bFirstDayOfMonth = b.AddDays(-b.Day + 1);
            return DateOnly.FromDateTime(aFirstDayOfMonth) == bFirstDayOfMonth;
        }
    }
}

