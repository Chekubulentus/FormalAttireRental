using AutoMapper;
using RentalAttireBackend.Application.Categories.DTOs;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Disposables.DTOs;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Disposables.ViewArchivedRecords.ViewCategoryRecord
{
    public class ViewCategoryRecord : IViewArchivedEntity
    {
        public string EntityType => "Category";
        private readonly ICategoryRepository _repo;
        private readonly IMapper _mapper;

        public ViewCategoryRecord(
            ICategoryRepository categoryRepository,
            IMapper mapper
            )
        {
            _repo = categoryRepository;
            _mapper = mapper;
        }

        public async Task<Result<ViewRecordResponse>> GetArchivedRecordAsync(int id, string entityType, CancellationToken ct)
        {
            var category = await _repo.GetCategoryByIdAsync(id, ct);

            if (category is null)
                return Result<ViewRecordResponse>.Failure("Record does not exist.");

            var categoryDto = _mapper.Map<CategoryDTO>(category);

            return Result<ViewRecordResponse>.Success(new ViewRecordResponse { 
            EntityType = "Category",
            Record = categoryDto
            });
        }
    }
}
