
using InsuranceManagementSystem.DTOs.Claim;
using InsuranceManagementSystem.Enums;
using InsuranceManagementSystem.Models;

namespace InsuranceManagementSystem.Repositories.Interfaces
{
    public interface IClaimRepository
    {
        Task<IEnumerable<Claim>> GetAllAsync();

        Task<Claim?> GetByIdAsync(int id);

        Task<Claim?> GetByClaimNumberAsync(string claimNumber);

        Task<IEnumerable<Claim>> GetClaimsByCustomerIdAsync(int customerId);

        Task<IEnumerable<Claim>> GetClaimsByPolicyIdAsync(int policyId);

        Task<IEnumerable<Claim>> GetClaimsByStatusAsync(ClaimStatus status);
        Task AddAsync(Claim claim);
        Task UpdateAsync(Claim claim);

        Task<bool> ClaimNumberExistsAsync(string claimNumber);

        Task SaveChangesAsync();

        Task<(IEnumerable<Claim> Claims, int TotalRecords)> GetPagedClaimsAsync(ClaimPaginationRequestDto request);
        Task<bool> IsClaimOwnedByCustomerAsync(int claimId, int customerId);
        Task<bool> HasOpenClaimAsync(int policyId, DateOnly incidentDate);
    }

}