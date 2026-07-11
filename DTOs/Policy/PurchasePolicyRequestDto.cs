using System.ComponentModel.DataAnnotations;

namespace InsuranceManagementSystem.DTOs.Policy
{
    public class PurchasePolicyRequestDto
    {
        [Required]
        public int PlanId { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }
    }
}
