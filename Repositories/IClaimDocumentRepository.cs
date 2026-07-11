
using InsuranceManagementSystem.DTOs.Claim;
using InsuranceManagementSystem.Models;

namespace InsuranceManagementSystem.Repositories.Interfaces
{
    public interface IClaimDocumentRepository
    {
        Task<IEnumerable<ClaimDocument>> GetAllAsync();

        Task<IEnumerable<ClaimDocument>> GetByClaimIdAsync(int claimId);

        Task<ClaimDocument?> GetByIdAsync(int id);

        Task AddAsync(ClaimDocument claimDocument);

        Task SaveChangesAsync();
    }
}