using Microsoft.EntityFrameworkCore;
using TrackExpense.Application.Interfaces;
using TrackExpense.Domain.Entities;
using TrackExpense.Infrastructure.Data;

namespace TrackExpense.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _dbContext;

        public CategoryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Category?> Get(string id)
            => await _dbContext.Categories.FindAsync(id);

        public async Task<List<Category>> GetAll()
            => await _dbContext.Categories.ToListAsync();

        public async Task<List<Category>> GetAllByUserId(string userId)
            => await _dbContext.Categories.Where(x => x.UserId == userId).ToListAsync();

        public async Task Add(Category category)
            => await _dbContext.Categories.AddAsync(category);


        public void Update(Category category)
            => _dbContext.Update(category);

        public void Remove(Category category)
            => _dbContext.Remove(category);

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
