using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrackExpense.Api.Contracts.Accounts;
using TrackExpense.Application.Interfaces;
using TrackExpense.Domain.Entities;

namespace TrackExpense.Api.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAccountRepository _accountRepo;
        public AccountController(
            ILogger<AccountController> logger,
            IAccountRepository accountRepository,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _accountRepo = accountRepository;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAccount(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("User tried to get account with NullOrWhiteSpace id");
                return BadRequest("Id cant be empty");
            }
            try
            {
                var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
                if (user == null)
                {
                    _logger.LogWarning($"Cant find user with email:{User.FindFirstValue(ClaimTypes.Email)}");
                    return Unauthorized("Something went wrong, try again later please");
                }

                var account = await _accountRepo.Get(id);
                if (account == null)
                {
                    _logger.LogWarning($"Not found account with id: '{id}'");
                    return NotFound("This account cannot be found!");
                }

                if (account.UserId == user.Id || await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return Ok(account);
                }

                _logger.LogWarning($"User without permissions tried to get account with id:'{id}'");
                return NotFound($"Account with id:'{id}' cant be found");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception found with:'{ex.Message}'" +
                    $"\n with Inner: '{ex.InnerException?.Message}'" +
                    $"\n Stack: '{ex.StackTrace}'");
                return StatusCode(500, $"An unexpected error occurred");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAccountsForUser()
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
                if (user == null)
                {
                    _logger.LogWarning($"Cant find user with email:{User.FindFirstValue(ClaimTypes.Email)}");
                    return Unauthorized("Something went wrong, try again later please");
                }

                var accounts = await _accountRepo.GetForUser(user.Id);
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception found with:'{ex.Message}'" +
                    $"\n with Inner: '{ex.InnerException?.Message}'" +
                    $"\n Stack: '{ex.StackTrace}'");
                return StatusCode(500, $"An unexpected error occurred");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("user/{userId}")]
        public async Task<IActionResult> GetAllAccountsForUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("User tried to get users accounts with NullOrWhiteSpace userId");
                return BadRequest("Id cant be empty");
            }
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"Cant find user with id:'{userId}'");
                    return Unauthorized("Something went wrong, try again later please");
                }

                var accounts = await _accountRepo.GetForUser(userId);
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception found with:'{ex.Message}'" +
                    $"\n with Inner: '{ex.InnerException?.Message}'" +
                    $"\n Stack: '{ex.StackTrace}'");
                return StatusCode(500, $"An unexpected error occurred");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllAccountsAsAdmin()
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
                if (user == null)
                {
                    _logger.LogWarning($"Cant find user with email:{User.FindFirstValue(ClaimTypes.Email)}");
                    return Unauthorized("Something went wrong, try again later please");
                }

                var accounts = await _accountRepo.GetAll();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception found with:'{ex.Message}'" +
                    $"\n with Inner: '{ex.InnerException?.Message}'" +
                    $"\n Stack: '{ex.StackTrace}'");
                return StatusCode(500, $"An unexpected error occurred");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAccount(AccountDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"User tried to create account with {ModelState.ErrorCount} errors");
                return BadRequest("Please recheck your data");
            }
            try
            {
                var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
                if (user == null)
                {
                    _logger.LogWarning($"Cant find user with email:{User.FindFirstValue(ClaimTypes.Email)}");
                    return Unauthorized("Something went wrong, try again later please");
                }
                var account = new Account
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    Description = model.Description,
                    UserId = user.Id,
                };

                await _accountRepo.Add(account);
                await _accountRepo.Save();
                return Ok(new { account.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception found with:'{ex.Message}'" +
                    $"\n with Inner: '{ex.InnerException?.Message}'" +
                    $"\n Stack: '{ex.StackTrace}'");
                return StatusCode(500, $"An unexpected error occurred");
            }
        }

        [Authorize]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(string id, AccountDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"User tried to update account with {ModelState.ErrorCount} errors");
                return BadRequest("Please recheck your data");
            }
            try
            {
                var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
                if (user == null)
                {
                    _logger.LogWarning($"Cant find user with email:{User.FindFirstValue(ClaimTypes.Email)}");
                    return Unauthorized("Something went wrong, try again later please");
                }

                var account = await _accountRepo.Get(id);
                if (account == null)
                {
                    _logger.LogWarning($"Cant find account with id:{id}");
                    return BadRequest($"There is no account with id:{id}");
                }
                if (await _userManager.IsInRoleAsync(user, "Admin") || user.Id == account.UserId)
                {
                    account.Name = model.Name;
                    account.Description = model.Description;
                    _accountRepo.Update(account);
                    await _accountRepo.Save();

                    return Ok(account);
                }
                return StatusCode(403, $"You dont have permisions to modify '{id}'");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception found with:'{ex.Message}'" +
                    $"\n with Inner: '{ex.InnerException?.Message}'" +
                    $"\n Stack: '{ex.StackTrace}'");
                return StatusCode(500, $"An unexpected error occurred");
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"User tried to update account with {ModelState.ErrorCount} errors");
                return BadRequest("Please recheck your data");
            }
            try
            {
                var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
                if (user == null)
                {
                    _logger.LogWarning($"Cant find user with email:{User.FindFirstValue(ClaimTypes.Email)}");
                    return Unauthorized("Something went wrong, try again later please");
                }

                var account = await _accountRepo.Get(id);
                if (account == null)
                {
                    _logger.LogWarning($"Cant find account with id:{id}");
                    return BadRequest($"There is no account with id:{id}");
                }

                if (await _userManager.IsInRoleAsync(user, "Admin") || user.Id == account.UserId)
                {
                    _accountRepo.Remove(account);
                    await _accountRepo.Save();
                    return Ok();
                }

                _logger.LogWarning($"User '{user.Id}' without permissions tried to delete account with id:'{id}'");
                return StatusCode(403, $"You dont have permisions to delete '{id}'");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception found with:'{ex.Message}'" +
                    $"\n with Inner: '{ex.InnerException?.Message}'" +
                    $"\n Stack: '{ex.StackTrace}'");
                return StatusCode(500, $"An unexpected error occurred");
            }
        }
    }
}
