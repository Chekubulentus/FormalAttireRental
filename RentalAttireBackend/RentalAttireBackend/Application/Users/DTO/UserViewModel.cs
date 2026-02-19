namespace RentalAttireBackend.Application.Users.DTO
{
    public class UserViewModel : UserDTO
    {
        public string EmployeeCode { get; set; } = string.Empty;    
        public string RolePosition { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
