using InsuranceManagementSystem.Data;
using InsuranceManagementSystem.DTOs.Common;
using InsuranceManagementSystem.Models;
using InsuranceManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagementSystem.Repositories.Implementations
{
    public class InsuranceProductRepository : IInsuranceProductRepository
    {
        private readonly AppDbContext _context;

        // Constructor for Dependency Injection
        public InsuranceProductRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves all insurance products
        public async Task<PagedResponse<InsuranceProduct>> GetAllAsync(ProductQueryDto query)
        {
            var products = _context.InsuranceProducts
                .Include(p => p.PolicyPlans)
                .Include(p => p.Policies)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                products = products.Where(p => p.ProductName.Contains(query.Search));
            }

            // Filter Product Type
            if (query.ProductType.HasValue)
            {
                products = products.Where(p => p.ProductType == query.ProductType.Value);
            }

            // Filter Active
            if (query.IsActive.HasValue)
            {
                products = products.Where(p => p.IsActive == query.IsActive.Value);
            }

            // Sorting
            products = query.SortBy.ToLower() switch
            {
                "createddate" => query.Descending
                    ? products.OrderByDescending(x => x.CreatedDate)
                    : products.OrderBy(x => x.CreatedDate),

                "producttype" => query.Descending
                    ? products.OrderByDescending(x => x.ProductType)
                    : products.OrderBy(x => x.ProductType),

                _ => query.Descending
                    ? products.OrderByDescending(x => x.ProductName)
                    : products.OrderBy(x => x.ProductName)
            };

            var totalRecords = await products.CountAsync();

            var records = await products
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResponse<InsuranceProduct>
            {
                Records = records,
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / query.PageSize),
                IsLastPage = query.PageNumber >= (int)Math.Ceiling((double)totalRecords / query.PageSize),
                SortField = query.SortBy,
                SortDirection = query.Descending ? "DESC" : "ASC"
            };
        }

        // Retrieves only active products
        public async Task<IEnumerable<InsuranceProduct>> GetActiveProductsAsync()
        {
            return await _context.InsuranceProducts
                .Where(p => p.IsActive)
                .Include(p => p.PolicyPlans)
                .ToListAsync();
        }

        // Retrieves product by Id
        public async Task<InsuranceProduct?> GetByIdAsync(int id)
        {
            return await _context.InsuranceProducts
                .Include(p => p.PolicyPlans)
                .Include(p => p.Policies)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        // Retrieves product by name
        public async Task<InsuranceProduct?> GetByNameAsync(string productName)
        {
            return await _context.InsuranceProducts
                .Include(p => p.PolicyPlans)
                .FirstOrDefaultAsync(p => p.ProductName == productName);
        }

        // Checks if product name exists
        public async Task<bool> ProductNameExistsAsync(string productName)
        {
            return await _context.InsuranceProducts
                .AnyAsync(p => p.ProductName == productName);
        }

        // Adds new product
        public async Task AddAsync(InsuranceProduct product)
        {
            await _context.InsuranceProducts.AddAsync(product);
        }

        // Updates product
        public Task UpdateAsync(InsuranceProduct product)
        {
            _context.InsuranceProducts.Update(product);
            return Task.CompletedTask;
        }

        // Soft delete (mark inactive)
        public Task SoftDeleteAsync(InsuranceProduct product)
        {
            product.IsActive = false;
            _context.InsuranceProducts.Update(product);
            return Task.CompletedTask;
        }

        // Save changes
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}