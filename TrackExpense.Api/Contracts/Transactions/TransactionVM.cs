using TrackExpense.Domain.Entities;

namespace TrackExpense.Api.Contracts.Transactions
{
    public class TransactionVM
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public string CategoryId { get; set; }
        public string AccountId { get; set; }
        public TransactionVM(Transaction transac)
        {
            Id = transac.Id;
            Amount = transac.Amount;
            TransactionType = transac.TransactionType;
            Date = transac.Date;
            Description = transac.Description;
            CategoryId = transac.CategoryId;
            AccountId = transac.AccountId;
        }
    }
}
