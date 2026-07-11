using InsuranceManagementSystem.DTOs.Common;
using InsuranceManagementSystem.DTOs.PremiumPayment;

namespace InsuranceManagementSystem.Services.Interfaces
{
    public interface IPremiumPaymentService
    {
        Task<PagedResponse<PremiumPaymentResponseDto>> GetAllPaymentsAsync(PaginationRequestDto paginationDto);
        Task<PagedResponse<PremiumPaymentResponseDto>> GetPaymentsByPolicyIdAsync(int policyId, PaginationRequestDto paginationDto);

        Task<PagedResponse<PremiumPaymentResponseDto>> GetPaymentsByCustomerIdAsync(int customerId, int userId, string role, PaginationRequestDto paginationDto);

        Task<PremiumPaymentResponseDto?> GetPaymentByIdAsync(int id);

        Task<PremiumPaymentResponseDto> MakePaymentAsync(PremiumPaymentRequestDto requestDto, int userId, string role);
    }
}
