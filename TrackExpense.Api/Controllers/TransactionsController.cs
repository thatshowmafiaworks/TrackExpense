using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrackExpense.Api.Contracts.Transactions;
using TrackExpense.Application.Interfaces;
using TrackExpense.Domain.Entities;

namespace TrackExpense.Api.Controllers
{
    [ApiController]
    [Route("transaction")]
    public class TransactionsController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITransactionRepository _transacRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly ICategoryRepository _categoryRepo;

        public TransactionsController(
            ILogger<AccountController> logger,
            UserManager<IdentityUser> userManager,
            ITransactionRepository transacRepo,
            IAccountRepository accountRepo,
            ICategoryRepository categoryRepo)
        {
            _logger = logger;
            _userManager = userManager;
            _transacRepo = transacRepo;
            _accountRepo = accountRepo;
            _categoryRepo = categoryRepo;
        }

        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetTransaction(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("User tried to get transaction with NullOrWhiteSpace id");
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

                var transac = await _transacRepo.Get(id);
                if (transac == null)
                {
                    _logger.LogWarning($"Not found transaction with id: '{id}'");
                    return NotFound("This transaction cannot be found!");
                }

                if (transac.UserId == user.Id || await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    var response = new TransactionVM(transac);
                    return Ok(response);
                }

                _logger.LogWarning($"User without permissions tried to get transaction with id:'{id}'");
                return NotFound($"Transaction with id:'{id}' cant be found");
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
        public async Task<IActionResult> GetAllTransactionsForCurrentUser()
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
                if (user == null)
                {
                    _logger.LogWarning($"Cant find user with email:{User.FindFirstValue(ClaimTypes.Email)}");
                    return Unauthorized("Something went wrong, try again later please");
                }

                var transactions = await _transacRepo.GetAllForUser(user.Id);
                var response = transactions.Select(x => new TransactionVM(x)).ToList();
                return Ok(response);
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
        public async Task<IActionResult> GetAllTransactionsForUserAsAdmin(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("User tried to get transactions with NullOrWhiteSpace userId");
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

                var transactions = await _transacRepo.GetAllForUser(userId);
                var response = transactions.Select(x => new TransactionVM(x)).ToList();
                return Ok(response);
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
        public async Task<IActionResult> GetAllTransactionsAsAdmin()
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
                if (user == null)
                {
                    _logger.LogWarning($"Cant find user with email:{User.FindFirstValue(ClaimTypes.Email)}");
                    return Unauthorized("Something went wrong, try again later please");
                }

                var transactions = await _transacRepo.GetAll();
                var response = transactions.Select(x => new TransactionVM(x)).ToList();
                return Ok(response);
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
        [Route("account/{accountId}")]
        public async Task<IActionResult> GetAllTransactionsForAccount(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
            {
                _logger.LogWarning("User tried to get transactions with NullOrWhiteSpace accountId");
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
                var account = await _accountRepo.Get(accountId);
                if (account == null)
                {
                    _logger.LogWarning($"Cant find account with id: '{accountId}'");
                    return NotFound($"Cant find account with id:'{accountId}'");
                }
                if (account.UserId == user.Id || await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    var transactions = await _transacRepo.GetAllForAccount(accountId);
                    var response = transactions.Select(x => new TransactionVM(x)).ToList();
                    return Ok(response);
                }

                _logger.LogWarning($"User without permissions tried to get transaction from account with id:'{accountId}'");
                return NotFound($"Account with id:'{accountId}' cant be found");
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
        public async Task<IActionResult> CreateTransaction(TransactionDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"User tried to create Transaction with {ModelState.ErrorCount} errors");
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
                var category = await _categoryRepo.Get(model.CategoryId);
                if (category == null)
                {
                    _logger.LogWarning($"Cant find category with id:'{model.CategoryId}'");
                    return NotFound($"There is no category with id:'{model.CategoryId}'");
                }
                var account = await _accountRepo.Get(model.AccountId);
                if (account == null)
                {
                    _logger.LogWarning($"Cant find account with id:'{model.AccountId}'");
                    return NotFound($"There is no account with id:'{model.AccountId}'");
                }

                var transaction = new Transaction
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                    Amount = model.Amount,
                    TransactionType = model.TransactionType,
                    Date = DateTime.UtcNow,
                    Description = model.Description,
                    CategoryId = category.Id,
                    AccountId = account.Id
                };

                await _transacRepo.Add(transaction);
                await _transacRepo.Save();
                return Ok(new { transaction.Id });
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
        public async Task<IActionResult> Update(string id, TransactionDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"User tried to update transaction with {ModelState.ErrorCount} errors");
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

                var transaction = await _transacRepo.Get(id);
                if (transaction == null)
                {
                    _logger.LogWarning($"Cant find account with id:{id}");
                    return BadRequest($"There is no account with id:{id}");
                }
                if (await _userManager.IsInRoleAsync(user, "Admin") || user.Id == transaction.UserId)
                {
                    transaction.AccountId = model.AccountId;
                    transaction.CategoryId = model.CategoryId;
                    transaction.Amount = model.Amount;
                    transaction.TransactionType = model.TransactionType;
                    transaction.Description = model.Description;

                    _transacRepo.Update(transaction);
                    await _transacRepo.Save();

                    return Ok(new TransactionVM(transaction));
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
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning($"User tried to delete transaction with empty id");
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

                var transaction = await _transacRepo.Get(id);
                if (transaction == null)
                {
                    _logger.LogWarning($"Cant find transaction with id:{id}");
                    return BadRequest($"There is no transaction with id:{id}");
                }

                if (await _userManager.IsInRoleAsync(user, "Admin") || user.Id == transaction.UserId)
                {
                    _transacRepo.Remove(transaction);
                    await _transacRepo.Save();
                    return Ok();
                }

                _logger.LogWarning($"User '{user.Id}' without permissions tried to delete transaction with id:'{id}'");
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
