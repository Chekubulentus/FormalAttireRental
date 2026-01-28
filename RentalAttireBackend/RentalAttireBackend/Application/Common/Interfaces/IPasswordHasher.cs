namespace RentalAttireBackend.Application.Common.Interfaces
{
    public interface IPasswordHasher
    {
        public string HashPassword(string password);
        public bool VerifyPassword(string enteredPassword, string hashedPassword);
    }
}
