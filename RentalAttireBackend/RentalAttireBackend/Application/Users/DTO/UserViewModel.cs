namespace RentalAttireBackend.Application.Users.DTO
{
    public class UserViewModel : UserDTO
    {
        public string CustomerCode { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;    
        public string RolePosition { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsGoogleAccount { get; set; } = false;
    }
}
