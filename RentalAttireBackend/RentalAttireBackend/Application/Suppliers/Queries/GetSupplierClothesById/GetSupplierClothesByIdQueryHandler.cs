using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Suppliers.Queries.GetSupplierClothesById
{
    public class GetSupplierClothesByIdQueryHandler : IRequestHandler<GetSupplierClothesByIdQuery, Result<PagedResult<ClotheDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISupplierRepository _supplierRepo;

        public GetSupplierClothesByIdQueryHandler(
            IMapper mapper,
            ISupplierRepository supplierRepo
            )
        {
            _mapper = mapper;
            _supplierRepo = supplierRepo;
        }
        public async Task<Result<PagedResult<ClotheDTO>>> Handle(GetSupplierClothesByIdQuery request, CancellationToken cancellationToken)
        {
            var paginatedClothes = await _supplierRepo.GetSupplierClothesByIdAsync(request.Id, request.CurrentPage, request.ItemsPerPage, cancellationToken);

            var paginatedClotheDtos = _mapper.Map<PagedResult<ClotheDTO>>(paginatedClothes);

            return Result<PagedResult<ClotheDTO>>.Success(paginatedClotheDtos);
        }
    }
}
