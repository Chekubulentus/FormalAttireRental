using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Employees.DTOs;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Employees.Queries.SearchEmployee
{
    public class SearchEmployeeQueryHandler : IRequestHandler<SearchEmployeeQuery, Result<PagedResult<EmployeeDTO>>>
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IMapper _mapper;
        private readonly IFileUploadService _fileUpload;

        public SearchEmployeeQueryHandler
            (
            IEmployeeRepository employeeRepo,
            IMapper mapper,
            IFileUploadService fileUpload
            )
        {
            _employeeRepo = employeeRepo;
            _mapper = mapper;
            _fileUpload = fileUpload;
        }

        public async Task<Result<PagedResult<EmployeeDTO>>> Handle(SearchEmployeeQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<PagedResult<EmployeeDTO>>.Failure("Invalid request. Please try again.");

            try
            {
                var query = await _employeeRepo.SearchEmployeeAsync(request.SearchQuery, request.PaginationParams, cancellationToken);

                if (!query.Items.Any() || query.Items.Count() == 0)
                    return Result<PagedResult<EmployeeDTO>>.Failure("Employee does not exist.");

                foreach(var employee in query.Items)
                {
                    var person = employee.User.Person;

                    if (person.ProfileImagePath is null)
                        continue;

                    person.ProfileImagePath = _fileUpload.GetFileUrl(person.ProfileImagePath);
                }

                var employeesDto = _mapper.Map<PagedResult<EmployeeDTO>>(query);

                return Result<PagedResult<EmployeeDTO>>.Success(employeesDto);
            }catch(Exception e)
            {
                return Result<PagedResult<EmployeeDTO>>.Failure(e.Message);
            }
        }
    }
}
