using MediatR;
using RentalAttireBackend.Application.Common;
using RentalAttireBackend.Application.Persons.DTO;

namespace RentalAttireBackend.Application.Persons.Queries.GetPersonById
{
    public class GetPersonByIdQuery : IRequest<Result<PersonDTO>>
    {
        public int Id { get; set; }
    }
}
