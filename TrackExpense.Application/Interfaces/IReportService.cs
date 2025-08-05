using TrackExpense.Application.Dtos;

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
    }
}
