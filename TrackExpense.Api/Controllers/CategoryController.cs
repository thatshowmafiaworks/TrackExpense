using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrackExpense.Api.Contracts.Categories;
using TrackExpense.Application.Interfaces;
using TrackExpense.Domain.Entities;

namespace TrackExpense.Api.Controllers
{
    [ApiController]
    [Route("category")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(
            ILogger<CategoryController> logger,
            UserManager<IdentityUser> userManager,
            ICategoryRepository categoryRepository
            )
        {
            _logger = logger;
            _userManager = userManager;
            _categoryRepo = categoryRepository;
        }

        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCategory(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("User tried to get category with NullOrWhiteSpace id");
                return BadRequest("Id cant be empty");
            }
            try
            {
                var category = await _categoryRepo.Get(id);
                if (category == null)
                {
                    _logger.LogWarning($"Not found Category with id: '{id}'");
                    return NotFound("This category cannot be found!");
                }
                return Ok(category);
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
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryRepo.GetAll();
                return Ok(categories);
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
        [Route("my")]
        public async Task<IActionResult> GetAllForCurrentUser()
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
                if (user == null)
                {
                    _logger.LogWarning($"Cant find user with email:{User.FindFirstValue(ClaimTypes.Email)}");
                    return Unauthorized("Something went wrong, try again later please");
                }

                var categories = await _categoryRepo.GetAllByUserId(user.Id);
                return Ok(categories);
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
        public async Task<IActionResult> CreateNew(CategoryDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"User tried to create category with {ModelState.ErrorCount} errors");
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
                var category = new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    Description = model.Description,
                    UserId = user.Id,
                };

                await _categoryRepo.Add(category);
                await _categoryRepo.Save();
                return Ok(new { category.Id });
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
        public async Task<IActionResult> Update(string id, CategoryDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"User tried to update category with {ModelState.ErrorCount} errors");
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

                var category = await _categoryRepo.Get(id);
                if (category == null)
                {
                    _logger.LogWarning($"Cant find category with id:{id}");
                    return BadRequest($"There is no category with id:{id}");
                }
                if (await _userManager.IsInRoleAsync(user, "Admin") || user.Id == category.UserId)
                {
                    category.Name = model.Name;
                    category.Description = model.Description;
                    _categoryRepo.Update(category);
                    await _categoryRepo.Save();

                    return Ok(category);
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
                _logger.LogWarning($"User tried to update category with {ModelState.ErrorCount} errors");
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

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    _logger.LogWarning($"User '{user.Id}' without permissions tried to delete category with id:'{id}'");
                    return StatusCode(403, $"You dont have permisions to delete '{id}'");
                }

                var category = await _categoryRepo.Get(id);
                if (category == null)
                {
                    _logger.LogWarning($"Cant find category with id:{id}");
                    return BadRequest($"There is no category with id:{id}");
                }

                _categoryRepo.Remove(category);
                await _categoryRepo.Save();

                return Ok();

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
