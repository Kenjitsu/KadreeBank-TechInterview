using KadreeBank.API.Common.Results;
using KadreeBank.API.DTOs.Transactions;

namespace KadreeBank.API.Interfaces;

public interface ITransactionService
{
    Task<Result<IReadOnlyList<TransactionResponseDto>>> GetRecentTransactionsAsync(int accountId, RecentTransactionsRequestDto request, CancellationToken cancellationToken);

    Task<Result<MonthlyStatementResponseDto>> GetMonthlyStatementAsync(int accountId, MonthlyStatementRequestDto request, CancellationToken cancellationToken);
}
