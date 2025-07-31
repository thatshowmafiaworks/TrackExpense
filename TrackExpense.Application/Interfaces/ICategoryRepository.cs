using TrackExpense.Domain.Entities;

namespace TrackExpense.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAll();
        Task<Category?> Get(string id);
        Task<List<Category>> GetAllByUserId(string userId);
        Task Add(Category category);
        void Update(Category category);
        void Remove(Category category);
        Task Save();
    }
}
