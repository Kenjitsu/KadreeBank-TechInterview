using System.Linq.Expressions;
using KadreeBank.API.DTOs.Accounts;
using KadreeBank.API.DTOs.Customers;
using KadreeBank.API.Models;

namespace KadreeBank.API.Extensions.Mappers;

public static class CustomerMapperExtensions
{
    // Incluye las cuentas del cliente: EF lo resuelve en una sola consulta (LEFT JOIN con Accounts).
    // Si cambia AccountResponseDto, actualizar también AccountMapperExtensions.
    private static readonly Expression<Func<Customer, CustomerResponseDto>> CustomerResponseProjection =
        customer => new CustomerResponseDto(
            customer.Id,
            customer.DocumentNumber,
            customer.FullName,
            customer.Type,
            customer.CreatedAt,
            customer.Accounts
                .OrderBy(account => account.CreatedAt)
                .Select(account => new AccountResponseDto(
                    account.Id,
                    account.AccountNumber,
                    account.CustomerId,
                    account.Type,
                    account.City,
                    account.Balance,
                    account.CreatedAt))
                .ToList());

    private static readonly Func<Customer, CustomerResponseDto> CustomerResponseMap =
        CustomerResponseProjection.Compile();

    public static IQueryable<CustomerResponseDto> ProjectToCustomerResponseDto(this IQueryable<Customer> query)
        => query.Select(CustomerResponseProjection);

    public static CustomerResponseDto ToCustomerResponseDto(this Customer customer)
        => CustomerResponseMap(customer);

    // El PinHash se asigna después en el servicio, porque el hasher recibe la entidad.
    public static Customer ToCustomerEntity(this CreateCustomerRequestDto request, DateTime createdAt)
        => new()
        {
            DocumentNumber = request.DocumentNumber.Trim(),
            FullName = request.FullName.Trim(),
            Type = request.Type!.Value,
            CreatedAt = createdAt
        };
}
