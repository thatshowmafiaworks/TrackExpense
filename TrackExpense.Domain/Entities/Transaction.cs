namespace TrackExpense.Domain.Entities
{
    public class Transaction
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public string CategoryId { get; set; }
        public Category Category { get; set; } = default!;
        public string AccountId { get; set; }
        public Account Account { get; set; }
    }
}
