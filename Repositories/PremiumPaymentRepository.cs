using InsuranceManagementSystem.Data;
using InsuranceManagementSystem.DTOs.Common;
using InsuranceManagementSystem.Models;
using InsuranceManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagementSystem.Repositories.Implementations
{
    public class PremiumPaymentRepository : IPremiumPaymentRepository
    {
        private readonly AppDbContext _context;

        // Constructor for Dependency Injection
        public PremiumPaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves all premium payments.
        public async Task<PagedResponse<PremiumPayment>> GetAllAsync(PaginationRequestDto paginationDto)
        {
            IQueryable<PremiumPayment> query = _context.PremiumPayments
                .Include(p => p.Policy)
                .Include(p => p.Customer);

            // Filter by Payment Status
            if (paginationDto.PaymentStatus.HasValue)
            {
                query = query.Where(p => p.PaymentStatus == paginationDto.PaymentStatus.Value);
            }

            // Filter by Payment Mode
            if (paginationDto.PaymentMode.HasValue)
            {
                query = query.Where(p => p.PaymentMode == paginationDto.PaymentMode.Value);
            }

            // Sorting
            switch (paginationDto.SortBy.ToLower())
            {
                case "amount":

                    query = paginationDto.SortDirection.ToLower() == "asc"
                        ? query.OrderBy(p => p.Amount)
                        : query.OrderByDescending(p => p.Amount);

                    break;

                case "paymentstatus":

                    query = paginationDto.SortDirection.ToLower() == "asc"
                        ? query.OrderBy(p => p.PaymentStatus)
                        : query.OrderByDescending(p => p.PaymentStatus);

                    break;

                case "paymentmode":

                    query = paginationDto.SortDirection.ToLower() == "asc"
                        ? query.OrderBy(p => p.PaymentMode)
                        : query.OrderByDescending(p => p.PaymentMode);

                    break;

                default:

                    query = paginationDto.SortDirection.ToLower() == "asc"
                        ? query.OrderBy(p => p.PaymentDate)
                        : query.OrderByDescending(p => p.PaymentDate);

                    break;
            }

            int totalRecords = await query.CountAsync();

            var records = await query
                .Skip((paginationDto.PageNumber - 1) * paginationDto.PageSize)
                .Take(paginationDto.PageSize)
                .ToListAsync();

            return new PagedResponse<PremiumPayment>
            {
                Records = records,
                CurrentPage = paginationDto.PageNumber,
                PageSize = paginationDto.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)paginationDto.PageSize),
                IsLastPage = paginationDto.PageNumber * paginationDto.PageSize >= totalRecords,
                SortField = paginationDto.SortBy,
                SortDirection = paginationDto.SortDirection
            };
        }

        // Retrieves all payments for a specific policy.
        public async Task<PagedResponse<PremiumPayment>> GetPaymentsByPolicyIdAsync(int policyId, PaginationRequestDto paginationDto)
        {
            IQueryable<PremiumPayment> query = _context.PremiumPayments
                .Where(p => p.PolicyId == policyId)
                .Include(p => p.Policy)
                .Include(p => p.Customer);

            // Filter by Payment Status
            if (paginationDto.PaymentStatus.HasValue)
            {
                query = query.Where(p => p.PaymentStatus == paginationDto.PaymentStatus.Value);
            }

            // Filter by Payment Mode
            if (paginationDto.PaymentMode.HasValue)
            {
                query = query.Where(p => p.PaymentMode == paginationDto.PaymentMode.Value);
            }

            // Sorting
            switch (paginationDto.SortBy.ToLower())
            {
                case "amount":

                    query = paginationDto.SortDirection.ToLower() == "asc"
                        ? query.OrderBy(p => p.Amount)
                        : query.OrderByDescending(p => p.Amount);

                    break;

                case "paymentstatus":

                    query = paginationDto.SortDirection.ToLower() == "asc"
                        ? query.OrderBy(p => p.PaymentStatus)
                        : query.OrderByDescending(p => p.PaymentStatus);

                    break;

                case "paymentmode":

                    query = paginationDto.SortDirection.ToLower() == "asc"
                        ? query.OrderBy(p => p.PaymentMode)
                        : query.OrderByDescending(p => p.PaymentMode);

                    break;

                default:

                    query = paginationDto.SortDirection.ToLower() == "asc"
                        ? query.OrderBy(p => p.PaymentDate)
                        : query.OrderByDescending(p => p.PaymentDate);

                    break;
            }

            int totalRecords = await query.CountAsync();

            var records = await query
                .Skip((paginationDto.PageNumber - 1) * paginationDto.PageSize)
                .Take(paginationDto.PageSize)
                .ToListAsync();

            return new PagedResponse<PremiumPayment>
            {
                Records = records,
                CurrentPage = paginationDto.PageNumber,
                PageSize = paginationDto.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)paginationDto.PageSize),
                IsLastPage = paginationDto.PageNumber * paginationDto.PageSize >= totalRecords,
                SortField = paginationDto.SortBy,
                SortDirection = paginationDto.SortDirection
            };
        }

        // Retrieves all payments made by a specific customer.
        public async Task<PagedResponse<PremiumPayment>> GetPaymentsByCustomerIdAsync(int customerId, PaginationRequestDto paginationDto)
        {
            IQueryable<PremiumPayment> query = _context.PremiumPayments
                .Where(p => p.CustomerId == customerId)
                .Include(p => p.Policy)
                .Include(p => p.Customer);

            // Filter by Payment Status
            if (paginationDto.PaymentStatus.HasValue)
            {
                query = query.Where(p => p.PaymentStatus == paginationDto.PaymentStatus.Value);
            }

            // Filter by Payment Mode
            if (paginationDto.PaymentMode.HasValue)
            {
                query = query.Where(p => p.PaymentMode == paginationDto.PaymentMode.Value);
            }

            // Sorting
            switch (paginationDto.SortBy.ToLower())
            {
                case "amount":

                    query = paginationDto.SortDirection.ToLower() == "asc"
                        ? query.OrderBy(p => p.Amount)
                        : query.OrderByDescending(p => p.Amount);

                    break;

                case "paymentstatus":

                    query = paginationDto.SortDirection.ToLower() == "asc"
                        ? query.OrderBy(p => p.PaymentStatus)
                        : query.OrderByDescending(p => p.PaymentStatus);

                    break;

                case "paymentmode":

                    query = paginationDto.SortDirection.ToLower() == "asc"
                        ? query.OrderBy(p => p.PaymentMode)
                        : query.OrderByDescending(p => p.PaymentMode);

                    break;

                default:

                    query = paginationDto.SortDirection.ToLower() == "asc"
                        ? query.OrderBy(p => p.PaymentDate)
                        : query.OrderByDescending(p => p.PaymentDate);

                    break;
            }

            int totalRecords = await query.CountAsync();

            var records = await query
                .Skip((paginationDto.PageNumber - 1) * paginationDto.PageSize)
                .Take(paginationDto.PageSize)
                .ToListAsync();

            return new PagedResponse<PremiumPayment>
            {
                Records = records,
                CurrentPage = paginationDto.PageNumber,
                PageSize = paginationDto.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)paginationDto.PageSize),
                IsLastPage = paginationDto.PageNumber * paginationDto.PageSize >= totalRecords,
                SortField = paginationDto.SortBy,
                SortDirection = paginationDto.SortDirection
            };
        }

        // Retrieves a premium payment by its unique identifier.
        public async Task<PremiumPayment?> GetByIdAsync(int id)
        {
            return await _context.PremiumPayments
                .Include(p => p.Policy)
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(p => p.PaymentId == id);
        }

        // Retrieves a premium payment using its transaction reference.
        public async Task<PremiumPayment?> GetByTransactionReferenceAsync(string transactionReference)
        {
            return await _context.PremiumPayments
                .Include(p => p.Policy)
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(p => p.TransactionReference == transactionReference);
        }

        // Adds a new premium payment.
        public async Task AddAsync(PremiumPayment premiumPayment)
        {
            await _context.PremiumPayments.AddAsync(premiumPayment);
        }

        // Updates an existing premium payment.
        public Task UpdateAsync(PremiumPayment premiumPayment)
        {
            _context.PremiumPayments.Update(premiumPayment);
            return Task.CompletedTask;
        }

        // Saves all pending changes to the database.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}