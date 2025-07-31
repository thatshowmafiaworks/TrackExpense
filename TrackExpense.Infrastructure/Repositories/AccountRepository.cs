using Microsoft.EntityFrameworkCore;
using TrackExpense.Application.Interfaces;
using TrackExpense.Domain.Entities;
using TrackExpense.Infrastructure.Data;

namespace TrackExpense.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _dbContext;

        public AccountRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public async Task<Account?> Get(string id)
            => await _dbContext.Accounts.FindAsync(id);

        public async Task<List<Account>> GetAll()
            => await _dbContext.Accounts.ToListAsync();

        public async Task<List<Account>> GetForUser(string userId)
            => await _dbContext.Accounts.Where(x => x.UserId == userId).ToListAsync();

        public async Task Add(Account account)
            => await _dbContext.Accounts.AddAsync(account);

        public void Update(Account account)
            => _dbContext.Accounts.Update(account);

        public void Remove(Account account)
            => _dbContext.Remove(account);

        public async Task Save() => await _dbContext.SaveChangesAsync();
    }
}
