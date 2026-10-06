namespace KadreeBank.API.DTOs.Accounts;

public sealed record BalanceResponseDto(
    int AccountId,
    string AccountNumber,
    decimal Balance);
