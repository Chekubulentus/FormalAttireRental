using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Employees.DTOs;
using RentalAttireBackend.Domain.Common;
using RentalAttireBackend.Domain.Interfaces;
using System.CodeDom;

namespace RentalAttireBackend.Application.Disposables.GetAllArchivedEntities
{
    public class GetAllArchivedEntitiesQueryHandler : IRequestHandler<GetAllArchivedEntitiesQuery, Result<PagedResult<ArchivedEntityDto>>>
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IUserRepository _userRepo;
        private readonly IPersonRepository _personRepo;
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepo;
        private readonly IClotheRepository _clotheRepo;

        public GetAllArchivedEntitiesQueryHandler
            (
            IEmployeeRepository employeeRepo,
            IPersonRepository personRepo,
            IUserRepository userRepo,
            IMapper mapper,
            ICustomerRepository customerRepo,
            IClotheRepository clotheRepo
            )
        {
            _employeeRepo = employeeRepo;
            _userRepo = userRepo;
            _personRepo = personRepo;
            _mapper = mapper;
            _customerRepo = customerRepo;
            _clotheRepo = clotheRepo;
        }
        public async Task<Result<PagedResult<ArchivedEntityDto>>> Handle(GetAllArchivedEntitiesQuery request, 
            CancellationToken cancellationToken)
        {
            var archivedEntities = new PagedResult<ArchivedEntityDto>();

            //Employees
            archivedEntities.Items.AddRange(
                _mapper.Map<List<ArchivedEntityDto>>(await _employeeRepo.GetAllArchivedEmployeesAsync(cancellationToken))
                );

            //Customers
            archivedEntities.Items.AddRange(
                _mapper.Map<List<ArchivedEntityDto>>(await _customerRepo.GetAllArchivedCustomersAsync(cancellationToken))
                );

            //Clothes
            archivedEntities.Items.AddRange(
                _mapper.Map<List<ArchivedEntityDto>>(await _clotheRepo.GetAllArchivedClothesAsync(cancellationToken))
                ); 

            archivedEntities.TotalCount = archivedEntities.Items.Count();
            archivedEntities.PageNumber = request.PaginationParams.CurrentPage;
            archivedEntities.PageSize = request.PaginationParams.ItemsPerPage;

            if (archivedEntities.TotalCount == 0)
                return Result<PagedResult<ArchivedEntityDto>>.Failure("No archived record registered yet.");


            return Result<PagedResult<ArchivedEntityDto>>.Success(archivedEntities);
        }
    }
}
