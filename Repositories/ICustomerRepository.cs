
using InsuranceManagementSystem.DTOs.Customer;
using InsuranceManagementSystem.Models;

namespace InsuranceManagementSystem.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<PagedResult<Customer>> GetAllAsync(CustomerQueryDto query);
        Task<IEnumerable<Customer>> GetActiveCustomersAsync();

        Task<Customer?> GetByIdAsync(int id);

        Task<Customer?> GetByUserIdAsync(int userId);

        Task AddAsync(Customer customer);

        Task UpdateAsync(Customer customer);

        Task SaveChangesAsync();
    }
}