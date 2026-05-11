using AutoMapper;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Disposables.DTOs;
using RentalAttireBackend.Application.Employees.DTOs;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Disposables.ViewArchivedRecords.ViewEmployeeRecord
{
    public class ViewEmployeeRecord : IViewArchivedEntity
    {
        private readonly IEmployeeRepository _repo;
        private readonly IMapper _mapper;
        private readonly IFileUploadService _fileUploadService;
        public string EntityType => "Employee";

        public ViewEmployeeRecord(
            IEmployeeRepository repo,
            IMapper mapper,
            IFileUploadService fileUploadService
            )
        {
            _repo = repo;
            _mapper = mapper;
            _fileUploadService = fileUploadService;
        }

        public async Task<Result<ViewRecordResponse>> GetArchivedRecordAsync(int id, string entityType, CancellationToken ct)
        {
            var employee = await _repo.GetEmployeeByIdAsync(id, ct);

            if (employee is null)
                return Result<ViewRecordResponse>.Failure("Record does not exist.");

            var employeeDto = _mapper.Map<EmployeeDTO>(employee);

            employeeDto.Person.ProfileImagePath = _fileUploadService.GetFileUrl(employeeDto.Person.ProfileImagePath);

            return Result<ViewRecordResponse>.Success(new ViewRecordResponse
            {
                EntityType = "Employee",
                Record = employeeDto
            });
        }
    }
}
