using TrackExpense.Application.Dtos;
using TrackExpense.Domain.Entities;

namespace TrackExpense.Application.Interfaces
{
    public interface IReportService
    {
        Task<List<ExpensePerCategory>> ExpensesPerCategories(
            string userId,
            string accountId = "",
            int daysCount = 30);
        Task<List<IncomeAndExpenseByMonth>> IncomeAndExpensesByMonths(
            string userId,
            string accountId = "",
            int monthsCount = 12);

        Task<List<Transaction>> TopNExpenses(
            string userId,
            DateTime startDate,
            DateTime endDate,
            string accountId = "",
            int nItems = 0
            );

        Task<List<CategoryAndPercent>> CategoryExpensesAsPercents(
            string userId,
            DateTime startDate,
            DateTime endDate,
            string accountId = ""
            );
    }
}
