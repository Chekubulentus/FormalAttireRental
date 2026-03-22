using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Employees.Queries.SearchEmployee;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using RentalAttireBackend.Infrastructure.Persistence.DataContext;

namespace RentalAttireBackend.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private FormalAttireContext _context;

        public CategoryRepository(
            FormalAttireContext context
            )
        {
            _context = context;
        }

        public async Task<bool> CategoryDuplicateValidationAsync(
            string categoryCode, 
            string categoryName, 
            CancellationToken cancellationToken)
        {
            return await _context.Categories
                .AnyAsync(c =>
                    (
                    c.CategoryCode.ToLower().Equals(categoryCode.ToLower()) ||
                    c.CategoryName.ToLower().Equals(categoryName.ToLower())
                    )
                    &&
                    c.IsActive
                );
        }

        public async Task<bool> CreateCategoryAsync(Category category, CancellationToken cancellationToken)
        {
            await _context.Categories.AddAsync(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedResult<Category>> FilterCategoriesAsync(
            string searchQuery,
            PaginationParams paginationParams,
            CancellationToken cancellationToken)
        {
            var categories = _context.Categories
                .AsNoTracking()
                .Where(c =>
                    (
                    string.IsNullOrEmpty(searchQuery) ||
                    c.CategoryCode.ToLower().Contains(searchQuery.ToLower()) ||
                    c.CategoryName.ToLower().Contains(searchQuery.ToLower())
                    )
                    &&
                    c.IsActive
                ).AsQueryable();

            var totalCount = await categories.CountAsync();

            var paginatedItems = await categories
                .Skip(paginationParams.Skip)
                .Take(paginationParams.ItemsPerPage)
                .ToListAsync(cancellationToken);

            return new PagedResult<Category>
            {
                Items = paginatedItems,
                TotalCount = totalCount,
                PageNumber = paginationParams.CurrentPage,
                PageSize = paginationParams.ItemsPerPage
            };
        }

        public async Task<PagedResult<Category>> GetAllCategoriesAsync(PaginationParams paginationParams, CancellationToken cancellationToken)
        {
            var categories = _context.Categories.AsNoTracking();

            var totalCount = await categories.CountAsync();

            var items = await categories
                .Skip(paginationParams.Skip)
                .Take(paginationParams.ItemsPerPage)
                .ToListAsync(cancellationToken);

            return new PagedResult<Category>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = paginationParams.CurrentPage,
                PageSize = paginationParams.ItemsPerPage,
            };
        }

        public async Task<Category?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Category?> GetCategoryByIdNoTrackingAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Category?> GetCategoryByNameAsync(string categoryName, CancellationToken cancellationToken)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName.Equals(categoryName));
        }

        public async Task<PagedResult<Category>> SearchCategoryByNameAsync(string searchQuery, PaginationParams paginationParams, CancellationToken cancellationToken)
        {
            var categories = _context.Categories
                .AsNoTracking()
                .Where(c =>
                c.CategoryName.ToLower().Contains(searchQuery) ||
                c.CategoryCode.ToLower().Contains(searchQuery)
                );

            var totalCount = await categories.CountAsync();

            var items = await categories
                .Skip(paginationParams.Skip)
                .Take(paginationParams.ItemsPerPage)
                .ToListAsync(cancellationToken);

            return new PagedResult<Category>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = paginationParams.CurrentPage,
                PageSize = paginationParams.ItemsPerPage
            };
        }

        public async Task<bool> UpdateCategoryAsync(Category category, CancellationToken cancellationToken)
        {
            _context.Categories.Update(category);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
