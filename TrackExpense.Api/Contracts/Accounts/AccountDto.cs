using System.ComponentModel.DataAnnotations;

namespace TrackExpense.Api.Contracts.Accounts
{
    public class AccountDto
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
