using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Users.DTO;

namespace RentalAttireBackend.Application.Users.Queries.GetUserViewModel
{
    public class GetUserViewModelByIdQuery : IRequest<Result<UserViewModel>>
    {
        public int Id { get; set; }
    }
}
