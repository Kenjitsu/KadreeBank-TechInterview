namespace KadreeBank.API.DTOs.Reports;

public sealed record OutOfCityWithdrawalItemDto(
    int CustomerId,
    string DocumentNumber,
    string FullName,
    int WithdrawalCount,
    decimal TotalWithdrawn);
