using System.ComponentModel.DataAnnotations;

namespace InsuranceManagementSystem.DTOs.Claim
{
    public class SupportingDocumentDto
    {
        [Required]
        [StringLength(100)]
        public string DocumentName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string DocumentReference { get; set; } = string.Empty;
    }
}
