using Microsoft.AspNetCore.Identity;

namespace TrackExpense.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(IdentityUser user, IList<string> roles);
    }
}
