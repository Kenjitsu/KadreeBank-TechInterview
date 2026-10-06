using System.Net;
using System.Security.Cryptography;
using KadreeBank.API.Common.Errors;
using KadreeBank.API.Common.Results;
using KadreeBank.API.Data;
using KadreeBank.API.DTOs.Accounts;
using KadreeBank.API.DTOs.Transactions;
using KadreeBank.API.Extensions.Mappers;
using KadreeBank.API.Interfaces;
using KadreeBank.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace KadreeBank.API.Services;

public class AccountService(KadreeBankDbContext dbContext, TimeProvider timeProvider) : IAccountService
{
    private const int AccountNumberLength = 10;
    private const int MaxAccountNumberAttempts = 5;

    public async Task<Result<AccountResponseDto>> OpenAccountAsync(OpenAccountRequestDto request, CancellationToken cancellationToken)
    {
        var customerType = await dbContext.Customers
            .Where(c => c.Id == request.CustomerId)
            .Select(c => (CustomerType?)c.Type)
            .FirstOrDefaultAsync(cancellationToken);

        if (customerType is null)
            return Result<AccountResponseDto>.Failure(CustomerErrors.NotFound, HttpStatusCode.NotFound);

        if (!IsAccountTypeAllowed(customerType.Value, request.Type!.Value))
            return Result<AccountResponseDto>.Failure(AccountErrors.TypeNotAllowed, HttpStatusCode.Conflict);

        var accountNumber = await GenerateAccountNumberAsync(cancellationToken);
        var account = request.ToAccountEntity(accountNumber, timeProvider.GetUtcNow().UtcDateTime);

        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<AccountResponseDto>.Success(account.ToAccountResponseDto(), HttpStatusCode.Created);
    }

    public async Task<Result<AccountResponseDto>> GetAccountAsync(int accountId, CancellationToken cancellationToken)
    {
        var account = await dbContext.Accounts
            .Where(a => a.Id == accountId)
            .ProjectToAccountResponseDto()
            .FirstOrDefaultAsync(cancellationToken);

        return account is null
            ? Result<AccountResponseDto>.Failure(AccountErrors.NotFound, HttpStatusCode.NotFound)
            : Result<AccountResponseDto>.Success(account);
    }

    public async Task<Result<BalanceResponseDto>> GetBalanceAsync(int accountId, CancellationToken cancellationToken)
    {
        var balance = await dbContext.Accounts
            .Where(a => a.Id == accountId)
            .ProjectToBalanceResponseDto()
            .FirstOrDefaultAsync(cancellationToken);

        return balance is null
            ? Result<BalanceResponseDto>.Failure(AccountErrors.NotFound, HttpStatusCode.NotFound)
            : Result<BalanceResponseDto>.Success(balance);
    }

    public Task<Result<TransactionResponseDto>> DepositAsync(int accountId, TransactionRequestDto request, CancellationToken cancellationToken)
        => ApplyTransactionAsync(accountId, TransactionType.Deposit, request, cancellationToken);

    public Task<Result<TransactionResponseDto>> WithdrawAsync(int accountId, TransactionRequestDto request, CancellationToken cancellationToken)
        => ApplyTransactionAsync(accountId, TransactionType.Withdrawal, request, cancellationToken);

    /// <summary>
    /// Actualiza el saldo de la cuenta y registra la transacción en la base de datos. 
    /// Se asegura de que la operación sea atómica y consistente.
    /// </summary>
    private async Task<Result<TransactionResponseDto>> ApplyTransactionAsync(
        int accountId,
        TransactionType type,
        TransactionRequestDto request,
        CancellationToken cancellationToken)
    {
        var amount = request.Amount;

        if (amount <= 0 || decimal.Round(amount, 2) != amount)
            return Result<TransactionResponseDto>.Failure(AccountErrors.InvalidAmount, HttpStatusCode.BadRequest);

        await using var dbTransaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var account = dbContext.Accounts.Where(a => a.Id == accountId);

        var affectedRows = type == TransactionType.Deposit
            ? await account.ExecuteUpdateAsync(
                s => s.SetProperty(a => a.Balance, a => a.Balance + amount), cancellationToken)
            : await account
                .Where(a => a.Balance >= amount)
                .ExecuteUpdateAsync(
                    s => s.SetProperty(a => a.Balance, a => a.Balance - amount), cancellationToken);

        if (affectedRows == 0)
        {
            // No se actualizó ninguna fila, lo que significa que la cuenta no existe o no tiene fondos suficientes.
            var exists = await account.AnyAsync(cancellationToken);

            return exists
                ? Result<TransactionResponseDto>.Failure(AccountErrors.InsufficientFunds, HttpStatusCode.Conflict)
                : Result<TransactionResponseDto>.Failure(AccountErrors.NotFound, HttpStatusCode.NotFound);
        }

        var balanceAfter = await account
            .Select(a => a.Balance)
            .SingleAsync(cancellationToken);

        var transaction = request.ToTransactionEntity(accountId, type, balanceAfter, timeProvider.GetUtcNow().UtcDateTime);

        dbContext.Transactions.Add(transaction);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dbTransaction.CommitAsync(cancellationToken);

        return Result<TransactionResponseDto>.Success(transaction.ToTransactionResponseDto());
    }

    private static bool IsAccountTypeAllowed(CustomerType customerType, AccountType accountType)
        => (customerType, accountType) switch
        {
            (CustomerType.NaturalPerson, AccountType.Savings) => true,
            (CustomerType.Company, AccountType.Checking) => true,
            _ => false
        };

    private async Task<string> GenerateAccountNumberAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < MaxAccountNumberAttempts; attempt++)
        {
            var candidate = string.Concat(
                Enumerable.Range(0, AccountNumberLength)
                    .Select(_ => RandomNumberGenerator.GetInt32(0, 10)));

            var exists = await dbContext.Accounts
                .AnyAsync(a => a.AccountNumber == candidate, cancellationToken);

            if (!exists)
                return candidate;
        }

        // No debería de llegar aquí, si ocurre se maneja la excepción en el middleware.
        throw new InvalidOperationException("No fue posible generar un número de cuenta único.");
    }
}
