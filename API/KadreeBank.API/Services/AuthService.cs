using System.Net;
using KadreeBank.API.Common.Errors;
using KadreeBank.API.Common.Results;
using KadreeBank.API.Data;
using KadreeBank.API.DTOs.Auth;
using KadreeBank.API.DTOs.Customers;
using KadreeBank.API.Extensions.Mappers;
using KadreeBank.API.Interfaces;
using KadreeBank.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KadreeBank.API.Services;

public class AuthService(
    KadreeBankDbContext dbContext,
    IPasswordHasher<Customer> pinHasher) : IAuthService
{
    public async Task<Result<CustomerResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var documentNumber = request.DocumentNumber.Trim();

        var credentials = await dbContext.Customers
            .Where(c => c.DocumentNumber == documentNumber)
            .Select(c => new Customer { Id = c.Id, PinHash = c.PinHash })
            .FirstOrDefaultAsync(cancellationToken);

        if (credentials is null || !IsPinValid(credentials, request.Pin))
            return Result<CustomerResponseDto>.Failure(AuthErrors.InvalidCredentials, HttpStatusCode.Unauthorized);

        var customer = await dbContext.Customers
            .Where(c => c.Id == credentials.Id)
            .ProjectToCustomerResponseDto()
            .FirstAsync(cancellationToken);

        return Result<CustomerResponseDto>.Success(customer);
    }

    private bool IsPinValid(Customer customer, string pin)
        => pinHasher.VerifyHashedPassword(customer, customer.PinHash, pin) != PasswordVerificationResult.Failed;
}
