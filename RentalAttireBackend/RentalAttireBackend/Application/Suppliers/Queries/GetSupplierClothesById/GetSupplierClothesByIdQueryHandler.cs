using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Suppliers.Queries.GetSupplierClothesById
{
    public class GetSupplierClothesByIdQueryHandler : IRequestHandler<GetSupplierClothesByIdQuery, Result<PagedResult<ClotheDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISupplierRepository _supplierRepo;
        private readonly IFileUploadService _fileUploadService;

        public GetSupplierClothesByIdQueryHandler(
            IMapper mapper,
            ISupplierRepository supplierRepo,
            IFileUploadService fileUploadService
            )
        {
            _mapper = mapper;
            _supplierRepo = supplierRepo;
            _fileUploadService = fileUploadService;
        }
        public async Task<Result<PagedResult<ClotheDTO>>> Handle(GetSupplierClothesByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
                return Result<PagedResult<ClotheDTO>>.FailureWithErrorType("Invalid supplier identifier", ErrorType.BadRequest);

            var supplier = await _supplierRepo.GetSupplierWithNoRelationshipsByIdAsync(request.Id, cancellationToken);

            if (supplier is null)
                return Result<PagedResult<ClotheDTO>>.FailureWithErrorType("Supplier record does not exist", ErrorType.NotFound);

            var paginatedClothes = await _supplierRepo.GetSupplierClothesByIdAsync(
                request.Id, 
                request.SearchQuery,
                request.Category,
                request.Availability,
                request.Gender,
                request.CurrentPage, 
                request.ItemsPerPage, 
                cancellationToken);

            var paginatedClotheDtos = _mapper.Map<PagedResult<ClotheDTO>>(paginatedClothes);

            foreach(var clothe in paginatedClotheDtos.Items)
            {
                if(string.IsNullOrEmpty(clothe.ProfileImagePath))
                    continue;

                clothe.ProfileImagePath = _fileUploadService.GetFileUrl(clothe.ProfileImagePath);
            }

            return Result<PagedResult<ClotheDTO>>.Success(paginatedClotheDtos);
        }
    }
}
