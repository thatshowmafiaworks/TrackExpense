namespace TrackExpense.Application.Dtos
{
    public class CategoryAndPercent
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }

        public decimal Total { get; set; }
        public decimal Percents { get; set; }
    }
}
