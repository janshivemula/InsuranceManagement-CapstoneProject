using InsuranceManagementSystem.DTOs.User;
using InsuranceManagementSystem.Models;

namespace InsuranceManagementSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<PagedResult<User>> GetAllAsync(UserQueryDto query);

        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<IEnumerable<User>> GetActiveInternalStaffAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdForUpdateAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> UpdateUserStatusAsync(int userId, bool isActive);
        Task SoftDeleteAsync(User user);
        Task SaveChangesAsync();
    }
}