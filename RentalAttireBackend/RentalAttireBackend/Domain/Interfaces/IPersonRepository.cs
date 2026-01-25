using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Domain.Interfaces
{
    public interface IPersonRepository
    {
        #region Commands
        public Task<int> CreatePersonAsync(Person person);
        public Task<bool> UpdatePersonAsync(Person person);
        #endregion

        #region Queries
        public Task<List<Person>> GetAllPersonAsync();
        public Task<Person?> GetPersonByIdAsync(int id);
        #endregion
    }
}
