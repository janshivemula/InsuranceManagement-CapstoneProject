using InsuranceManagementSystem.DTOs.Auth;
using InsuranceManagementSystem.DTOs.User;

namespace InsuranceManagementSystem.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterCustomerAsync(RegisterRequestDto dto);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);

    }
}