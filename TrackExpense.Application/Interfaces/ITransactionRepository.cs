using TrackExpense.Domain.Entities;

namespace TrackExpense.Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAll();
        Task<List<Transaction>> GetAllForUser(string id);
        Task<Transaction?> Get(string id);
        Task Add(Transaction transaction);
        void Update(Transaction transaction);
        void Remove(Transaction transaction);
        Task Save();
    }
}
