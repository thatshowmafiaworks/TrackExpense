namespace TrackExpense.Domain.Entities
{
    public class Account
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string? Description { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
