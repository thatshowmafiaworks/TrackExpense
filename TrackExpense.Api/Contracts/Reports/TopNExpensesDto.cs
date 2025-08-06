namespace TrackExpense.Api.Contracts.Reports
{
    public class TopNExpensesDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public int NItems { get; set; } = 0;
    }
}
