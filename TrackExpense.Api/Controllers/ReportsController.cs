using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrackExpense.Api.Contracts.Reports;
using TrackExpense.Application.Interfaces;

namespace TrackExpense.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<CategoryController> _logger;
        private readonly IAccountRepository _accountRepo;

        public ReportsController(
            IReportService reportService,
            UserManager<IdentityUser> userManager,
            ILogger<CategoryController> logger,
            IAccountRepository accountRepository)
        {
            _reportService = reportService;
            _userManager = userManager;
            _logger = logger;
            _accountRepo = accountRepository;
        }


        [HttpGet]
        [Route("expensespercategories")]
        public async Task<IActionResult> ExpensesPerCategories(string accountId = "", int days = 30)
        {
            try
            {
                if (days <= 0)
                {
                    _logger.LogWarning($"User tried to get report with '{days}' days");
                    return BadRequest($"Days cant be less than 1");
                }
                var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
                if (user == null)
                {
                    _logger.LogWarning($"Cant find user with email:{User.FindFirstValue(ClaimTypes.Email)}");
                    return Unauthorized("Something went wrong, try again later please");
                }
                var account = await _accountRepo.Get(accountId);
                if (account == null)
                {
                    _logger.LogWarning($"Cant find account with id:'{accountId}'");
                    accountId = string.Empty;

                }
                if (account != null && account?.UserId != user.Id)
                {
                    _logger.LogWarning($"User tried to get report for account with id:'{account?.Id}'");
                    return NotFound("This account cant be found");
                }
                var result = await _reportService.ExpensesPerCategories(user.Id, accountId, days);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception found with:'{ex.Message}'" +
                    $"\n with Inner: '{ex.InnerException?.Message}'" +
                    $"\n Stack: '{ex.StackTrace}'");
                return StatusCode(500, $"An unexpected error occurred");
            }
        }

        [HttpGet]
        [Route("incomeandexpensesbymonth")]
        public async Task<IActionResult> IncomeAndExpensesByMonths(string accountId = "", int months = 12)
        {
            try
            {
                if (months <= 0)
                {
                    _logger.LogWarning($"User tried to get report with '{months}' months");
                    return BadRequest($"Months cant be less than 1");
                }
                var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
                if (user == null)
                {
                    _logger.LogWarning($"Cant find user with email:{User.FindFirstValue(ClaimTypes.Email)}");
                    return Unauthorized("Something went wrong, try again later please");
                }
                var account = await _accountRepo.Get(accountId);
                if (account == null)
                {
                    _logger.LogWarning($"Cant find account with id:'{accountId}'");
                    accountId = string.Empty;

                }
                if (account != null && account?.UserId != user.Id)
                {
                    _logger.LogWarning($"User tried to get report for account with id:'{account?.Id}'");
                    return NotFound("This account cant be found");
                }
                var result = await _reportService.IncomeAndExpensesByMonths(user.Id, accountId, months);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception found with:'{ex.Message}'" +
                    $"\n with Inner: '{ex.InnerException?.Message}'" +
                    $"\n Stack: '{ex.StackTrace}'");
                return StatusCode(500, $"An unexpected error occurred");
            }
        }

        [HttpPost]
        [Route("topnexpenses")]
        public async Task<IActionResult> TopNExpenses(TopNExpensesDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"User tried to get 'TopNExpenses' report with {ModelState.ErrorCount} errors");
                return BadRequest("Please recheck your data");
            }
            if (model.StartDate > DateTime.UtcNow)
            {
                _logger.LogWarning($"User tried to get 'TopNExpenses' report with StartDate later that now");
                return BadRequest("StartDate cant be later than Utc Now");
            }
            if (model.EndDate > DateTime.UtcNow) model.EndDate = DateTime.UtcNow;
            if (model.NItems < 0)
            {
                _logger.LogWarning($"User tried to get report with '{model.NItems}' items");
                return BadRequest($"Number of items cant be less than ");
            }
            try
            {
                var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
                if (user == null)
                {
                    _logger.LogWarning($"Cant find user with email:{User.FindFirstValue(ClaimTypes.Email)}");
                    return Unauthorized("Something went wrong, try again later please");
                }
                var account = await _accountRepo.Get(model.AccountId);
                if (account == null)
                {
                    _logger.LogWarning($"Cant find account with id:'{model.AccountId}'");
                    model.AccountId = string.Empty;

                }
                if (account != null && account?.UserId != user.Id)
                {
                    _logger.LogWarning($"User tried to get report for account with id:'{account?.Id}'");
                    return NotFound("This account cant be found");
                }
                var result = await _reportService.TopNExpenses(
                    user.Id,
                    model.StartDate,
                    model.EndDate,
                    model.AccountId,
                    model.NItems
                    );
                var response = result.Select(x => new TopNExpensesVM(x)).ToList();
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

        [HttpPost]
        [Route("categoryexpensesaspercents")]
        public async Task<IActionResult> CategoryExpensesAsPercents(CategoryExpensesAsPercentsDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"User tried to get 'CategoryExpensesAsPercents' report with {ModelState.ErrorCount} errors");
                return BadRequest("Please recheck your data");
            }
            if (model.StartDate > DateTime.UtcNow)
            {
                _logger.LogWarning($"User tried to get 'CategoryExpensesAsPercents' report with StartDate later that now");
                return BadRequest("StartDate cant be later than Utc Now");
            }
            if (model.EndDate > DateTime.UtcNow) model.EndDate = DateTime.UtcNow;
            try
            {
                var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
                if (user == null)
                {
                    _logger.LogWarning($"Cant find user with email:{User.FindFirstValue(ClaimTypes.Email)}");
                    return Unauthorized("Something went wrong, try again later please");
                }
                var account = await _accountRepo.Get(model.AccountId);
                if (account == null)
                {
                    _logger.LogWarning($"Cant find account with id:'{model.AccountId}'");
                    model.AccountId = string.Empty;

                }
                if (account != null && account?.UserId != user.Id)
                {
                    _logger.LogWarning($"User tried to get report for account with id:'{account?.Id}'");
                    return NotFound("This account cant be found");
                }
                var result = await _reportService.CategoryExpensesAsPercents(
                    user.Id,
                    model.StartDate,
                    model.EndDate,
                    model.AccountId
                    );
                return Ok(result);
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
