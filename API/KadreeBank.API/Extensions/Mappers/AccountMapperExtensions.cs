using System.Linq.Expressions;
using KadreeBank.API.DTOs.Accounts;
using KadreeBank.API.Models;

namespace KadreeBank.API.Extensions.Mappers;

public static class AccountMapperExtensions
{
    private static readonly Expression<Func<Account, AccountResponseDto>> AccountResponseProjection =
        account => new AccountResponseDto(
            account.Id,
            account.AccountNumber,
            account.CustomerId,
            account.Type,
            account.City,
            account.Balance,
            account.CreatedAt);

    private static readonly Func<Account, AccountResponseDto> AccountResponseMap =
        AccountResponseProjection.Compile();

    public static IQueryable<AccountResponseDto> ProjectToAccountResponseDto(this IQueryable<Account> query)
        => query.Select(AccountResponseProjection);

    public static IQueryable<BalanceResponseDto> ProjectToBalanceResponseDto(this IQueryable<Account> query)
        => query.Select(account => new BalanceResponseDto(
            account.Id,
            account.AccountNumber,
            account.Balance));

    public static AccountResponseDto ToAccountResponseDto(this Account account)
        => AccountResponseMap(account);

    // Una cuenta siempre debe de crearse con un balance de 0.
    public static Account ToAccountEntity(this OpenAccountRequestDto request, string accountNumber, DateTime createdAt)
        => new()
        {
            AccountNumber = accountNumber,
            CustomerId = request.CustomerId,
            Type = request.Type!.Value,
            City = request.City.Trim(),
            Balance = 0m,
            CreatedAt = createdAt
        };
}
