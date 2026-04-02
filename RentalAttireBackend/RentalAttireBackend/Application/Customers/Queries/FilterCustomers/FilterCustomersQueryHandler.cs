using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Customers.DTOs;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Customers.Queries.FilterCustomers
{
    public class FilterCustomersQueryHandler : IRequestHandler<FilterCustomersQuery, Result<PagedResult<CustomerDTO>>>
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IMapper _mapper;
        private readonly IFileUploadService _fileUploadService;

        public FilterCustomersQueryHandler(
            ICustomerRepository customerRepo,
            IMapper mapper,
            IFileUploadService fileUploadService
            )
        {
            _customerRepo = customerRepo;
            _mapper = mapper;
            _fileUploadService = fileUploadService;
        }
        public async Task<Result<PagedResult<CustomerDTO>>> Handle(FilterCustomersQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<PagedResult<CustomerDTO>>.Failure("Invalid request.");

            var customers = await _customerRepo.FilterCustomersAsync(
                request.PaginationParams,
                request.SearchQuery,
                cancellationToken
                );

            if (!customers.Items.Any() || customers.Items.Count() == 0)
                return Result<PagedResult<CustomerDTO>>.Failure("No customers currently found.");

            var customerDtos = _mapper.Map<PagedResult<CustomerDTO>>(customers);

            foreach(var customer in customerDtos.Items)
            {
                if (string.IsNullOrEmpty(customer.Person.ProfileImagePath))
                    continue;

                if (customer.IsGoogleAccount == true)
                    continue;

                customer.Person.ProfileImagePath = _fileUploadService.GetFileUrl(customer.Person.ProfileImagePath);
            }

            return Result<PagedResult<CustomerDTO>>.Success(customerDtos);
        }
    }
}
