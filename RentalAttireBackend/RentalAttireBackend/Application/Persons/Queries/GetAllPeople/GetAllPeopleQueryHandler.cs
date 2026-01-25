using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common;
using RentalAttireBackend.Application.Persons.DTO;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.InteropServices;

namespace RentalAttireBackend.Application.Persons.Queries.GetAllPeople
{
    public class GetAllPeopleQueryHandler : IRequestHandler<GetAllPeopleQuery, Result<List<PersonDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IPersonRepository _personRepo;

        public GetAllPeopleQueryHandler
            (
            IMapper mapper,
            IPersonRepository personRepo
            )
        {
            _mapper = mapper;
            _personRepo = personRepo;
        }

        public async Task<Result<List<PersonDTO>>> Handle(GetAllPeopleQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null)
                    return Result<List<PersonDTO>>.Failure("No person currently registered.");

                var people = await _personRepo.GetAllPersonAsync(cancellationToken);

                if (!people.Any() || people.Count() == 0)
                    return Result<List<PersonDTO>>.Failure("No person currently registered.");

                var peopleDto = _mapper.Map<List<PersonDTO>>(people);


                return Result<List<PersonDTO>>.Success(peopleDto);
            }catch(Exception e)
            {
                return Result<List<PersonDTO>>.Failure($"{e.Message}");
            }
        }
    }
}
