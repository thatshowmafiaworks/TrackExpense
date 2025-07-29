using Microsoft.AspNetCore.Identity;
using TrackExpense.Application.Contracts;

namespace TrackExpense.Api.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        public string GenerateToken(IdentityUser user, IList<string> roles)
        {
            throw new NotImplementedException();
        }
    }
}
