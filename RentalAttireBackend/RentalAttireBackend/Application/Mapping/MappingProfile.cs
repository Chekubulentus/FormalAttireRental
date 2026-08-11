using AutoMapper;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using RentalAttireBackend.Application.AuditLogs.DTOs;
using RentalAttireBackend.Application.Authentication.Commands.ProfileCompletion;
using RentalAttireBackend.Application.Authentication.Commands.RegistrationCommand;
using RentalAttireBackend.Application.Categories.Commands.CreateCategory;
using RentalAttireBackend.Application.Categories.Commands.UpdateCategory;
using RentalAttireBackend.Application.Categories.DTOs;
using RentalAttireBackend.Application.Clothes.Commands.CreateClothe;
using RentalAttireBackend.Application.Clothes.Commands.UpdateClothe;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Customers.Commands.CustomerRegistration;
using RentalAttireBackend.Application.Customers.DTOs;
using RentalAttireBackend.Application.Employees.Commands.CreateEmployee;
using RentalAttireBackend.Application.Employees.Commands.UpdateEmployee;
using RentalAttireBackend.Application.Employees.DTOs;
using RentalAttireBackend.Application.Persons.Commands.UpdatePerson;
using RentalAttireBackend.Application.Persons.DTO;
using RentalAttireBackend.Application.Rentals.Commands.RentalTransaction;
using RentalAttireBackend.Application.Rentals.DTOs;
using RentalAttireBackend.Application.Users.DTO;
using RentalAttireBackend.Domain.Entities;
using System.CodeDom;
using System.Runtime.Serialization;

namespace RentalAttireBackend.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            #region Person->PersonDTO
            CreateMap<Person, PersonDTO>()
                .ForMember(dest => dest.Gender,
                opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.MaritalStatus,
                opt => opt.MapFrom(src => src.MaritalStatus.ToString()));
            #endregion

            #region PersonDTO->Person
            CreateMap<PersonDTO, Person>()
                .ForMember(dest => dest.Gender,
                opt => opt.MapFrom(src => Enum.Parse<Gender>(src.Gender, true)))
                .ForMember(dest => dest.MaritalStatus,
                opt => opt.MapFrom(src => Enum.Parse<MaritalStatus>(src.MaritalStatus, true)))
                .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => "Person"));
            #endregion

            #region UpdatePersonCommand->Person
            CreateMap<UpdatePersonCommand, Person>()
                .ForMember(dest => dest.Gender,
                opt => opt.MapFrom(src => Enum.Parse<Gender>(src.Gender, true)))
                .ForMember(dest => dest.MaritalStatus,
                opt => opt.MapFrom(src => Enum.Parse<MaritalStatus>(src.MaritalStatus, true)));
            #endregion

            #region Employee->EmployeeDTO
            CreateMap<Employee, EmployeeDTO>()
                .ForMember(dest => dest.RolePosition,
                opt => opt.MapFrom(src => src.Role.RolePosition))
                .ForMember(dest => dest.Person,
                opt => opt.MapFrom(src => src.User.Person));

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            #endregion

            #region Disposables
            CreateMap<User, ArchivedEntityDto>()
                .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => src.EntityType))
                .ForMember(dest => dest.EntityId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ArchivedAt,
                opt => opt.MapFrom(src => src.ArchivedAt))
                .ForMember(dest => dest.ArchivedBy,
                opt => opt.MapFrom(src => src.ArchivedBy))
                .ForMember(dest => dest.EntityName,
                opt => opt.MapFrom(src => src.Person.FullName));

            CreateMap<Employee, ArchivedEntityDto>()
                .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => src.EntityType))
                .ForMember(dest => dest.EntityId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ArchivedAt,
                opt => opt.MapFrom(src => src.ArchivedAt))
                .ForMember(dest => dest.ArchivedBy,
                opt => opt.MapFrom(src => src.ArchivedBy))
                .ForMember(dest => dest.EntityName,
                opt => opt.MapFrom(src => src.User.Person.FullName));

            CreateMap<Person, ArchivedEntityDto>()
                .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => src.EntityType))
                .ForMember(dest => dest.EntityId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ArchivedAt,
                opt => opt.MapFrom(src => src.ArchivedAt))
                .ForMember(dest => dest.ArchivedBy,
                opt => opt.MapFrom(src => src.ArchivedBy));

            CreateMap<Customer, ArchivedEntityDto>()
                .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => src.EntityType))
                .ForMember(dest => dest.EntityId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ArchivedAt,
                opt => opt.MapFrom(src => src.ArchivedAt))
                .ForMember(dest => dest.ArchivedBy,
                opt => opt.MapFrom(src => src.ArchivedBy))
                .ForMember(dest => dest.EntityName,
                opt => opt.MapFrom(src => src.User.Person.FullName));

            CreateMap<Clothe, ArchivedEntityDto>()
                .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => src.EntityType))
                .ForMember(dest => dest.EntityId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ArchivedAt,
                opt => opt.MapFrom(src => src.ArchivedAt))
                .ForMember(dest => dest.ArchivedBy,
                opt => opt.MapFrom(src => src.ArchivedBy))
                .ForMember(dest => dest.EntityName,
                opt => opt.MapFrom(src => src.ClotheName));

            CreateMap<Category, ArchivedEntityDto>()
                .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => src.EntityType))
                .ForMember(dest => dest.EntityId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ArchivedAt,
                opt => opt.MapFrom(src => src.ArchivedAt))
                .ForMember(dest => dest.ArchivedBy,
                opt => opt.MapFrom(src => src.ArchivedBy))
                .ForMember(dest => dest.EntityName,
                opt => opt.MapFrom(src => src.CategoryName));

            #endregion

            #region CreateEmployeeCommand->Employee
            CreateMap<CreateEmployeeCommand, Employee>()
                .ForMember(dest => dest.RoleId,
                opt => opt.MapFrom(src => Enum.Parse<RolePosition>(src.RolePosition, true)))
                .ForMember(dest => dest.Id,
                opt => opt.Ignore())
                .ForPath(dest => dest.User.Email,
                opt => opt.MapFrom(src => src.Email))
                .ForPath(dest => dest.User.HashedPassword,
                opt => opt.Ignore());
            #endregion

            #region CreateEmployeeCommand-User
            CreateMap<CreateEmployeeCommand, User>()
                .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.HashedPassword,
                opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken,
                opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenExpiryTime,
                opt => opt.MapFrom(src => DateTime.UtcNow.AddDays(7)))
                .ForMember(dest => dest.Person,
                opt => opt.MapFrom(src => src.Person))
                .ForMember(dest => dest.CreatedBy,
                opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => "Employee"));
            #endregion

            #region UpdateEmployeeCommand -> Employee
            CreateMap<UpdateEmployeeCommand, Employee>()
                .ForMember(dest => dest.RoleId,
                opt => opt.MapFrom(src => Enum.Parse<RolePosition>(src.RolePosition, true)))
                .ForMember(dest => dest.UpdatedBy,
                opt => opt.MapFrom(src => src.PerformedBy))
                .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(8)))
                .ForPath(dest => dest.User.Person.UpdatedBy,
                opt => opt.MapFrom(src => src.PerformedBy))
                .ForPath(dest => dest.User.Person.UpdatedAt,
                opt => opt.MapFrom(src => DateTime.UtcNow));
            #endregion

            #region User->UserDTO
            CreateMap<User, UserDTO>();
            #endregion

            #region UserViewModel
            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.EmployeeCode,
                opt => opt.MapFrom(src => src.Employee.EmployeeCode))
                .ForMember(dest => dest.RolePosition,
                opt => opt.MapFrom(src => src.Employee.Role.RolePosition.ToString()))
                .ForMember(dest => dest.Department,
                opt => opt.MapFrom(src => src.Employee.Department))
                .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.Person.FullName))
                .ForMember(dest => dest.IsGoogleAccount,
                opt => opt.MapFrom(src => src.IsGoogleAccount))
                .ForMember(dest => dest.CustomerCode,
                opt => opt.MapFrom(src => src.Customer.CustomerCode));
            #endregion

            #region AuditLog -> AuditLogDTO
            CreateMap<AuditLog, AuditLogDTO>();
            #endregion

            #region Employee -> Employee
            CreateMap<Employee, Employee>();
            #endregion

            #region Clothe -> ClotheDTO
            CreateMap<Clothe, ClotheDTO>()
                .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.ClotheGender,
                opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.Condition,
                opt => opt.MapFrom(src => src.Condition.ToString()))
                .ForMember(dest => dest.ProfileImagePath,
                opt => opt.MapFrom(src => src.ProfileImagePath));
            #endregion

            #region CreateClotheCommand -> Clothe
            CreateMap<CreateClotheCommand, Clothe>()
                .ForMember(c => c.CategoryId,
                opt => opt.Ignore())
                .ForMember(dest => dest.Gender,
                opt => opt.MapFrom(src => Enum.Parse<ClotheGender>(src.ClotheGender, true)))
                .ForMember(dest => dest.Condition,
                opt => opt.MapFrom(src => Enum.Parse<Condition>(src.Condition, true)))
                .ForMember(dest => dest.ProfileImagePath,
                opt => opt.Ignore())
                .ForMember(dest => dest.IsAvailable,
                opt => opt.Ignore())
                .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => "Clothe"))
                .ForMember(dest => dest.CreatedBy,
                opt => opt.MapFrom(src => src.PerformedBy));
            #endregion

            #region UpdateClotheCommand -> Clothe
            CreateMap<UpdateClotheCommand, Clothe>()
                .ForMember(dest => dest.CategoryId,
                opt => opt.Ignore())
                .ForMember(dest => dest.Gender,
                opt => opt.MapFrom(src => Enum.Parse<ClotheGender>(src.ClotheGender, true)))
                .ForMember(dest => dest.ProfileImagePath,
                opt => opt.Ignore())
                .ForMember(dest => dest.IsAvailable,
                opt => opt.Ignore());
            #endregion

            #region Category -> CategoryDTO 
            CreateMap<Category, CategoryDTO>();
            #endregion

            #region CreateCategoryCommand -> Category
            CreateMap<CreateCategoryCommand, Category>()
                .ForMember(dest => dest.Id,
                opt => opt.Ignore())
                .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => "Category"));
            #endregion

            #region UpdateCategoryCommand -> Category
            CreateMap<UpdateCategoryCommand, Category>()
                .ForMember(dest => dest.Id,
                opt => opt.Ignore())
                .ForMember(dest => dest.CategoryCode,
                opt => opt.MapFrom(src => src.CategoryCode))
                .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.Description,
                opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedBy,
                opt => opt.MapFrom(src => src.PerformedBy));
            #endregion

            #region Customer -> CustomerDTO
            CreateMap<Customer, CustomerDTO>()
                .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.IsGoogleAccount,
                opt => opt.MapFrom(src => src.User.IsGoogleAccount))
                .ForMember(dest => dest.Person,
                opt => opt.MapFrom(src => src.User.Person));
            #endregion

            #region RegistrationCustomerCommnad -> Customer
            CreateMap<CustomerRegistrationCommand, Customer>()
                .ForMember(dest => dest.CustomerCode,
                opt => opt.Ignore())
                .ForMember(dest => dest.UserId,
                opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy,
                opt => opt.MapFrom(src => src.PerformedBy))
                .ForPath(dest => dest.User.Email,
                opt => opt.MapFrom(src => src.Email))
                .ForPath(dest => dest.User.HashedPassword,
                opt => opt.Ignore())
                .ForPath(dest => dest.User.Person,
                opt => opt.MapFrom(src => src.Person))
                .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => "Customer"));
            #endregion

            #region RentalItem -> RentalItemDTO
            CreateMap<RentalItem, RentalItemDTO>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.RentalCode,
                opt => opt.MapFrom(src => src.Rental.RentalCode))
                .ForMember(dest => dest.Clothe,
                opt => opt.MapFrom(src => src.Clothe))
                .ForMember(dest => dest.RentalPrice,
                opt => opt.MapFrom(src => src.RentalPrice))
                .ForMember(dest => dest.Quantity,
                opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.TotalAmount,
                opt => opt.MapFrom(src => src.TotalAmount));
            #endregion

            #region Rental -> RentalDTO
            CreateMap<Rental, RentalDTO>()
                .ForMember(dest => dest.RentalCode,
                opt => opt.MapFrom(src => src.RentalCode))
                .ForMember(dest => dest.Customer,
                opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.PickupDate,
                opt => opt.MapFrom(src => src.RentalDate))
                .ForMember(dest => dest.ReturnDate,
                opt => opt.MapFrom(src => src.ReturnDate))
                .ForMember(dest => dest.TotalAmount,
                opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.DepositAmount,
                opt => opt.MapFrom(dest => dest.Status))
                .ForMember(dest => dest.PaymentMethod,
                opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.RentalItems,
                opt => opt.MapFrom(src => src.RentalItems));
            #endregion

            #region RentalItemRequest -> RentalItem 
            CreateMap<RentalItemRequest, RentalItem>()
                .ForMember(dest => dest.RentalId,
                opt => opt.Ignore())
                .ForMember(dest => dest.ClotheId,
                opt => opt.MapFrom(src => src.ClotheId))
                .ForMember(dest => dest.RentalPrice,
                opt => opt.Ignore())
                .ForMember(dest => dest.Quantity,
                opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.DepositAmount,
                opt => opt.MapFrom(src => src.DepositAmount));
            #endregion

            #region RentalTransactionCommand -> Rental
            CreateMap<RentalTransactionCommand, Rental>()
                .ForMember(dest => dest.RentalCode,
                opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId,
                opt => opt.Ignore())
                .ForMember(dest => dest.RentalDate,
                opt => opt.MapFrom(src => src.PickupDate))
                .ForMember(dest => dest.RentalDate,
                opt => opt.MapFrom(src => src.ReturnDate))
                .ForMember(dest => dest.TotalAmount,
                opt => opt.Ignore())
                .ForMember(dest => dest.DepositAmount,
                opt => opt.Ignore())
                .ForMember(dest => dest.Status,
                opt => opt.Ignore())
                .ForMember(dest => dest.PaymentMethod,
                opt => opt.MapFrom(src => Enum.Parse<PaymentMethod>(src.PaymentMethod, true)));
            #endregion

            #region ProfileCompletionComamnd -> Person
            CreateMap<ProfileCompletionCommand, Person>()
                .ForMember(dest => dest.Age,
                opt => opt.MapFrom(src => src.Age))
                .ForMember(dest => dest.Gender,
                opt => opt.MapFrom(src => Enum.Parse<Gender>(src.Gender, true)))
                .ForMember(dest => dest.MaritalStatus,
                opt => opt.MapFrom(src => Enum.Parse<MaritalStatus>(src.MaritalStatus, true)))
                .ForMember(dest => dest.PhoneNumber,
                opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Street,
                opt => opt.MapFrom(src => src.Street))
                .ForMember(dest => dest.Barangay,
                opt => opt.MapFrom(src => src.Barangay))
                .ForMember(dest => dest.City,
                opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.Province,
                opt => opt.MapFrom(src => src.Province))
                .ForMember(dest => dest.PostalCode,
                opt => opt.MapFrom(src => src.PostalCode));
            #endregion
        }
    }
}
