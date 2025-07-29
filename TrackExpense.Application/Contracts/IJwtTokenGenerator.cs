using Microsoft.AspNetCore.Identity;

namespace TrackExpense.Application.Contracts
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(IdentityUser user, IList<string> roles);
    }
}
