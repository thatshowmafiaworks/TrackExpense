namespace TrackExpense.Application.Dtos
{
    public class ExpensePerCategory
    {
        public string CategoryId { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
    }
}
