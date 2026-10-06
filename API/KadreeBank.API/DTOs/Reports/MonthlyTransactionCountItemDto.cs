namespace KadreeBank.API.DTOs.Reports;

public sealed record MonthlyTransactionCountItemDto(
    int CustomerId,
    string DocumentNumber,
    string FullName,
    int TransactionCount);
