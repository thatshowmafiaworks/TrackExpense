using TrackExpense.Domain.Entities;

namespace TrackExpense.Application.Interfaces
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetAll();
        Task<List<Account>> GetForUser(string userId);
        Task<Account?> Get(string id);
        Task Add(Account account);
        void Update(Account account);
        void Remove(Account account);
        Task Save();
    }
}
