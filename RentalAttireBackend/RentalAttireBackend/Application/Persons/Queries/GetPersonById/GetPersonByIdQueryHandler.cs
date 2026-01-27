using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Persons.DTO;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Persons.Queries.GetPersonById
{
    public class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, Result<PersonDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IPersonRepository _personRepo;

        public GetPersonByIdQueryHandler
            (
            IMapper mapper,
            IPersonRepository personRepo
            )
        {
            _mapper = mapper;
            _personRepo = personRepo;
        }

        public async Task<Result<PersonDTO>> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Id == 0)
                    return Result<PersonDTO>.Failure("Invalid person identifier.");

                var person = await _personRepo.GetPersonByIdAsync(request.Id, cancellationToken);

                if (person is null)
                    return Result<PersonDTO>.Failure("Person does not exist.");

                var personDto = _mapper.Map<PersonDTO>(person);

                return Result<PersonDTO>.Success(personDto);
            }
            catch(Exception e)
            {
                return Result<PersonDTO>.Failure(e.Message);
            }
        }
    }
}
