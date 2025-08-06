using TrackExpense.Domain.Entities;

namespace TrackExpense.Api.Contracts.Reports
{
    public class TopNExpensesVM
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public string CategoryId { get; set; }
        public string AccountId { get; set; }

        public TopNExpensesVM(Transaction item)
        {
            Id = item.Id;
            Amount = item.Amount;
            TransactionType = item.TransactionType;
            Date = item.Date;
            Description = item.Description;
            CategoryId = item.CategoryId;
            AccountId = item.AccountId;
        }
    }
}
