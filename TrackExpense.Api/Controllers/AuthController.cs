using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrackExpense.Api.Contracts.Auth;
using TrackExpense.Application.Interfaces;

namespace TrackExpense.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<AuthController> logger,
            IJwtTokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserCredentials creds)
        {
            if (creds == null)
            {
                _logger.LogWarning($"Null credentials was sent");
                return Unauthorized(new { Error = "Something wrong, please try again" });
            }
            if (string.IsNullOrWhiteSpace(creds.Email))
            {
                _logger.LogWarning($"User sent empty email");
                return Unauthorized(new { Error = "Something wrong, please try again" });
            }
            if (string.IsNullOrWhiteSpace(creds.Password))
            {
                _logger.LogWarning($"User with email:'{creds.Email}' sent empty password");
                return Unauthorized(new { Error = "Something wrong, please try again" });
            }
            var user = await _userManager.FindByEmailAsync(creds.Email);
            if (user == null)
            {
                _logger.LogWarning($"Registered user with email:'{creds.Email}' was not found");
                return Unauthorized(new { Error = "Wrong email, please recheck" });
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, creds.Password, false);
            if (!result.Succeeded)
            {
                _logger.LogWarning($"User tried to signin with email:'{creds.Email}' with wrong password");
                return Unauthorized(new { Error = "Wrong password or email, try again please" });
            }
            try
            {
                var roles = await _userManager.GetRolesAsync(user);

                var token = _tokenGenerator.GenerateToken(user, roles);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Signing in went wrong with exception '{nameof(ex)}':{ex.Message}\nWith Inner:{ex.InnerException?.Message}");
                return StatusCode(500, new { Error = "Something wrong, please try again" });
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserCredentials creds)
        {
            if (creds == null)
            {
                _logger.LogWarning($"Null credentials was sent");
                return StatusCode(500, new { Error = "Something wrong, please try again" });
            }
            if (string.IsNullOrWhiteSpace(creds.Email))
            {
                _logger.LogWarning($"User sent empty email");
                return BadRequest(new { Error = "Empty email" });
            }
            if (string.IsNullOrWhiteSpace(creds.Password))
            {
                _logger.LogWarning($"User with email:'{creds.Email}' sent empty password");
                return BadRequest(new { Error = "Empty password" });
            }
            var user = await _userManager.FindByEmailAsync(creds.Email);
            if (user != null)
            {
                _logger.LogWarning($"User tried to register existing email:'{creds.Email}'");
                return BadRequest(new { Error = "This Email is already registered" });
            }
            try
            {
                var newUser = new IdentityUser()
                {
                    UserName = creds.Email,
                    Email = creds.Email
                };
                await _userManager.CreateAsync(newUser, creds.Password);
                await _userManager.AddToRoleAsync(newUser, "User");
                _logger.LogInformation($"Created new user with email:'{newUser.Email}'");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Register went wrong with exception:{ex.Message}\nWith Inner:{ex.InnerException?.Message}");
                return StatusCode(500, new { Error = "Something wrong, please try again" });
            }
        }
    }
}
