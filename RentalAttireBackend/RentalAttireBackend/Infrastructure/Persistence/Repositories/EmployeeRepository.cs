using Microsoft.EntityFrameworkCore;
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
        public async Task<bool> CreateEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            await _context.Employees.AddAsync(employee);
            return await _context.SaveChangesAsync(cancellationToken) > 0 ;
        }

        public async Task<List<Employee>> GetAllEmployeesAsync(CancellationToken cancellationToken)
        {
            return await _context.Employees
                .AsNoTracking()
                .Include(e => e.Role)
                .Include(e => e.Person)
                .ToListAsync(cancellationToken);
        }

        public async Task<Employee?> GetEmployeeByEmployeeCodeAsync(string employeeCode, CancellationToken cancellationToken)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeCode.Contains(employeeCode), cancellationToken);
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Employees.FindAsync(id, cancellationToken);
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            _context.Employees.Update(employee);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
