using InsuranceManagementSystem.DTOs.Common;
using InsuranceManagementSystem.Enums;
using System.Data;

namespace InsuranceManagementSystem.DTOs.User
{
    public class UserQueryDto : PaginationRequestDto
    {
        public UserRole? Role { get; set; }

        public bool? IsActive { get; set; }
    }
}