using System.ComponentModel.DataAnnotations;

namespace InsuranceManagementSystem.DTOs.Policy
{
    public class IssuePolicyRequestDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int PlanId { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }
    }
}
