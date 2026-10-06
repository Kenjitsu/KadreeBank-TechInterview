using KadreeBank.API.Common.Results;
using KadreeBank.API.DTOs.Accounts;
using KadreeBank.API.DTOs.Transactions;

namespace KadreeBank.API.Interfaces;

public interface IAccountService
{
    Task<Result<AccountResponseDto>> OpenAccountAsync(OpenAccountRequestDto request, CancellationToken cancellationToken);

    Task<Result<AccountResponseDto>> GetAccountAsync(int accountId, CancellationToken cancellationToken);

    Task<Result<BalanceResponseDto>> GetBalanceAsync(int accountId, CancellationToken cancellationToken);

    Task<Result<TransactionResponseDto>> DepositAsync(int accountId, TransactionRequestDto request, CancellationToken cancellationToken);

    Task<Result<TransactionResponseDto>> WithdrawAsync(int accountId, TransactionRequestDto request, CancellationToken cancellationToken);
}
