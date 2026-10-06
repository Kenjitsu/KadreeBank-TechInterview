using System.Linq.Expressions;
using KadreeBank.API.DTOs.Transactions;
using KadreeBank.API.Models;
using KadreeBank.API.Models.Enums;

namespace KadreeBank.API.Extensions.Mappers;

public static class TransactionMapperExtensions
{
    private static readonly Expression<Func<Transaction, TransactionResponseDto>> TransactionResponseProjection =
        transaction => new TransactionResponseDto(
            transaction.Id,
            transaction.AccountId,
            transaction.Type,
            transaction.Amount,
            transaction.City,
            transaction.BalanceAfter,
            transaction.CreatedAt);

    private static readonly Func<Transaction, TransactionResponseDto> TransactionResponseMap =
        TransactionResponseProjection.Compile();

    public static IQueryable<TransactionResponseDto> ProjectToTransactionResponseDto(this IQueryable<Transaction> query)
        => query.Select(TransactionResponseProjection);

    public static TransactionResponseDto ToTransactionResponseDto(this Transaction transaction)
        => TransactionResponseMap(transaction);

    public static Transaction ToTransactionEntity(
        this TransactionRequestDto request,
        int accountId,
        TransactionType type,
        decimal balanceAfter,
        DateTime createdAt)
        => new()
        {
            AccountId = accountId,
            Type = type,
            Amount = request.Amount,
            City = request.City.Trim(),
            BalanceAfter = balanceAfter,
            CreatedAt = createdAt
        };
}
