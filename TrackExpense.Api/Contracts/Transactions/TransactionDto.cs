using System.ComponentModel.DataAnnotations;
using TrackExpense.Domain.Entities;

namespace TrackExpense.Api.Contracts.Transactions
{
    public class TransactionDto
    {
        [Required]
        public string AccountId { get; set; }
        [Required]
        public string CategoryId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public TransactionType TransactionType { get; set; }

        public string? Description { get; set; }

    }
}
