using InsuranceManagementSystem.Enums;

namespace InsuranceManagementSystem.Helpers
{
    public static class ClaimStatusValidator
    {
        public static bool IsValidTransition(ClaimStatus currentStatus, ClaimStatus newStatus)
        {
            return currentStatus switch
            {
                ClaimStatus.Submitted => newStatus == ClaimStatus.UnderReview,

                ClaimStatus.UnderReview =>
                    newStatus == ClaimStatus.RecommendedForApproval ||
                    newStatus == ClaimStatus.RecommendedForRejection,

                ClaimStatus.RecommendedForApproval => newStatus == ClaimStatus.Approved,

                ClaimStatus.RecommendedForRejection => newStatus == ClaimStatus.Rejected,

                ClaimStatus.Approved => false,

                ClaimStatus.Rejected => false,

                _ => false
            };
        }
    }
}
