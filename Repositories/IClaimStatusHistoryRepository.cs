
using InsuranceManagementSystem.DTOs.Claim;
using InsuranceManagementSystem.Models;

namespace InsuranceManagementSystem.Repositories.Interfaces
{
    public interface IClaimStatusHistoryRepository
    {
        Task<IEnumerable<ClaimStatusHistory>> GetAllAsync();

        Task<IEnumerable<ClaimStatusHistory>> GetByClaimIdAsync(int claimId);

        Task<ClaimStatusHistory?> GetByIdAsync(int historyId);

        Task AddAsync(ClaimStatusHistory claimStatusHistory);

        Task SaveChangesAsync();
    }
}