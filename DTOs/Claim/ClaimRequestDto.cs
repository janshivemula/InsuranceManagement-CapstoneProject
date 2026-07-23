using System.ComponentModel.DataAnnotations;

namespace InsuranceManagementSystem.DTOs.Claim
{
    public class ClaimRequestDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Policy id must be greater than 0.")]
        public int PolicyId { get; set; }

        [Range(typeof(decimal), "1", "999999999999",
            ErrorMessage = "Claim amount must be greater than 0.")]
        public decimal ClaimAmount { get; set; }

        [Required(ErrorMessage = "Claim reason is required.")]
        [StringLength(500, MinimumLength = 5,
            ErrorMessage = "Claim reason must be between 5 and 500 characters.")]
        public string ClaimReason { get; set; } = string.Empty;

        [Required(ErrorMessage = "Incident date is required.")]
        public DateTime IncidentDate { get; set; }

        [Required(ErrorMessage = "At least one claim document is required.")]
        [MinLength(1, ErrorMessage = "At least one claim document is required.")]
        public List<ClaimDocumentRequestDto> Documents { get; set; } = new();
    }
}