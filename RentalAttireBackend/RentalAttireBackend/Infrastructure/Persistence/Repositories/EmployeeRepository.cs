using Microsoft.EntityFrameworkCore;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using RentalAttireBackend.Infrastructure.Persistence.DataContext;

namespace RentalAttireBackend.Infrastructure.Persistence.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly FormalAttireContext _context;

        public EmployeeRepository
            (
            FormalAttireContext context
            )
        {
            _context = context;
        }
        public async Task<int> CreateEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync(cancellationToken);
            return employee.Id;
        }

        public async Task<PagedResult<Employee>> GetAllEmployeesAsync(
            PaginationParams paginationParams,
            CancellationToken cancellationToken
            )
        {
            var employees = _context.Employees
            .AsNoTracking()
            .Include(e => e.Role)
            .Include(e => e.User)
                .ThenInclude(u => u.Person)
            .AsQueryable();

            var totalCount = await employees.CountAsync(cancellationToken);

            var items = await employees
                .Skip(paginationParams.Skip)
                .Take(paginationParams.ItemsPerPage)
                .ToListAsync(cancellationToken);

            return new PagedResult<Employee>
            {
                Items = items,
                PageNumber = paginationParams.CurrentPage,
                PageSize = paginationParams.ItemsPerPage,
                TotalCount = totalCount
            };
        }

        public async Task<Employee?> GetEmployeeByEmployeeCodeAsync(string employeeCode, CancellationToken cancellationToken)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeCode.Contains(employeeCode), cancellationToken);
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Employees
                .Include(e => e.User)
                    .ThenInclude(u => u.Person)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<PagedResult<Employee>> SearchEmployeeAsync
            (string searchQuery,
            PaginationParams paginationParams,
            CancellationToken cancellationToken
            )
        {
            var query = _context.Employees
                .AsNoTracking()
                .Include(e => e.Role)
                .Include(e => e.User)
                .ThenInclude(u => u.Person)
                .Where(e =>
                    e.EmployeeCode.ToLower().Contains(searchQuery.ToLower()) ||
                    e.Role.RolePosition.ToString().ToLower().Contains(searchQuery.ToLower()) ||
                    e.User.Person.LastName.ToLower().Contains(searchQuery.ToLower())
                );

            var totalCount = await query.CountAsync();

            var employees = await query
                .Skip(paginationParams.Skip)
                .Take(paginationParams.ItemsPerPage)
                .ToListAsync(cancellationToken);

            return new PagedResult<Employee>
            {
                Items = employees,
                TotalCount = totalCount,
                PageNumber = paginationParams.CurrentPage,
                PageSize = paginationParams.ItemsPerPage
            };
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            _context.Employees.Update(employee);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
