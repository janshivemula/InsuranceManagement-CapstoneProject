using InsuranceManagementSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace InsuranceManagementSystem.DTOs.Claim
{
    public class ClaimReviewDto
    {
        [Required]
        public ClaimStatus RecommendedStatus { get; set; }

        [Required]
        [StringLength(500)]
        public string Remarks { get; set; }
    }
}
