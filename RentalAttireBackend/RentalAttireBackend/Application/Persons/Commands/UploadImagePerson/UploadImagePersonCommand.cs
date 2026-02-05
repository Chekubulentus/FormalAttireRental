using MediatR;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Persons.Commands.UploadImagePerson
{
    public class UploadImagePersonCommand : IRequest<Result<string>>
    {
        public IFormFile? Image { get; set; }
        public int Id { get; set; }
    }
}
