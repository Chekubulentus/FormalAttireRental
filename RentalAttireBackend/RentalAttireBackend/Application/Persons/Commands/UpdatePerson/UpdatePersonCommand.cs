using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Persons.DTO;

namespace RentalAttireBackend.Application.Persons.Commands.UpdatePerson
{
    public class UpdatePersonCommand : PersonDTO, IRequest<Result<bool>>
    {

    }
}
