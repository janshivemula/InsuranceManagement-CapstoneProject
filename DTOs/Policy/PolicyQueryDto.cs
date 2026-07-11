using InsuranceManagementSystem.DTOs.Common;
using InsuranceManagementSystem.Enums;

namespace InsuranceManagementSystem.DTOs.Policy
{
    public class PolicyQueryDto : PaginationRequestDto
    {
        public PolicyStatus? Status { get; set; }

        public int? CustomerId { get; set; }

        public int? PlanId { get; set; }
    }
}
