using KadreeBank.API.DTOs.Reports;
using KadreeBank.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KadreeBank.API.Controllers;

public class ReportsController(IReportService reportService) : BaseApiController
{
    /// <summary>Clientes con su número de transacciones en un mes, de mayor a menor.</summary>
    [HttpGet("monthly-transactions")]
    public async Task<IActionResult> GetMonthlyTransactionCounts([FromQuery] MonthlyTransactionsReportRequestDto request, CancellationToken cancellationToken)
    {
        var result = await reportService.GetMonthlyTransactionCountsAsync(request, cancellationToken);

        return result.Match(
            onSuccess: successResult => Ok(successResult),
            onFailure: failureResult => StatusCode(failureResult.StatusCode, failureResult)
        );
    }

    /// <summary>Clientes cuyos retiros fuera de la ciudad de origen de la cuenta suman más de $1.000.000. Año y mes opcionales.</summary>
    [HttpGet("out-of-city-withdrawals")]
    public async Task<IActionResult> GetOutOfCityWithdrawals([FromQuery] OutOfCityWithdrawalsReportRequestDto request, CancellationToken cancellationToken)
    {
        var result = await reportService.GetOutOfCityWithdrawalsAsync(request, cancellationToken);

        return result.Match(
            onSuccess: successResult => Ok(successResult),
            onFailure: failureResult => StatusCode(failureResult.StatusCode, failureResult)
        );
    }
}
