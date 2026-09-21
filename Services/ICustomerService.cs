using InsuranceManagementSystem.DTOs.Common;
using InsuranceManagementSystem.DTOs.Customer;
using Microsoft.AspNetCore.Http;

namespace InsuranceManagementSystem.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<PagedResponse<CustomerResponseDto>> GetAllCustomersAsync(CustomerQueryDto query);

        Task<IEnumerable<CustomerResponseDto>> GetActiveCustomersAsync();

        Task<CustomerResponseDto?> GetCustomerByIdAsync(int id);

        Task<CustomerResponseDto?> GetCustomerByUserIdAsync(int userId);
        Task<CustomerResponseDto?> GetMyProfileAsync(int loggedInUserId);

        Task<CustomerResponseDto> CreateCustomerAsync(int userId,CustomerRequestDto requestDto,IFormFile? profileImage);

        Task<CustomerResponseDto> UpdateCustomerAsync(int id,int loggedInUserId,CustomerRequestDto requestDto,IFormFile? profileImage);
    }
}