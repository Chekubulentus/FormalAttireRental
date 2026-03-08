using Microsoft.EntityFrameworkCore;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using RentalAttireBackend.Infrastructure.Persistence.DataContext;

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

        public async Task<bool> CreateClotheAsync(Clothe clothe, CancellationToken cancellationToken)
        {
            await _context.Clothes.AddAsync(clothe);
            return await _context.SaveChangesAsync() > 0;
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
