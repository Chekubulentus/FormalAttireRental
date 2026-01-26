using Microsoft.EntityFrameworkCore;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using RentalAttireBackend.Infrastructure.Persistence.DataContext;

namespace RentalAttireBackend.Infrastructure.Persistence.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly FormalAttireContext _context;

        public PersonRepository(FormalAttireContext context)
        {
            _context = context;
        }

        public async Task<int> CreatePersonAsync(Person person, CancellationToken cancellationToken)
        {
            await _context.People.AddAsync(person);
            await _context.SaveChangesAsync(cancellationToken);
            return person.Id;
        }

        public async Task<List<Person>> GetAllPersonAsync(CancellationToken cancellationToken)
        {
            return await _context.People
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Person>> GetPersonByLastName(string email, CancellationToken cancellationToken)
        {
            return await _context.People
                .Where(p => p.LastName.Contains(email))
                .ToListAsync(cancellationToken);
        }

        public async Task<Person?> GetPersonByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.People.FindAsync(id, cancellationToken);
        }

        public async Task<bool> UpdatePersonAsync(Person person, CancellationToken cancellationToken)
        {
            _context.People.Update(person);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
