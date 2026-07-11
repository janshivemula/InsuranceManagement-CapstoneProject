using InsuranceManagementSystem.Data;
using InsuranceManagementSystem.DTOs.Customer;
using InsuranceManagementSystem.Models;
using InsuranceManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagementSystem.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        // Constructor for Dependency Injection
        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves all customers.
        public async Task<PagedResult<Customer>> GetAllAsync(CustomerQueryDto query)
        {
            var customers = _context.Customers
                .Include(c => c.User)
                .Include(c => c.Policies)
                .Include(c => c.PremiumPayments)
                .AsQueryable();

            // Filtering
            if (query.IsActive.HasValue)
            {
                customers = customers.Where(c => c.IsActive == query.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.City))
            {
                customers = customers.Where(c => c.City.ToLower() == query.City.Trim().ToLower());
            }

            if (!string.IsNullOrWhiteSpace(query.State))
            {
                customers = customers.Where(c => c.State.ToLower() == query.State.Trim().ToLower());
            }

            // Sorting
            customers = (query.SortBy.ToLower(), query.SortDirection.ToLower()) switch
            {
                ("city", "asc") => customers.OrderBy(c => c.City),
                ("city", "desc") => customers.OrderByDescending(c => c.City),

                ("state", "asc") => customers.OrderBy(c => c.State),
                ("state", "desc") => customers.OrderByDescending(c => c.State),

                ("createddate", "asc") => customers.OrderBy(c => c.CreatedDate),
                ("createddate", "desc") => customers.OrderByDescending(c => c.CreatedDate),

                _ => throw new InvalidOperationException("Invalid sorting options.")
            };

            var totalRecords = await customers.CountAsync();

            var records = await customers
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResult<Customer>
            {
                Items = records,
                TotalRecords = totalRecords
            };
        }

        // Retrieves all active customers.
        public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
        {
            return await _context.Customers
                .Where(c => c.IsActive)
                .Include(c => c.User)
                .OrderBy(c => c.CustomerId)
                .ToListAsync();
        }

        // Retrieves a customer by its unique identifier.
        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.User)
                .Include(c => c.Policies)
                .Include(c => c.PremiumPayments)
                .FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        // Retrieves a customer by the associated user id.
        public async Task<Customer?> GetByUserIdAsync(int userId)
        {
            return await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        // Adds a new customer.
        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }

        // Updates an existing customer.
        public Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            return Task.CompletedTask;
        }

        // Saves all pending changes to the database.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}