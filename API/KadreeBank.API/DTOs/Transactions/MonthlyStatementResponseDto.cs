namespace KadreeBank.API.DTOs.Transactions;

public sealed record MonthlyStatementResponseDto(
    int AccountId,
    string AccountNumber,
    int Year,
    int Month,
    decimal OpeningBalance,
    decimal TotalDeposits,
    decimal TotalWithdrawals,
    decimal ClosingBalance,
    IReadOnlyList<TransactionResponseDto> Transactions);
