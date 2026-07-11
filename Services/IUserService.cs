using InsuranceManagementSystem.DTOs.Common;
using InsuranceManagementSystem.DTOs.User;

namespace InsuranceManagementSystem.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResponse<UserResponseDto>> GetAllUsersAsync(UserQueryDto query);

        Task<IEnumerable<UserResponseDto>> GetActiveUsersAsync();

        Task<IEnumerable<UserResponseDto>> GetActiveInternalStaffAsync();

        Task<UserResponseDto?> GetUserByIdAsync(int id);

        Task<UserResponseDto> CreateAdminAsync(CreateAdminRequestDto dto);

        Task<UserResponseDto> CreateInternalStaffAsync(CreateInternalStaffRequestDto dto);

        Task<UserResponseDto> UpdateUserStatusAsync(int id, UserStatusUpdateDto dto);
        Task<bool> UpdateUserStatusAsync(int userId, bool isActive);
    }
}