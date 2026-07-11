using InsuranceManagementSystem.Data;
using InsuranceManagementSystem.DTOs.Common;
using InsuranceManagementSystem.Models;
using InsuranceManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagementSystem.Repositories.Implementations
{
    public class PolicyPlanRepository : IPolicyPlanRepository
    {
        private readonly AppDbContext _context;

        // Constructor for Dependency Injection
        public PolicyPlanRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves all policy plans.
        public async Task<PagedResponse<PolicyPlan>> GetAllAsync(PolicyPlanQueryDto query)
        {
            var plans = _context.PolicyPlans
                .Include(p => p.InsuranceProduct)
                .Include(p => p.Policies)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                plans = plans.Where(p => p.PlanName.Contains(query.Search));
            }

            // Filter by Product
            if (query.ProductId.HasValue)
            {
                plans = plans.Where(p => p.InsuranceProductId == query.ProductId.Value);
            }

            // Filter by Premium Type
            if (query.PremiumType.HasValue)
            {
                plans = plans.Where(p => p.PremiumType == query.PremiumType.Value);
            }

            // Filter by Active Status
            if (query.IsActive.HasValue)
            {
                plans = plans.Where(p => p.IsActive == query.IsActive.Value);
            }

            // Sorting
            plans = query.SortBy.ToLower() switch
            {
                "coverageamount" => query.Descending
                    ? plans.OrderByDescending(p => p.CoverageAmount)
                    : plans.OrderBy(p => p.CoverageAmount),

                "premiumamount" => query.Descending
                    ? plans.OrderByDescending(p => p.PremiumAmount)
                    : plans.OrderBy(p => p.PremiumAmount),

                "durationyears" => query.Descending
                    ? plans.OrderByDescending(p => p.DurationInYears)
                    : plans.OrderBy(p => p.DurationInYears),

                "createddate" => query.Descending
                    ? plans.OrderByDescending(p => p.CreatedDate)
                    : plans.OrderBy(p => p.CreatedDate),

                _ => query.Descending
                    ? plans.OrderByDescending(p => p.PlanName)
                    : plans.OrderBy(p => p.PlanName)
            };

            var totalRecords = await plans.CountAsync();

            var records = await plans
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResponse<PolicyPlan>
            {
                Records = records,
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / query.PageSize),
                IsLastPage = query.PageNumber >=
                    (int)Math.Ceiling((double)totalRecords / query.PageSize),
                SortField = query.SortBy,
                SortDirection = query.Descending ? "DESC" : "ASC"
            };
        }

        // Retrieves only active plans.
        public async Task<IEnumerable<PolicyPlan>> GetActivePlansAsync()
        {
            return await _context.PolicyPlans
                .Where(p => p.IsActive)
                .Include(p => p.InsuranceProduct)
                .ToListAsync();
        }

        // Retrieves plans by product id.
        public async Task<IEnumerable<PolicyPlan>> GetByProductIdAsync(int productId)
        {
            return await _context.PolicyPlans
                .Where(p => p.InsuranceProductId == productId)
                .Include(p => p.InsuranceProduct)
                .ToListAsync();
        }

        // Retrieves active plans by product id.
        public async Task<IEnumerable<PolicyPlan>> GetActivePlansByProductIdAsync(int productId)
        {
            return await _context.PolicyPlans
                .Where(p => p.InsuranceProductId == productId && p.IsActive)
                .Include(p => p.InsuranceProduct)
                .ToListAsync();
        }

        // Retrieves plan by id.
        public async Task<PolicyPlan?> GetByIdAsync(int id)
        {
            return await _context.PolicyPlans
                .Include(p => p.InsuranceProduct)
                .Include(p => p.Policies)
                .FirstOrDefaultAsync(p => p.PlanId == id);
        }

        // Retrieves plan by name.
        public async Task<PolicyPlan?> GetByNameAsync(string planName)
        {
            return await _context.PolicyPlans
                .Include(p => p.InsuranceProduct)
                .FirstOrDefaultAsync(p => p.PlanName == planName);
        }

        // Checks if plan name already exists.
        public async Task<bool> PlanNameExistsAsync(string planName)
        {
            return await _context.PolicyPlans
                .AnyAsync(p => p.PlanName == planName);
        }

        // Adds a new policy plan.
        public async Task AddAsync(PolicyPlan plan)
        {
            await _context.PolicyPlans.AddAsync(plan);
        }

        // Updates an existing policy plan.
        public Task UpdateAsync(PolicyPlan plan)
        {
            _context.PolicyPlans.Update(plan);
            return Task.CompletedTask;
        }

        // Soft delete (marks inactive instead of deleting)
        public Task SoftDeleteAsync(PolicyPlan plan)
        {
            plan.IsActive = false;
            _context.PolicyPlans.Update(plan);
            return Task.CompletedTask;
        }

        // Saves changes
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}