namespace TrackExpense.Api.Contracts.Reports
{
    public class CategoryExpensesAsPercentsDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AccountId { get; set; } = string.Empty;
    }
}
