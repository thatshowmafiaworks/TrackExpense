namespace TrackExpense.Domain.Entities
{
    public class Category
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
