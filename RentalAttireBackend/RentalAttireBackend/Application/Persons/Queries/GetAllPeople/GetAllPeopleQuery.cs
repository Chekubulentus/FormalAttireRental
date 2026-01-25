using MediatR;
using RentalAttireBackend.Application.Common;
using RentalAttireBackend.Application.Persons.DTO;

namespace RentalAttireBackend.Application.Persons.Queries.GetAllPeople
{
    public class GetAllPeopleQuery : IRequest<Result<List<PersonDTO>>>
    {
    }
}
