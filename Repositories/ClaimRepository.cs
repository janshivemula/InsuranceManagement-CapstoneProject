using InsuranceManagementSystem.Data;
using InsuranceManagementSystem.DTOs.Claim;
using InsuranceManagementSystem.Enums;
using InsuranceManagementSystem.Models;
using InsuranceManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagementSystem.Repositories.Implementations
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly AppDbContext _context;

        public ClaimRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves all claims.
        public async Task<IEnumerable<Claim>> GetAllAsync()
        {
            return await _context.Claims
                .Include(c => c.Customer)
                    .ThenInclude(cu => cu.User)
                .Include(c => c.Policy)
                .Include(c => c.ClaimDocuments)
                .Include(c => c.ClaimStatusHistories)
                    .ThenInclude(h => h.User)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        // Retrieves a claim by its unique identifier.
        public async Task<Claim?> GetByIdAsync(int id)
        {
            return await _context.Claims
                .Include(c => c.Customer)
                    .ThenInclude(cu => cu.User)
                .Include(c => c.Policy)
                .Include(c => c.ClaimDocuments)
                .Include(c => c.ClaimStatusHistories)
                    .ThenInclude(h => h.User)
                .FirstOrDefaultAsync(c => c.ClaimId == id);
        }

        // Retrieves a claim by its business identifier.
        public async Task<Claim?> GetByClaimNumberAsync(string claimNumber)
        {
            return await _context.Claims
                .Include(c => c.Customer)
                    .ThenInclude(cu => cu.User)
                .Include(c => c.Policy)
                .Include(c => c.ClaimDocuments)
                .Include(c => c.ClaimStatusHistories)
                    .ThenInclude(h => h.User)
                .FirstOrDefaultAsync(c => c.ClaimNumber == claimNumber);
        }

        // Retrieves all claims submitted by a specific customer.
        public async Task<IEnumerable<Claim>> GetClaimsByCustomerIdAsync(int customerId)
        {
            return await _context.Claims
                .Where(c => c.CustomerId == customerId)
                .Include(c => c.Customer)
                    .ThenInclude(cu => cu.User)
                .Include(c => c.Policy)
                .Include(c => c.ClaimDocuments)
                .Include(c => c.ClaimStatusHistories)
                    .ThenInclude(h => h.User)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        // Retrieves all claims for a specific policy.
        public async Task<IEnumerable<Claim>> GetClaimsByPolicyIdAsync(int policyId)
        {
            return await _context.Claims
                .Where(c => c.PolicyId == policyId)
                .Include(c => c.Customer)
                    .ThenInclude(cu => cu.User)
                .Include(c => c.Policy)
                .Include(c => c.ClaimDocuments)
                .Include(c => c.ClaimStatusHistories)
                    .ThenInclude(h => h.User)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        // Retrieves all claims with a specific status.
        public async Task<IEnumerable<Claim>> GetClaimsByStatusAsync(ClaimStatus status)
        {
            return await _context.Claims
                .Where(c => c.ClaimStatus == status)
                .Include(c => c.Customer)
                    .ThenInclude(cu => cu.User)
                .Include(c => c.Policy)
                .Include(c => c.ClaimDocuments)
                .Include(c => c.ClaimStatusHistories)
                    .ThenInclude(h => h.User)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        // Adds a new claim.
        public async Task AddAsync(Claim claim)
        {
            await _context.Claims.AddAsync(claim);
        }

        // Updates an existing claim.
        public Task UpdateAsync(Claim claim)
        {
            _context.Claims.Update(claim);
            return Task.CompletedTask;
        }

        // Checks whether a claim number already exists.
        public async Task<bool> ClaimNumberExistsAsync(string claimNumber)
        {
            return await _context.Claims
                .AnyAsync(c => c.ClaimNumber == claimNumber);
        }

        public async Task<(IEnumerable<Claim> Claims, int TotalRecords)> GetPagedClaimsAsync(ClaimPaginationRequestDto request)
        {
            var query = _context.Claims
                .Include(c => c.Customer)
                    .ThenInclude(cu => cu.User)
                .Include(c => c.Policy)
                .AsQueryable();

            // Filtering
            if (request.ClaimStatus.HasValue)
            {
                query = query.Where(c => c.ClaimStatus == request.ClaimStatus.Value);
            }

            if (request.CustomerId.HasValue)
            {
                query = query.Where(c => c.CustomerId == request.CustomerId.Value);
            }

            if (request.PolicyId.HasValue)
            {
                query = query.Where(c => c.PolicyId == request.PolicyId.Value);
            }

            // Sorting
            switch (request.SortBy.ToLower())
            {
                case "claimamount":
                    query = request.SortDirection.ToLower() == "asc"
                        ? query.OrderBy(c => c.ClaimAmount)
                        : query.OrderByDescending(c => c.ClaimAmount);
                    break;

                case "claimnumber":
                    query = request.SortDirection.ToLower() == "asc"
                        ? query.OrderBy(c => c.ClaimNumber)
                        : query.OrderByDescending(c => c.ClaimNumber);
                    break;

                default:
                    query = request.SortDirection.ToLower() == "asc"
                        ? query.OrderBy(c => c.CreatedDate)
                        : query.OrderByDescending(c => c.CreatedDate);
                    break;
            }

            int totalRecords = await query.CountAsync();

            var claims = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return (claims, totalRecords);
        }

        public async Task<bool> IsClaimOwnedByCustomerAsync(int claimId, int customerId)
        {
            return await _context.Claims
                .AnyAsync(c => c.ClaimId == claimId && c.CustomerId == customerId);
        }

        public async Task<bool> HasOpenClaimAsync(int policyId, DateOnly incidentDate)
        {
            return await _context.Claims.AnyAsync(c =>
                c.PolicyId == policyId &&
                c.IncidentDate == incidentDate &&
                c.ClaimStatus != ClaimStatus.Rejected &&
                c.ClaimStatus != ClaimStatus.Approved);
        }

        // Saves all pending changes.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}