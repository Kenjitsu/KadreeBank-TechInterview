using KadreeBank.API.DTOs.Accounts;
using KadreeBank.API.DTOs.Transactions;
using KadreeBank.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KadreeBank.API.Controllers;

public class AccountsController(
    IAccountService accountService,
    ITransactionService transactionService) : BaseApiController
{
    /// <summary>Abre una nueva cuenta para un cliente existente.</summary>
    [HttpPost]
    public async Task<IActionResult> OpenAccount(OpenAccountRequestDto request, CancellationToken cancellationToken)
    {
        var result = await accountService.OpenAccountAsync(request, cancellationToken);

        return result.Match(
            onSuccess: successResult => CreatedAtAction(nameof(GetAccount), new { accountId = successResult.Data!.Id }, successResult),
            onFailure: failureResult => StatusCode(failureResult.StatusCode, failureResult)
        );
    }

    /// <summary>Obtiene una cuenta, incluyendo su saldo actual.</summary>
    [HttpGet("{accountId:int}")]
    public async Task<IActionResult> GetAccount(int accountId, CancellationToken cancellationToken)
    {
        var result = await accountService.GetAccountAsync(accountId, cancellationToken);

        return result.Match(
            onSuccess: successResult => Ok(successResult),
            onFailure: failureResult => StatusCode(failureResult.StatusCode, failureResult)
        );
    }

    /// <summary>Obtiene el balance actual de una cuenta.</summary>
    [HttpGet("{accountId:int}/balance")]
    public async Task<IActionResult> GetBalance(int accountId, CancellationToken cancellationToken)
    {
        var result = await accountService.GetBalanceAsync(accountId, cancellationToken);

        return result.Match(
            onSuccess: successResult => Ok(successResult),
            onFailure: failureResult => StatusCode(failureResult.StatusCode, failureResult)
        );
    }

    /// <summary>Deposita dinero en una cuenta.</summary>
    [HttpPost("{accountId:int}/deposits")]
    public async Task<IActionResult> Deposit(int accountId, TransactionRequestDto request, CancellationToken cancellationToken)
    {
        var result = await accountService.DepositAsync(accountId, request, cancellationToken);

        return result.Match(
            onSuccess: successResult => Ok(successResult),
            onFailure: failureResult => StatusCode(failureResult.StatusCode, failureResult)
        );
    }

    /// <summary>Retira dinero de una cuenta. El saldo nunca puede volverse negativo.</summary>
    [HttpPost("{accountId:int}/withdrawals")]
    public async Task<IActionResult> Withdraw(int accountId, TransactionRequestDto request, CancellationToken cancellationToken)
    {
        var result = await accountService.WithdrawAsync(accountId, request, cancellationToken);

        return result.Match(
            onSuccess: successResult => Ok(successResult),
            onFailure: failureResult => StatusCode(failureResult.StatusCode, failureResult)
        );
    }

    /// <summary>Obtiene los movimientos más recientes de una cuenta (por defecto 10, máximo 100).</summary>
    [HttpGet("{accountId:int}/transactions")]
    public async Task<IActionResult> GetRecentTransactions(int accountId, [FromQuery] RecentTransactionsRequestDto request, CancellationToken cancellationToken)
    {
        var result = await transactionService.GetRecentTransactionsAsync(accountId, request, cancellationToken);

        return result.Match(
            onSuccess: successResult => Ok(successResult),
            onFailure: failureResult => StatusCode(failureResult.StatusCode, failureResult)
        );
    }

    /// <summary>Genera el extracto mensual de una cuenta: saldo inicial, totales, saldo final y movimientos del mes.</summary>
    [HttpGet("{accountId:int}/statements/{year:int}/{month:int}")]
    public async Task<IActionResult> GetMonthlyStatement(int accountId, [FromRoute] MonthlyStatementRequestDto request, CancellationToken cancellationToken)
    {
        var result = await transactionService.GetMonthlyStatementAsync(accountId, request, cancellationToken);

        return result.Match(
            onSuccess: successResult => Ok(successResult),
            onFailure: failureResult => StatusCode(failureResult.StatusCode, failureResult)
        );
    }
}
