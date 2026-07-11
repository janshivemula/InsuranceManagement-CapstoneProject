using InsuranceManagementSystem.Data;
using InsuranceManagementSystem.Models;
using InsuranceManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagementSystem.Repositories.Implementations
{
    public class ClaimStatusHistoryRepository : IClaimStatusHistoryRepository
    {
        private readonly AppDbContext _context;

        public ClaimStatusHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves all claim status history records.
        public async Task<IEnumerable<ClaimStatusHistory>> GetAllAsync()
        {
            return await _context.ClaimStatusHistories
                .Include(h => h.Claim)
                .Include(h => h.User)
                .OrderByDescending(h => h.UpdatedDate)
                .ToListAsync();
        }

        // Retrieves a claim status history record by its HistoryId.
        public async Task<ClaimStatusHistory?> GetByIdAsync(int historyId)
        {
            return await _context.ClaimStatusHistories
                .Include(h => h.Claim)
                .Include(h => h.User)
                .FirstOrDefaultAsync(h => h.HistoryId == historyId);
        }

        // Retrieves all status history records for a specific claim.
        public async Task<IEnumerable<ClaimStatusHistory>> GetByClaimIdAsync(int claimId)
        {
            return await _context.ClaimStatusHistories
                .Where(h => h.ClaimId == claimId)
                .Include(h => h.Claim)
                .Include(h => h.User)
                .OrderByDescending(h => h.UpdatedDate)
                .ToListAsync();
        }

        // Adds a new claim status history record.
        public async Task AddAsync(ClaimStatusHistory claimStatusHistory)
        {
            await _context.ClaimStatusHistories.AddAsync(claimStatusHistory);
        }

        // Saves all pending changes to the database.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}