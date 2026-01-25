using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Domain.Interfaces
{
    public interface IPersonRepository
    {
        #region Commands
        public Task<int> CreatePersonAsync(Person person, CancellationToken cancellationToken);
        public Task<bool> UpdatePersonAsync(Person person, CancellationToken cancellationToken);
        #endregion

        #region Queries
        public Task<List<Person>> GetAllPersonAsync(CancellationToken cancellationToken);
        public Task<Person?> GetPersonByIdAsync(int id, CancellationToken cancellationToken);
        #endregion
    }
}
