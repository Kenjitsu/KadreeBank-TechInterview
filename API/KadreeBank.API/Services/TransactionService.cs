using System.Net;
using KadreeBank.API.Common.Errors;
using KadreeBank.API.Common.Results;
using KadreeBank.API.Data;
using KadreeBank.API.DTOs.Transactions;
using KadreeBank.API.Extensions.Mappers;
using KadreeBank.API.Interfaces;
using KadreeBank.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace KadreeBank.API.Services;

public class TransactionService(KadreeBankDbContext dbContext) : ITransactionService
{
    public async Task<Result<IReadOnlyList<TransactionResponseDto>>> GetRecentTransactionsAsync(int accountId, RecentTransactionsRequestDto request, CancellationToken cancellationToken)
    {
        var accountExists = await dbContext.Accounts
            .AnyAsync(a => a.Id == accountId, cancellationToken);

        if (!accountExists)
            return Result<IReadOnlyList<TransactionResponseDto>>.Failure(AccountErrors.NotFound, HttpStatusCode.NotFound);

        // Más recientes primero.
        var transactions = await dbContext.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.CreatedAt)
            .ThenByDescending(t => t.Id)
            .Take(request.Take)
            .ProjectToTransactionResponseDto()
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<TransactionResponseDto>>.Success(transactions);
    }

    public async Task<Result<MonthlyStatementResponseDto>> GetMonthlyStatementAsync(int accountId, MonthlyStatementRequestDto request, CancellationToken cancellationToken)
    {
        var accountNumber = await dbContext.Accounts
            .Where(a => a.Id == accountId)
            .Select(a => a.AccountNumber)
            .FirstOrDefaultAsync(cancellationToken);

        if (accountNumber is null)
            return Result<MonthlyStatementResponseDto>.Failure(AccountErrors.NotFound, HttpStatusCode.NotFound);

        var periodStart = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = periodStart.AddMonths(1);

        // Saldo inicial: el saldo después del último movimiento anterior al mes. Si no hay, la cuenta estaba en 0.
        var openingBalance = await dbContext.Transactions
            .Where(t => t.AccountId == accountId && t.CreatedAt < periodStart)
            .OrderByDescending(t => t.CreatedAt)
            .ThenByDescending(t => t.Id)
            .Select(t => (decimal?)t.BalanceAfter)
            .FirstOrDefaultAsync(cancellationToken) ?? 0m;

        var transactions = await dbContext.Transactions
            .Where(t => t.AccountId == accountId && t.CreatedAt >= periodStart && t.CreatedAt < periodEnd)
            .OrderBy(t => t.CreatedAt)
            .ThenBy(t => t.Id)
            .ProjectToTransactionResponseDto()
            .ToListAsync(cancellationToken);

        var totalDeposits = transactions
            .Where(t => t.Type == TransactionType.Deposit)
            .Sum(t => t.Amount);

        var totalWithdrawals = transactions
            .Where(t => t.Type == TransactionType.Withdrawal)
            .Sum(t => t.Amount);

        // Saldo final, el saldo después del último movimiento del mes. Si no hubo movimientos, no cambió.
        var closingBalance = transactions.Count > 0
            ? transactions.Last().BalanceAfter
            : openingBalance;

        var statement = new MonthlyStatementResponseDto(
            accountId,
            accountNumber,
            request.Year,
            request.Month,
            openingBalance,
            totalDeposits,
            totalWithdrawals,
            closingBalance,
            transactions);

        return Result<MonthlyStatementResponseDto>.Success(statement);
    }
}
