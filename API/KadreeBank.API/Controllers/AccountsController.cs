using KadreeBank.API.Common.Results;
using KadreeBank.API.DTOs.Accounts;
using KadreeBank.API.DTOs.Transactions;
using KadreeBank.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KadreeBank.API.Controllers;

public class AccountsController(IAccountService accountService) : BaseApiController
{
    /// <summary>Abre una nueva cuenta para un cliente existente.</summary>
    [HttpPost]
    [ProducesResponseType<Result<AccountResponseDto>>(StatusCodes.Status201Created)]
    [ProducesResponseType<Result<AccountResponseDto>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Result<AccountResponseDto>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Result<AccountResponseDto>>(StatusCodes.Status409Conflict)]
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
    [ProducesResponseType<Result<AccountResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Result<AccountResponseDto>>(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType<Result<BalanceResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Result<BalanceResponseDto>>(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType<Result<TransactionResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Result<TransactionResponseDto>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Result<TransactionResponseDto>>(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType<Result<TransactionResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Result<TransactionResponseDto>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Result<TransactionResponseDto>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Result<TransactionResponseDto>>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Withdraw(int accountId, TransactionRequestDto request, CancellationToken cancellationToken)
    {
        var result = await accountService.WithdrawAsync(accountId, request, cancellationToken);

        return result.Match(
            onSuccess: successResult => Ok(successResult),
            onFailure: failureResult => StatusCode(failureResult.StatusCode, failureResult)
        );
    }
}
