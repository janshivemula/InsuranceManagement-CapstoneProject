using System.ComponentModel.DataAnnotations;
using InsuranceManagementSystem.Enums;

namespace InsuranceManagementSystem.DTOs.PremiumPayment
{
    public class PremiumPaymentRequestDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Policy id must be greater than 0.")]
        public int PolicyId { get; set; }

        [Range(typeof(decimal), "1", "999999999999", ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [EnumDataType(typeof(PaymentMode), ErrorMessage = "Invalid payment mode.")]
        public PaymentMode PaymentMode { get; set; }

        [Required(ErrorMessage = "Transaction reference is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Transaction reference must be between 5 and 100 characters.")]
        public string TransactionReference { get; set; } = string.Empty;
    }
}