using AutoMapper;
using AutoMapper.Configuration.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Clothes.Queries.FilterClothes
{
    public class FilterClothesQueryHandler : IRequestHandler<FilterClothesQuery, Result<PagedResult<ClotheDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IClotheRepository _clotheRepo;
        private readonly IFileUploadService _fileUpload;

        public FilterClothesQueryHandler(
            IMapper mapper,
            IClotheRepository clotheRepo,
            IFileUploadService fileUpload
            )
        {
            _mapper = mapper;
            _clotheRepo = clotheRepo;
            _fileUpload = fileUpload;
        }
        public async Task<Result<PagedResult<ClotheDTO>>> Handle(FilterClothesQuery  request, CancellationToken cancellationToken)
        {
            if (request.FilterParameters.PaginationParams is null)
                return Result<PagedResult<ClotheDTO>>.Failure("Current & ItemsPerPage is null");

            var clothesResult = await _clotheRepo.FilterClothesAsync(request.FilterParameters, cancellationToken);

            if (clothesResult.Items.Count() == 0 || !clothesResult.Items.Any())
                return Result<PagedResult<ClotheDTO>>.Failure("No clothes found.");

            var clothesResultDto = _mapper.Map<PagedResult<ClotheDTO>>(clothesResult);

            foreach(var clothe in clothesResultDto.Items)
            {
                if (string.IsNullOrEmpty(clothe.ProfileImagePath))
                    continue;

                clothe.ProfileImagePath = _fileUpload.GetFileUrl(clothe.ProfileImagePath);
            }

            return Result<PagedResult<ClotheDTO>>.Success(clothesResultDto);
        }
    }
}
