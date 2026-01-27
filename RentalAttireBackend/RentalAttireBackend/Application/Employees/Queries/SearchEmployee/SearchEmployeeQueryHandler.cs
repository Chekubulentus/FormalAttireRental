using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Employees.Queries.SearchEmployee
{
    public class SearchEmployeeQueryHandler : IRequestHandler<SearchEmployeeQuery, Result<PagedResult<Employee>>>
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IMapper _mapper;

        public SearchEmployeeQueryHandler
            (
            IEmployeeRepository employeeRepo,
            IMapper mapper
            )
        {
            _employeeRepo = employeeRepo;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<Employee>>> Handle(SearchEmployeeQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<PagedResult<Employee>>.Failure("Invalid request. Please try again.");

            try
            {
                var query = await _employeeRepo.SearchEmployeeAsync(request.SearchQuery, request.PaginationParams, cancellationToken);

                if (!query.Items.Any() || query.Items.Count() == 0)
                    return Result<PagedResult<Employee>>.Failure("Employee does not exist.");

                return Result<PagedResult<Employee>>.Success(query);
            }catch(Exception e)
            {
                return Result<PagedResult<Employee>>.Failure(e.Message);
            }
        }
    }
}
