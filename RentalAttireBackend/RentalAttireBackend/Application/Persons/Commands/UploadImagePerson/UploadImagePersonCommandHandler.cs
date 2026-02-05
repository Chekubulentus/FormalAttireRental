using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace RentalAttireBackend.Application.Persons.Commands.UploadImagePerson
{
    public class UploadImagePersonCommandHandler : IRequestHandler<UploadImagePersonCommand, Result<string>>
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;
        private readonly IPersonRepository _personRepo;
        private readonly ITransactionManager _transactionManager;

        public UploadImagePersonCommandHandler
            (
            IFileUploadService fileUploadService,
            IMapper mapper,
            IPersonRepository personRepo,
            ITransactionManager transactionManager
            )
        {
            _fileUploadService = fileUploadService;
            _mapper = mapper;
            _personRepo = personRepo;
            _transactionManager = transactionManager;
        }
        public async Task<Result<string>> Handle(UploadImagePersonCommand command, CancellationToken cancellationToken)
        {
            if (command is null)
                return Result<string>.Failure("No image uploaded. Please try again.");
            try
            {
                await _transactionManager.BeginTransactionAsync(cancellationToken);

                var person = await _personRepo.GetPersonByIdAsync(command.Id, cancellationToken);

                if (person is null)
                    return Result<string>.Failure("Person does not exist.");

                if (!string.IsNullOrEmpty(person.ProfileImagePath))
                {
                    await _fileUploadService.DeleteFileAsync(person.ProfileImagePath);
                }

                var uploadImage = await _fileUploadService.UploadImageAsync(command.Image, $"persons/{command.Id}");

                if(!uploadImage.Success)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<string>.Failure(uploadImage.ErrorMessage);
                }

                person.ProfileImagePath = uploadImage.FilePath;
                person.UpdatedAt = DateTime.UtcNow;

                var updatePerson = await _personRepo.UpdatePersonAsync(person, cancellationToken);

                if(!updatePerson)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<string>.Failure("Image cannot be uploaded.");
                }

                var imageUrl = _fileUploadService.GetFileUrl(uploadImage.FilePath);

                await _transactionManager.CommitTransacionAsync(cancellationToken);
                return Result<string>.SuccessWithMessage(imageUrl);
            }catch(Exception e)
            {
                await _transactionManager.RollbackTransactionAsync(cancellationToken);
                return Result<string>.Failure(e.Message);
            }
        }
    }
}
