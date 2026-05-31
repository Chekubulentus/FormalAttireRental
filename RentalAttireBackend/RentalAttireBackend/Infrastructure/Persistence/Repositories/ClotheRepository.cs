using Microsoft.EntityFrameworkCore;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using RentalAttireBackend.Infrastructure.Persistence.DataContext;
using System.Security.Cryptography.X509Certificates;

namespace RentalAttireBackend.Infrastructure.Persistence.Repositories
{
    public class ClotheRepository : IClotheRepository
    {
        private FormalAttireContext _context;

        public ClotheRepository(
            FormalAttireContext context
            )
        {
            _context = context;
        }

        public async Task<bool> ClotheDuplicationValidationAsync(string clotheName, CancellationToken ct)
        {
            return await _context.Clothes
                .AnyAsync(c =>
                c.ClotheName.ToLower().Equals(clotheName.ToLower())
                && c.IsActive && !c.IsDeleted);
        }

        public async Task<int> CreateClotheAsync(Clothe clothe, CancellationToken cancellationToken)
        {
            await _context.Clothes.AddAsync(clothe);
            await _context.SaveChangesAsync();
            return clothe.Id;
        }

        public async Task<PagedResult<Clothe>> FilterClothesAsync(ClothesFIlterParameters filters, CancellationToken cancellationToken)
        {
            Condition? conditionEnum = string.IsNullOrEmpty(filters.Condition)
                ? null : Enum.Parse<Condition>(filters.Condition, true);
            ClotheGender? clotheGenderEnum = string.IsNullOrEmpty(filters.ClotheGender)
                ? null : Enum.Parse<ClotheGender>(filters.ClotheGender, true);

            var clothes = _context.Clothes
                .Include(c => c.Category)
                .AsNoTracking()
                .Where(c =>
                    (
                    string.IsNullOrEmpty(filters.SearchQuery) ||
                    c.ClotheCode.ToLower().Contains(filters.SearchQuery.ToLower()) ||
                    c.ClotheName.ToLower().Contains(filters.SearchQuery.ToLower())
                    )
                    &&
                    (
                    string.IsNullOrEmpty(filters.Condition) ||
                    c.Condition == conditionEnum
                    ) &&
                    (
                    string.IsNullOrEmpty(filters.ClotheGender) ||
                    c.Gender == clotheGenderEnum
                    ) &&
                    (
                    string.IsNullOrEmpty(filters.Category) ||
                    c.Category.CategoryName.ToLower().Equals(filters.Category.ToLower())
                    )
                    &&
                    c.IsActive
                )
                .AsQueryable();

            var totalCount = await clothes.CountAsync();

            var paginatedClothes = await clothes
                .Skip(filters.PaginationParams.Skip)
                .Take(filters.PaginationParams.ItemsPerPage)
                .ToListAsync(cancellationToken);

            return new PagedResult<Clothe>
            {
                Items = paginatedClothes,
                TotalCount = totalCount,
                PageNumber = filters.PaginationParams.CurrentPage,
                PageSize = filters.PaginationParams.ItemsPerPage
            };
        }

        public async Task<List<Clothe>> GetAllArchivedClothesAsync(CancellationToken cancellationToken)
        {
            return await _context.Clothes
                .AsNoTracking()
                .Include(c => c.Category)
                .Where(c => !c.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<Clothe>> GetAllClothesAsync(PaginationParams paginationParams, CancellationToken cancellationToken)
        {
            var clothes = _context.Clothes
                .Include(c => c.Category)
                .AsNoTracking()
                .AsQueryable();

            var totalCount = await clothes.CountAsync();

            var items = await clothes
                .Skip(paginationParams.Skip)
                .Take(paginationParams.ItemsPerPage)
                .ToListAsync(cancellationToken);

            return new PagedResult<Clothe>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = paginationParams.CurrentPage,
                PageSize = paginationParams.ItemsPerPage
            };
        }

        public async Task<Clothe?> GetClotheByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Clothes
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Clothe?> GetClotheByIdNoTrackingAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Clothes
                .Include(c => c.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<bool> UpdateClotheAsync(Clothe clothe, CancellationToken cancellationToken)
        {
            _context.Clothes.Update(clothe);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
