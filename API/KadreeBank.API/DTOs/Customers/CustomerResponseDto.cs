using KadreeBank.API.DTOs.Accounts;
using KadreeBank.API.Models.Enums;

namespace KadreeBank.API.DTOs.Customers;

public sealed record CustomerResponseDto(
    int Id,
    string DocumentNumber,
    string FullName,
    CustomerType Type,
    DateTime CreatedAt,
    IReadOnlyList<AccountResponseDto> Accounts);
