using MediatR;
using RentalAttireBackend.Application.Common;
using RentalAttireBackend.Application.Persons.DTO;

namespace RentalAttireBackend.Application.Persons.Queries.GetPeopleByLastName
{
    public class GetPeopleByLastNameQuery : IRequest<Result<List<PersonDTO>>>
    {
        public string LastName { get; set; } = string.Empty;
    }
}
