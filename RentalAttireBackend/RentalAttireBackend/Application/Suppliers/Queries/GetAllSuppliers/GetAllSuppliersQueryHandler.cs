using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Suppliers.DTOs;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.CompilerServices;

namespace RentalAttireBackend.Application.Suppliers.Queries.GetAllSuppliers
{
    public class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, Result<List<SupplierSummaryDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISupplierRepository _supplierRepository;

        public GetAllSuppliersQueryHandler(
            IMapper mapper,
            ISupplierRepository supplierRepository
            )
        {
            _mapper = mapper;
            _supplierRepository = supplierRepository;
        }

        public async Task<Result<List<SupplierSummaryDTO>>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
        {
            var suppliers = await _supplierRepository.GetAllActiveSuppliersASync(cancellationToken);

            var suppliersDto = _mapper.Map<List<SupplierSummaryDTO>>(suppliers);

            var supplierIds = suppliersDto.Select(x => x.Id).ToList();

            var supplierAssignedClothesCount = await _supplierRepository.GetAllSuppliersAssignedClothesCount(supplierIds, cancellationToken);

            foreach(var supplier in suppliersDto)
            {
                supplier.AssignedClothesCount = supplierAssignedClothesCount.GetValueOrDefault(supplier.Id, 0);
            }

            return Result<List<SupplierSummaryDTO>>.Success(suppliersDto);
        }
    }
}
