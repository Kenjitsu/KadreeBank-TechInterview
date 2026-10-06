using KadreeBank.API.Models.Enums;

namespace KadreeBank.API.DTOs.Accounts;

public sealed record AccountResponseDto(
    int Id,
    string AccountNumber,
    int CustomerId,
    AccountType Type,
    string City,
    decimal Balance,
    DateTime CreatedAt);
