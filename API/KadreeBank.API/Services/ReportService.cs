using System.Net;
using KadreeBank.API.Common.Errors;
using KadreeBank.API.Common.Results;
using KadreeBank.API.Data;
using KadreeBank.API.DTOs.Reports;
using KadreeBank.API.Interfaces;
using KadreeBank.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace KadreeBank.API.Services;

public class ReportService(KadreeBankDbContext dbContext) : IReportService
{
    private const decimal OutOfCityWithdrawalThreshold = 1_000_000m;

    public async Task<Result<IReadOnlyList<MonthlyTransactionCountItemDto>>> GetMonthlyTransactionCountsAsync(MonthlyTransactionsReportRequestDto request, CancellationToken cancellationToken)
    {
        var periodStart = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = periodStart.AddMonths(1);

        // Se agrupa por cliente
        var report = await dbContext.Transactions
            .Where(t => t.CreatedAt >= periodStart && t.CreatedAt < periodEnd)
            .GroupBy(t => new
            {
                t.Account.CustomerId,
                t.Account.Customer.DocumentNumber,
                t.Account.Customer.FullName
            })
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key.CustomerId)
            .Select(g => new MonthlyTransactionCountItemDto(
                g.Key.CustomerId,
                g.Key.DocumentNumber,
                g.Key.FullName,
                g.Count()))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<MonthlyTransactionCountItemDto>>.Success(report);
    }

    public async Task<Result<IReadOnlyList<OutOfCityWithdrawalItemDto>>> GetOutOfCityWithdrawalsAsync(OutOfCityWithdrawalsReportRequestDto request, CancellationToken cancellationToken)
    {
        var year = request.Year;
        var month = request.Month;

        if (month is not null && year is null)
            return Result<IReadOnlyList<OutOfCityWithdrawalItemDto>>.Failure(ReportErrors.InvalidPeriod, HttpStatusCode.BadRequest);

        // Retiros hechos en una ciudad distinta a la ciudad de origen de la cuenta.
        var withdrawals = dbContext.Transactions
            .Where(t => t.Type == TransactionType.Withdrawal && t.City != t.Account.City);

        if (year is not null)
        {
            var periodStart = new DateTime(year.Value, month ?? 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var periodEnd = month is null ? periodStart.AddYears(1) : periodStart.AddMonths(1);

            withdrawals = withdrawals.Where(t => t.CreatedAt >= periodStart && t.CreatedAt < periodEnd);
        }

        var report = await withdrawals
            .GroupBy(t => new
            {
                t.Account.CustomerId,
                t.Account.Customer.DocumentNumber,
                t.Account.Customer.FullName
            })
            .Where(g => g.Sum(t => t.Amount) > OutOfCityWithdrawalThreshold)
            .OrderByDescending(g => g.Sum(t => t.Amount))
            .ThenBy(g => g.Key.CustomerId)
            .Select(g => new OutOfCityWithdrawalItemDto(
                g.Key.CustomerId,
                g.Key.DocumentNumber,
                g.Key.FullName,
                g.Count(),
                g.Sum(t => t.Amount)))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<OutOfCityWithdrawalItemDto>>.Success(report);
    }
}
