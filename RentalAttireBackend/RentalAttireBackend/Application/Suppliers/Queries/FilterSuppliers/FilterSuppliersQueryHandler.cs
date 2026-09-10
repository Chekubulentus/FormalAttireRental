using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Rentals.DTOs;
using RentalAttireBackend.Application.Suppliers.DTOs;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using System.Transactions;

namespace RentalAttireBackend.Application.Suppliers.Queries.FilterSuppliers
{
    public class FilterSuppliersQueryHandler : IRequestHandler<FilterSuppliersQuery, Result<PagedResult<SupplierDTO>>>
    {
        private readonly ISupplierRepository _supplierRepo;
        private readonly IMapper _mapper;

        public FilterSuppliersQueryHandler(
            ISupplierRepository supplierRepo,
            IMapper mapper
            )
        {
            _supplierRepo = supplierRepo;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<SupplierDTO>>> Handle(FilterSuppliersQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<PagedResult<SupplierDTO>>.FailureWithErrorType("Invalid request.", ErrorType.BadRequest);

            var paginatedSuppliers = await _supplierRepo.FilterSuppliersAsync(
                request.SearchQuery,
                request.CurrentPage,
                request.ItemsPerPage,
                cancellationToken
                );

            var paginatedSuppliersDto = _mapper.Map<PagedResult<SupplierDTO>>(paginatedSuppliers);

            var supplierIds = paginatedSuppliersDto.Items.Select(x => x.Id).ToList();

            var activeCounts = await _supplierRepo.GetSupplierActivePOCountByIdAsync(supplierIds, cancellationToken);
            var overdueCounts = await _supplierRepo.GetSupplierOverduePOCountByIdAsync(supplierIds, cancellationToken);

            foreach(var supplier in paginatedSuppliersDto.Items)
            {
                supplier.ActivePOCount = activeCounts.GetValueOrDefault(supplier.Id, 0);
                supplier.OverduePOCount = overdueCounts.GetValueOrDefault(supplier.Id, 0);
            }

            return Result<PagedResult<SupplierDTO>>.Success(paginatedSuppliersDto);
        }
    }
}
