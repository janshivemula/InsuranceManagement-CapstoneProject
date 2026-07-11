using InsuranceManagementSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace InsuranceManagementSystem.DTOs.Claim
{
    public class ClaimDecisionDto
    {
        [Required]
        public ClaimStatus FinalDecisionStatus { get; set; }

        [Required]
        [StringLength(500)]
        public string Remarks { get; set; }
    }
}
