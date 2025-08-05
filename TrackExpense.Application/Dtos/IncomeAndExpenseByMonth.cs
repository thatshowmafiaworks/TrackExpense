namespace TrackExpense.Application.Dtos
{
    public class IncomeAndExpenseByMonth
    {
        public string Month { get; set; }
        public DateOnly Date { get; set; }
        public Decimal Income { get; set; }
        public Decimal Expense { get; set; }
    }
}
