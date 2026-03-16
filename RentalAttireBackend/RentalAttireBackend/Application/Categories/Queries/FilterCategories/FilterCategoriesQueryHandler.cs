using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Categories.DTOs;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Categories.Queries.FilterCategories
{
    public class FilterCategoriesQueryHandler : IRequestHandler<FilterCategoriesQuery, Result<PagedResult<CategoryDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepo;

        public FilterCategoriesQueryHandler(
            IMapper mapper,
            ICategoryRepository categoryRepo
            )
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<CategoryDTO>>> Handle(FilterCategoriesQuery request, CancellationToken cancellationToken)
        {
            if (request.PaginationParams is null)
                return Result<PagedResult<CategoryDTO>>.Failure("Pagination parameters are null.");

            if(request.PaginationParams.CurrentPage == 0)
                return Result<PagedResult<CategoryDTO>>.Failure("Current page is zero.");

            if(request.PaginationParams.ItemsPerPage == 0)
                return Result<PagedResult<CategoryDTO>>.Failure("Items per page is zero.");

            var categories = await _categoryRepo.FilterCategoriesAsync(
                request.CategoryCode,
                request.CategoryName,
                request.PaginationParams,
                cancellationToken
                );

            if (!categories.Items.Any() || categories.Items.Count() == 0)
                return Result<PagedResult<CategoryDTO>>.Failure("No categories found.");

            var categoryDtos = _mapper.Map<PagedResult<CategoryDTO>>(categories);

            return Result<PagedResult<CategoryDTO>>.Success(categoryDtos);
        }
    }
}
