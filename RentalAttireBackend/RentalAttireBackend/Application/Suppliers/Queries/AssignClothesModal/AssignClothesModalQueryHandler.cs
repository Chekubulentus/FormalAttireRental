using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Suppliers.DTOs;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Suppliers.Queries.AssignClothesModal
{
    public class AssignClothesModalQueryHandler : IRequestHandler<AssignClothesModalQuery, Result<AssignSupplierClothesModalResponse>>
    {
        private ISupplierRepository _supplierRepo;
        private IMapper _mapper;

        public AssignClothesModalQueryHandler(
            ISupplierRepository supplierRepo,
            IMapper mapper
            )
        {
            _supplierRepo = supplierRepo;
            _mapper = mapper;
        }

        public async Task<Result<AssignSupplierClothesModalResponse>> Handle(AssignClothesModalQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<AssignSupplierClothesModalResponse>.FailureWithErrorType("Invalid request.", ErrorType.BadRequest);

            var assignableClothes = await _supplierRepo.FilterAssignableClothesAsync(
                request.SupplierId,
                request.SearchQuery,
                request.Category,
                request.Gender,
                request.CurrentPage,
                request.ItemsPerPage,
                cancellationToken
                );

            var assignableClotheDtos = _mapper.Map<List<ClotheDTO>>(assignableClothes);

            var assignedClothesCount = await _supplierRepo.GetSupplierAssignClothesCount(request.SupplierId, cancellationToken);
            var unassignedClothesCount = await _supplierRepo.GetAllUnassignedClothesAsync(cancellationToken);
            var totalCount = assignableClothes.Count();

            return Result<AssignSupplierClothesModalResponse>.Success(new AssignSupplierClothesModalResponse
            {
                Clothes = assignableClotheDtos,
                TotalCount = totalCount,
                AssignedClothesCount = assignedClothesCount,
                UnassignedClothesCount = unassignedClothesCount,
                CurrentPage = request.CurrentPage,
                ItemsPerPage = request.ItemsPerPage
            });
        }
    }
}
