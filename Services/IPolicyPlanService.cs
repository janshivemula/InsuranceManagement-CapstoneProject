using InsuranceManagementSystem.DTOs.Common;
using InsuranceManagementSystem.DTOs.Plan;

namespace InsuranceManagementSystem.Services.Interfaces
{
    public interface IPolicyPlanService
    {
        Task<PagedResponse<PlanResponseDto>> GetAllPlansAsync(PolicyPlanQueryDto query);

        Task<IEnumerable<PlanResponseDto>> GetActivePlansAsync();

        Task<IEnumerable<PlanResponseDto>> GetPlansByProductIdAsync(int productId);

        Task<IEnumerable<PlanResponseDto>> GetActivePlansByProductIdAsync(int productId);

        Task<PlanResponseDto?> GetPlanByIdAsync(int id);

        Task<PlanResponseDto> CreatePlanAsync(PlanRequestDto requestDto);

        Task UpdatePlanAsync(int id, PlanRequestDto requestDto);

        Task SoftDeletePlanAsync(int id);
    }
}