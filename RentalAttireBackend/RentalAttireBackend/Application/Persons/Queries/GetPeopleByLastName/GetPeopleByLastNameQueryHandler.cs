using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common;
using RentalAttireBackend.Application.Persons.DTO;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Persons.Queries.GetPeopleByLastName
{
    public class GetPeopleByLastNameQueryHandler : IRequestHandler<GetPeopleByLastNameQuery, Result<List<PersonDTO>>>
    {
        private ITransactionManager _transaction;
        private IPersonRepository _personRepo;
        private IMapper _mapper;

        public GetPeopleByLastNameQueryHandler
            (
            ITransactionManager transaction,
            IPersonRepository personRepo,
            IMapper mapper
            )
        {
            _transaction = transaction;
            _personRepo = personRepo;
            _mapper = mapper;
        }

        public async Task<Result<List<PersonDTO>>> Handle(GetPeopleByLastNameQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null || request.LastName is null)
                    return Result<List<PersonDTO>>.Failure($"Invalid request. Please provide a valid last name.");

                var people = await _personRepo.GetPersonByLastName(request.LastName, cancellationToken);

                if (!people.Any() || people.Count() == 0)
                    return Result<List<PersonDTO>>.Failure($"No people found with last name: '{request.LastName}'");

                var peopleDto = _mapper.Map<List<PersonDTO>>(people);

                return Result<List<PersonDTO>>.Success(peopleDto);

            }catch(Exception e)
            {
                return Result<List<PersonDTO>>.Failure(e.Message);
            }
        }
    }
}
