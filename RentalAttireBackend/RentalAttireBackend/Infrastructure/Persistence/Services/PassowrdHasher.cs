using RentalAttireBackend.Application.Common.Interfaces;

namespace RentalAttireBackend.Infrastructure.Persistence.Services
{
    public class PassowrdHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, hashedPassword);
        }
    }
}
