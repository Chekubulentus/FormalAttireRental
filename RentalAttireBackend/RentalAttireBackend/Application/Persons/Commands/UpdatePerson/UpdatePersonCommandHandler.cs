using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Persons.Commands.UpdatePerson
{
    public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, Result<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IPersonRepository _personRepo;

        public UpdatePersonCommandHandler
            (
            IMapper mapper,
            IPersonRepository personRepo
            )
        {
            _mapper = mapper;
            _personRepo = personRepo;
        }

        public async Task<Result<bool>> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null)
                    return Result<bool>.Failure("Invalid person identifier. Please try again.");

                var existingPerson = await _personRepo.GetPersonByIdAsync(request.Id, cancellationToken);

                if (existingPerson is null)
                    return Result<bool>.Failure("Person does not exist.");

                var newPerson = _mapper.Map(request, existingPerson);

                var updatePerson = await _personRepo.UpdatePersonAsync(newPerson, cancellationToken);

                if (!updatePerson)
                    return Result<bool>.Failure("Person cannot be updated. Please try again.");

                return Result<bool>.SuccessWithMessage("Person successfully updated!");

            }catch(Exception e)
            {
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
