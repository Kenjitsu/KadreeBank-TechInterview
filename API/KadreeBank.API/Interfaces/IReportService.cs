using KadreeBank.API.Common.Results;
using KadreeBank.API.DTOs.Reports;

namespace KadreeBank.API.Interfaces;

public interface IReportService
{
    Task<Result<IReadOnlyList<MonthlyTransactionCountItemDto>>> GetMonthlyTransactionCountsAsync(MonthlyTransactionsReportRequestDto request, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<OutOfCityWithdrawalItemDto>>> GetOutOfCityWithdrawalsAsync(OutOfCityWithdrawalsReportRequestDto request, CancellationToken cancellationToken);
}
