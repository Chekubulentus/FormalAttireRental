using AutoMapper;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Customers.DTOs;
using RentalAttireBackend.Application.Disposables.DTOs;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Disposables.ViewArchivedRecords.ViewCustomerRecord
{
    public class ViewCustomerRecord : IViewArchivedEntity
    {
        private readonly ICustomerRepository _repo;
        private readonly IMapper _mapper;
        public string EntityType => "Customer";

        public ViewCustomerRecord(
            ICustomerRepository repo,
            IMapper mapper
            )
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Result<ViewRecordResponse>> GetArchivedRecordAsync(int id, string entityType, CancellationToken ct)
        {
            var customer = await _repo.GetCustomerByIdAsync(id, ct);

            if (customer is null)
                return Result<ViewRecordResponse>.Failure("Record does not exist.");

            var customerDto = _mapper.Map<CustomerDTO>(customer);

            return Result<ViewRecordResponse>.Success(new ViewRecordResponse
            {
                EntityType = "Customer",
                Record = customerDto
            });
        }
    }
}
