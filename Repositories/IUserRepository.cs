using Reddit.Models;

namespace Reddit.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync(int pageNumber = 1, int pageSize = 1000, bool? isAscending = true, string? sortKey = null, string? searchKey = null, string? filterOn = null);
        Task<User?> GetByIdAsync(int id);
        Task<User?> PutUserAsync(int id, User user);
        Task<User> PostUserAsync(User user);
        Task<User?> DeleteUserAsync(int id);


    }
}
