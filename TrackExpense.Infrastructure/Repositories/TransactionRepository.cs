using Microsoft.EntityFrameworkCore;
using TrackExpense.Application.Interfaces;
using TrackExpense.Domain.Entities;
using TrackExpense.Infrastructure.Data;

namespace TrackExpense.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _dbContext;

        public TransactionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Transaction?> Get(string id)
            => await _dbContext.Transactions.FindAsync(id);

        public async Task<List<Transaction>> GetAll()
            => await _dbContext.Transactions.ToListAsync();

        public async Task<List<Transaction>> GetAllForUser(string id)
            => await _dbContext.Transactions.Where(t => t.UserId == id).ToListAsync();

        public async Task Add(Transaction transaction)
            => await _dbContext.Transactions.AddAsync(transaction);

        public void Update(Transaction transaction)
            => _dbContext.Transactions.Update(transaction);

        public void Remove(Transaction transaction)
            => _dbContext.Transactions.Remove(transaction);

        public async Task Save()
            => await _dbContext.SaveChangesAsync();
    }
}
