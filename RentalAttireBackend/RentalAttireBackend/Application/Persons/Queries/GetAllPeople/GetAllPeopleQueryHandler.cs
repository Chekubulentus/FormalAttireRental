using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Persons.DTO;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.InteropServices;

namespace RentalAttireBackend.Application.Persons.Queries.GetAllPeople
{
    public class GetAllPeopleQueryHandler : IRequestHandler<GetAllPeopleQuery, Result<PagedResult<PersonDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IPersonRepository _personRepo;
        private readonly IFileUploadService _fileUploadService;

        public GetAllPeopleQueryHandler
            (
            IMapper mapper,
            IPersonRepository personRepo,
            IFileUploadService fileUploadService
            )
        {
            _mapper = mapper;
            _personRepo = personRepo;
            _fileUploadService = fileUploadService;
        }

        public async Task<Result<PagedResult<PersonDTO>>> Handle(GetAllPeopleQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null)
                    return Result<PagedResult<PersonDTO>>.Failure("No person currently registered.");

                var people = await _personRepo.GetAllPersonAsync(request.PaginationParams,cancellationToken);

                foreach (var person in people.Items)
                {
                    if (string.IsNullOrEmpty(person.ProfileImagePath))
                        continue;
                    person.ProfileImagePath = _fileUploadService.GetFileUrl(person.ProfileImagePath);
                }

                if (!people.Items.Any() || people.Items.Count() == 0)
                    return Result<PagedResult<PersonDTO>>.Failure("No person currently registered.");

                var peopleDto = _mapper.Map<PagedResult<PersonDTO>>(people);


                return Result<PagedResult<PersonDTO>>.Success(peopleDto);
            }catch(Exception e)
            {
                return Result<PagedResult<PersonDTO>>.Failure($"{e.Message}");
            }
        }
    }
}
