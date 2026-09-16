using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Customers.DTOs;

namespace RentalAttireBackend.Application.Customers.Queries.GetCustomerProfile
{
    public class GetCustomerProfileQuery : IRequest<Result<CustomerDTO>>
    {
    }
}
