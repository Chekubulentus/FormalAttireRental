using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Categories.DTOs;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDTO>>
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepo;

        public GetCategoryByIdQueryHandler(
            IMapper mapper,
            ICategoryRepository categoryRepo
            )
        {
            _mapper = mapper;
            _categoryRepo = categoryRepo;
        }

        public async Task<Result<CategoryDTO>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
                return Result<CategoryDTO>.Failure("Invalid category identifier.");
            try
            {
                var category = await _categoryRepo.GetCategoryByIdAsync(request.Id, cancellationToken);

                if (category is null)
                    return Result<CategoryDTO>.Failure("Category does not exist.");

                var categoryDto = _mapper.Map<CategoryDTO>(category);

                return Result<CategoryDTO>.Success(categoryDto);
            }
            catch(Exception e)
            {
                return Result<CategoryDTO>.Failure(e.Message);
            }
        }
    }
}
