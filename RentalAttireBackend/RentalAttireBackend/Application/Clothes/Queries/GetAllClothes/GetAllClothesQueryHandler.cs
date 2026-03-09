using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;
using RentalAttireBackend.Infrastructure.Persistence.Services;

namespace RentalAttireBackend.Application.Clothes.Queries.GetAllClothes
{
    public class GetAllClothesQueryHandler : IRequestHandler<GetAllClothesQuery, Result<PagedResult<ClotheDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IClotheRepository _clotheRepo;
        private readonly IFileUploadService _fileUploadService;

        public GetAllClothesQueryHandler(
            IMapper mapper,
            IClotheRepository clotheRepo,
            IFileUploadService fileUploadService
            )
        {
            _clotheRepo = clotheRepo;
            _mapper = mapper;
            _fileUploadService = fileUploadService;
        }

        public async Task<Result<PagedResult<ClotheDTO>>> Handle(GetAllClothesQuery request, CancellationToken cancellationToken)
        {
            if (request.PaginationParams is null)
                return Result<PagedResult<ClotheDTO>>.Failure("Pagination Parameters are required.");

            var clothes = await _clotheRepo.GetAllClothesAsync(request.PaginationParams, cancellationToken);

            if (!clothes.Items.Any() || clothes.Items.Count() == 0)
                return Result<PagedResult<ClotheDTO>>.Failure("No clothes currently registered.");

            var clothesDto = _mapper.Map<PagedResult<ClotheDTO>>(clothes);

            foreach(var clothe in clothesDto.Items)
            {
                if (string.IsNullOrEmpty(clothe.ProfileImagePath))
                    continue;

                clothe.ProfileImagePath = _fileUploadService.GetFileUrl(clothe.ProfileImagePath);
            }

            return Result<PagedResult<ClotheDTO>>.Success(clothesDto);
        }
    }
}
