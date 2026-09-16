using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Suppliers.DTOs;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Suppliers.Queries.GetSupplierById
{
    public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, Result<SupplierDTO>>
    {
        private readonly ISupplierRepository _supplierRepo;
        private readonly IMapper _mapper;

        public GetSupplierByIdQueryHandler(
            ISupplierRepository supplierRepo,
            IMapper mapper
            )
        {
            _supplierRepo = supplierRepo;
            _mapper = mapper;
        }

        public async Task<Result<SupplierDTO>> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<SupplierDTO>.FailureWithErrorType("Invalid request", ErrorType.BadRequest);

            var supplier = await _supplierRepo.GetSupplierByIdAsync(request.Id, cancellationToken);

            if (supplier is null)
                return Result<SupplierDTO>.FailureWithErrorType("Supplier record does not exist", ErrorType.NotFound);

            var supplierDto = _mapper.Map<SupplierDTO>(supplier);

            return Result<SupplierDTO>.Success(supplierDto);
        }
    }
}
