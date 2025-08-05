using System.Globalization;
using TrackExpense.Application.Dtos;
using TrackExpense.Application.Interfaces;
using TrackExpense.Domain.Entities;

namespace TrackExpense.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly ITransactionRepository _transRepo;

        public ReportService(
            ICategoryRepository categoryRepo,
            IAccountRepository accountRepo,
            ITransactionRepository transRepo)
        {
            _categoryRepo = categoryRepo;
            _accountRepo = accountRepo;
            _transRepo = transRepo;
        }

        public async Task<List<ExpensePerCategory>> ExpensesPerCategories(
            string userId,
            string accountId = "",
            int daysCount = 30)
        {
            List<Transaction> expenses;
            var categories = await _categoryRepo.GetAll();
            // filtering for account if specified
            if (string.IsNullOrWhiteSpace(accountId))
            {
                expenses = (await _transRepo.GetAllForUser(userId))
                    .Where(x => x.TransactionType == TransactionType.Expense)
                    .ToList();
            }
            else
            {
                expenses = (await _transRepo.GetAllForAccount(accountId))
                    .Where(x => x.TransactionType == TransactionType.Expense)
                    .ToList();
            }
            // endTime is today+1 day to not filter transactions today
            var endTime = DateTime.Today.AddDays(1);
            // beginTime is today - daysCount+1 to get 30 days all
            var beginTime = DateTime.Today.AddDays(-daysCount + 1);
            // filtering by date, default is 30 days
            expenses = expenses.Where(x => x.Date >= beginTime && x.Date <= endTime).ToList();
            var groupedByCat = expenses.GroupBy(x => x.CategoryId)
                .Select(x => new ExpensePerCategory
                {
                    CategoryId = x.Key,
                    Category = categories.FirstOrDefault(y => y.Id == x.Key).Name ?? "Error",
                    Amount = x.Sum(i => i.Amount)
                })
                .OrderByDescending(x => x.Amount)
                .ToList();

            return groupedByCat;
        }

        public async Task<List<IncomeAndExpenseByMonth>> IncomeAndExpensesByMonths(
            string userId,
            string accountId = "",
            int monthsCount = 12)
        {
            List<Transaction> transactions;
            List<IncomeAndExpenseByMonth> result = new();
            // filtering for account if specified
            if (string.IsNullOrWhiteSpace(accountId))
            {
                transactions = await _transRepo.GetAllForUser(userId);
            }
            else
            {
                transactions = await _transRepo.GetAllForAccount(accountId);
            }

            var incomes = transactions.Where(x => x.TransactionType == TransactionType.Income).ToList();
            var expenses = transactions.Where(x => x.TransactionType == TransactionType.Expense).ToList();

            var beginDate = DateTime.Now.AddMonths(-monthsCount + 1);
            var index = beginDate.AddDays(-beginDate.Day + 1);
            while (index <= DateTime.Now)
            {
                result.Add(new IncomeAndExpenseByMonth
                {
                    Month = Month(index.Month),
                    Date = DateOnly.FromDateTime(index),
                    Expense = 0,
                    Income = 0
                });
                index = index.AddMonths(1);
            }
            foreach (var month in result)
            {
                var expensesInThisMonth = expenses
                    .Where(e => InSameMonth(e.Date, month.Date))
                    .Sum(x => x.Amount);
                var incomesInThisMonth = incomes
                    .Where(e => InSameMonth(e.Date, month.Date))
                    .Sum(x => x.Amount);
                month.Income = incomesInThisMonth;
                month.Expense = expensesInThisMonth;
            }

            return result;
        }

        private string Month(int month) => new DateTime(1, month, 1).ToString("MMMM", new CultureInfo("en_US"));

        private bool InSameMonth(DateTime a, DateOnly b)
        {
            var aFirstDayOfMonth = a.AddDays(-a.Day + 1);
            var bFirstDayOfMonth = b.AddDays(-b.Day + 1);
            return DateOnly.FromDateTime(aFirstDayOfMonth) == bFirstDayOfMonth;
        }
    }
}
