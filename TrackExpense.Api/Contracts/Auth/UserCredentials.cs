using System.ComponentModel.DataAnnotations;

namespace TrackExpense.Api.Contracts.Auth
{
    public class UserCredentials
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
