using System.Net;
using KadreeBank.API.Common.Errors;
using KadreeBank.API.Common.Results;
using KadreeBank.API.Data;
using KadreeBank.API.DTOs.Customers;
using KadreeBank.API.Extensions.Mappers;
using KadreeBank.API.Interfaces;
using KadreeBank.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace KadreeBank.API.Services;

public class CustomerService(
    KadreeBankDbContext dbContext,
    IPasswordHasher<Customer> pinHasher,
    TimeProvider timeProvider) : ICustomerService
{
    private const int UniqueIndexViolation = 2601;
    private const int UniqueConstraintViolation = 2627;


    public async Task<Result<CustomerResponseDto>> CreateCustomerAsync(CreateCustomerRequestDto request, CancellationToken cancellationToken)
    {
        var documentNumber = request.DocumentNumber.Trim();

        var documentExists = await dbContext.Customers
            .AnyAsync(c => c.DocumentNumber == documentNumber, cancellationToken);

        if (documentExists)
            return Result<CustomerResponseDto>.Failure(CustomerErrors.DocumentAlreadyExists, HttpStatusCode.Conflict);

        var customer = request.ToCustomerEntity(timeProvider.GetUtcNow().UtcDateTime);

        customer.PinHash = pinHasher.HashPassword(customer, request.Pin);

        dbContext.Customers.Add(customer);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: UniqueIndexViolation or UniqueConstraintViolation })
        {
            return Result<CustomerResponseDto>.Failure(CustomerErrors.DocumentAlreadyExists, HttpStatusCode.Conflict);
        }

        return Result<CustomerResponseDto>.Success(customer.ToCustomerResponseDto(), HttpStatusCode.Created);
    }

    public async Task<Result<CustomerResponseDto>> GetCustomerAsync(int customerId, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Customers
            .Where(c => c.Id == customerId)
            .ProjectToCustomerResponseDto()
            .FirstOrDefaultAsync(cancellationToken);

        return customer is null
            ? Result<CustomerResponseDto>.Failure(CustomerErrors.NotFound, HttpStatusCode.NotFound)
            : Result<CustomerResponseDto>.Success(customer);
    }
}
