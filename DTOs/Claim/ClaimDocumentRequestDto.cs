using System.ComponentModel.DataAnnotations;

namespace InsuranceManagementSystem.DTOs.Claim
{
    public class ClaimDocumentRequestDto
    {
        [Required]
        public int ClaimId { get; set; }

        [Required(ErrorMessage = "Document name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Document name must be between 2 and 100 characters.")]
        public string DocumentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Document type is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Document type must be between 2 and 50 characters.")]
        public string DocumentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Document reference is required.")]
        [StringLength(300, MinimumLength = 3, ErrorMessage = "Document reference must be between 3 and 300 characters.")]
        public string DocumentReference { get; set; } = string.Empty;
    }
}