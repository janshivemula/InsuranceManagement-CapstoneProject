using InsuranceManagementSystem.Data;
using InsuranceManagementSystem.DTOs.Policy;
using InsuranceManagementSystem.Enums;
using InsuranceManagementSystem.Models;
using InsuranceManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagementSystem.Repositories.Implementations
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly AppDbContext _context;

        // Constructor for Dependency Injection
        public PolicyRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves all policies.
        public async Task<PagedResult<Policy>> GetAllAsync(PolicyQueryDto query)
        {
            var policies = _context.Policies
                .Include(p => p.Customer)
                    .ThenInclude(c => c.User)
                .Include(p => p.InsuranceProduct)
                .Include(p => p.Plan)
                    .ThenInclude(pp => pp.InsuranceProduct)
                .AsQueryable();

            // Filtering
            if (query.Status.HasValue)
                policies = policies.Where(p => p.PolicyStatus == query.Status.Value);

            if (query.CustomerId.HasValue)
                policies = policies.Where(p => p.CustomerId == query.CustomerId.Value);

            if (query.PlanId.HasValue)
                policies = policies.Where(p => p.PlanId == query.PlanId.Value);

            // Sorting
            policies = query.SortBy.ToLower() switch
            {
                "policynumber" => query.SortDirection.ToLower() == "asc"
                    ? policies.OrderBy(p => p.PolicyNumber)
                    : policies.OrderByDescending(p => p.PolicyNumber),

                "startdate" => query.SortDirection.ToLower() == "asc"
                    ? policies.OrderBy(p => p.StartDate)
                    : policies.OrderByDescending(p => p.StartDate),

                "enddate" => query.SortDirection.ToLower() == "asc"
                    ? policies.OrderBy(p => p.EndDate)
                    : policies.OrderByDescending(p => p.EndDate),

                "status" => query.SortDirection.ToLower() == "asc"
                    ? policies.OrderBy(p => p.PolicyStatus)
                    : policies.OrderByDescending(p => p.PolicyStatus),

                _ => query.SortDirection.ToLower() == "asc"
                    ? policies.OrderBy(p => p.CreatedDate)
                    : policies.OrderByDescending(p => p.CreatedDate)
            };

            int totalRecords = await policies.CountAsync();

            var records = await policies
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResult<Policy>
            {
                Items = records,
                TotalRecords = totalRecords
            };
        }

        // Retrieves all policies for a specific customer.
        public async Task<IEnumerable<Policy>> GetPoliciesByCustomerIdAsync(int customerId)
        {
            return await _context.Policies
                .Where(p => p.CustomerId == customerId)
                .Include(p => p.Customer)
                    .ThenInclude(c => c.User)
                .Include(p => p.InsuranceProduct)
                .Include(p => p.Plan)
                    .ThenInclude(pp => pp.InsuranceProduct)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Policy>> GetPoliciesByUserIdAsync(int userId)
        {
            return await _context.Policies
                .Where(p => p.Customer.UserId == userId)
                .Include(p => p.Customer)
                    .ThenInclude(c => c.User)
                .Include(p => p.InsuranceProduct)
                .Include(p => p.Plan)
                    .ThenInclude(pp => pp.InsuranceProduct)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
        }

        // Retrieves all active policies.
        public async Task<IEnumerable<Policy>> GetActivePoliciesAsync()
        {
            return await _context.Policies
                .Where(p => p.PolicyStatus == PolicyStatus.Active)
                .Include(p => p.Customer)
                    .ThenInclude(c => c.User)
                .Include(p => p.InsuranceProduct)
                .Include(p => p.Plan)
                    .ThenInclude(pp => pp.InsuranceProduct)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
        }

        // Retrieves a policy by its unique identifier.
        public async Task<Policy?> GetByIdAsync(int id)
        {
            return await _context.Policies
                .Include(p => p.Customer)
                    .ThenInclude(c => c.User)
                .Include(p => p.InsuranceProduct)
                .Include(p => p.Plan)
                    .ThenInclude(pp => pp.InsuranceProduct)
                .FirstOrDefaultAsync(p => p.PolicyId == id);
        }

        // Retrieves a policy by its business identifier (Policy Number).
        public async Task<Policy?> GetByPolicyNumberAsync(string policyNumber)
        {
            return await _context.Policies
                .Include(p => p.Customer)
                    .ThenInclude(c => c.User)
                .Include(p => p.InsuranceProduct)
                .Include(p => p.Plan)
                    .ThenInclude(pp => pp.InsuranceProduct)
                .FirstOrDefaultAsync(p => p.PolicyNumber == policyNumber);
        }

        // Adds a new policy.
        public async Task AddAsync(Policy policy)
        {
            await _context.Policies.AddAsync(policy);
        }

        // Updates an existing policy.
        public Task UpdateAsync(Policy policy)
        {
            _context.Policies.Update(policy);
            return Task.CompletedTask;
        }

        // Saves all pending changes to the database.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}