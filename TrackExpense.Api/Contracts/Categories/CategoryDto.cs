using System.ComponentModel.DataAnnotations;

namespace TrackExpense.Api.Contracts.Categories
{
    public class CategoryDto
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
