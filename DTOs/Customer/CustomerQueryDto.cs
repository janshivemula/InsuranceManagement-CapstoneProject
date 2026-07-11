using InsuranceManagementSystem.DTOs.Common;

namespace InsuranceManagementSystem.DTOs.Customer
{
    public class CustomerQueryDto : PaginationRequestDto
    {
        public bool? IsActive { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }
    }
}