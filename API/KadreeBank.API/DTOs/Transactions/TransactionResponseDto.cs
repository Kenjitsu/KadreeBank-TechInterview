using KadreeBank.API.Models.Enums;

namespace KadreeBank.API.DTOs.Transactions;

public sealed record TransactionResponseDto(
    long Id,
    int AccountId,
    TransactionType Type,
    decimal Amount,
    string City,
    decimal BalanceAfter,
    DateTime CreatedAt);
