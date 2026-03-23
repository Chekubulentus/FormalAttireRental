using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Categories.DTOs;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Disposables.GetAllArchivedEntities;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<List<CategoryDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepo;

        public GetAllCategoriesQueryHandler(
            IMapper mapper,
            ICategoryRepository categoryRepo
            )
        {
            _mapper = mapper;
            _categoryRepo = categoryRepo;
        }

        public async Task<Result<List<CategoryDTO>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepo.GetAllCategoriesAsync(cancellationToken);

            if (!categories.Any() || categories.Count() == 0)
                return Result<List<CategoryDTO>>.Failure("No categories found.");

            var categoryDtos = _mapper.Map<List<CategoryDTO>>(categories);

            return Result<List<CategoryDTO>>.Success(categoryDtos);
        }
    }
}
