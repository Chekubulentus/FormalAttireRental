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

        public async Task<int> CreatePersonAsync(Person person)
        {
            await _context.People.AddAsync(person);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<Person>> GetAllPersonAsync()
        {
            return await _context.People
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Person?> GetPersonByIdAsync(int id)
        {
            return await _context.People.FindAsync(id);
        }

        public async Task<bool> UpdatePersonAsync(Person person)
        {
            _context.People.Update(person);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
