using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Categories.DTOs;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Disposables.GetAllArchivedEntities;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<PagedResult<CategoryDTO>>>
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

        public async Task<Result<PagedResult<CategoryDTO>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            if (request.PaginationParams.CurrentPage == 0 || request.PaginationParams.ItemsPerPage == 0)
                return Result<PagedResult<CategoryDTO>>.Failure("No category currently registered.");

            try
            {
                var categoriesPagedResult = await _categoryRepo.GetAllCategoriesAsync(request.PaginationParams, cancellationToken);

                if (!categoriesPagedResult.Items.Any() || categoriesPagedResult.Items.Count() == 0)
                    return Result<PagedResult<CategoryDTO>>.Failure("No categories currently registered.");

                var paginatedCategories = _mapper.Map<PagedResult<CategoryDTO>>(categoriesPagedResult.Items);

                return Result<PagedResult<CategoryDTO>>.Success(paginatedCategories);
            }catch(Exception e)
            {
                return Result<PagedResult<CategoryDTO>>.Failure(e.Message);
            }
        }
    }
}
