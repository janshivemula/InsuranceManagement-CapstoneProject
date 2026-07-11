using InsuranceManagementSystem.DTOs.Claim;
using InsuranceManagementSystem.DTOs.Common;
using InsuranceManagementSystem.Enums;

namespace InsuranceManagementSystem.Services.Interfaces
{
    public interface IClaimService
    {
        Task<IEnumerable<ClaimResponseDto>> GetAllClaimsAsync();

        Task<IEnumerable<ClaimResponseDto>> GetClaimsByCustomerIdAsync(int customerId);

        Task<IEnumerable<ClaimResponseDto>> GetMyClaimsAsync(int userId);

        Task<IEnumerable<ClaimResponseDto>> GetClaimsByPolicyIdAsync(int policyId);

        Task<IEnumerable<ClaimResponseDto>> GetClaimsByStatusAsync(ClaimStatus status);

        
        Task<ClaimResponseDto?> GetClaimByIdAsync(int claimId, int userId, UserRole role);

        Task<ClaimResponseDto?> GetClaimByClaimNumberAsync(string claimNumber);

        Task<ClaimResponseDto> CreateClaimAsync(int userId, ClaimRequestDto requestDto);

        Task<ClaimDocumentResponseDto> AddClaimDocumentAsync(int userId, UserRole role, ClaimDocumentRequestDto requestDto);

        
        Task<IEnumerable<ClaimDocumentResponseDto>> GetClaimDocumentsAsync(int claimId, int userId, UserRole role);

        
        Task<IEnumerable<ClaimStatusHistoryResponseDto>> GetClaimStatusHistoryAsync(int claimId, int userId, UserRole role);

        Task<PagedResponse<ClaimResponseDto>> GetPagedClaimsAsync(ClaimPaginationRequestDto request);

        Task ReviewClaimByStaffAsync(int claimId, int staffUserId, ClaimReviewDto requestDto);

        Task DecideClaimByAdminAsync(int claimId, int adminUserId, ClaimDecisionDto requestDto);
    }
}