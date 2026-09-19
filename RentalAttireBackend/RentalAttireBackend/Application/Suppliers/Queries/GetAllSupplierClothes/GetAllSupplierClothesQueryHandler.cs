using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Suppliers.Queries.GetAllSupplierClothes
{
    public class GetAllSupplierClothesQueryHandler : IRequestHandler<GetAllSupplierClothesQuery, Result<List<ClotheDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISupplierRepository _supplierRepository;

        public GetAllSupplierClothesQueryHandler(
            IMapper mapper,
            ISupplierRepository supplierRepository
            )
        {
            _mapper = mapper;
            _supplierRepository = supplierRepository;
        }

        public async Task<Result<List<ClotheDTO>>> Handle(GetAllSupplierClothesQuery request, CancellationToken cancellationToken)
        {
            var clothes = await _supplierRepository.GetAllSupplierClothesByIdAsync(request.SupplierId, cancellationToken);

            var clotheDtos = _mapper.Map<List<ClotheDTO>>(clothes);

            return Result<List<ClotheDTO>>.Success(clotheDtos);
        }
    }
}
