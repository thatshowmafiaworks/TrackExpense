using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrackExpense.Application.Contracts;

namespace TrackExpense.Api.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _config;
        private readonly ILogger<JwtTokenGenerator> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public JwtTokenGenerator(IConfiguration config, ILogger<JwtTokenGenerator> logger, UserManager<IdentityUser> userManager)
        {
            _config = config;
            _logger = logger;
            _userManager = userManager;
        }
        public string GenerateToken(IdentityUser user, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_config["JWT:SigninKey"] ?? throw new ArgumentException("JWT:SigninKey is empty"));


            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(ClaimTypes.Email,user.Email ?? throw new ArgumentException("User Email is empty")),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Name,user.UserName ?? throw new ArgumentException("User Name is empty"))
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role.ToString(), role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
