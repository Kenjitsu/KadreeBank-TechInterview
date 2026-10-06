using System.Text.Json;
using System.Text.Json.Serialization;
using KadreeBank.API.Data.Seeding.DTOs;
using KadreeBank.API.Models;
using KadreeBank.API.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KadreeBank.API.Data.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<KadreeBankDbContext>();
        var pinHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Customer>>();
        var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

        await context.Database.MigrateAsync();

        if (await context.Customers.AnyAsync()) return;

        var seedFilePath = Path.Combine(environment.ContentRootPath, "Data", "seedData.json");
        var jsonData = await File.ReadAllTextAsync(seedFilePath);

        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
        var seedData = JsonSerializer.Deserialize<SeedDataDto>(jsonData, options);

        if (seedData == null) return;

        foreach (var customerDto in seedData.Customers)
        {
            var customer = new Customer
            {
                DocumentNumber = customerDto.DocumentNumber,
                FullName = customerDto.FullName,
                Type = customerDto.Type,
                CreatedAt = customerDto.CreatedAt
            };

            customer.PinHash = pinHasher.HashPassword(customer, customerDto.Pin);

            foreach (var accountDto in customerDto.Accounts)
            {
                customer.Accounts.Add(CreateAccount(accountDto));
            }

            context.Customers.Add(customer);
        }

        await context.SaveChangesAsync();
    }

    private static Account CreateAccount(SeedAccountDto accountDto)
    {
        var account = new Account
        {
            AccountNumber = accountDto.AccountNumber,
            Type = accountDto.Type,
            City = accountDto.City,
            Balance = 0m,
            CreatedAt = accountDto.CreatedAt
        };

        foreach (var transactionDto in accountDto.Transactions.OrderBy(t => t.CreatedAt))
        {
            account.Balance = transactionDto.Type == TransactionType.Deposit
                ? account.Balance + transactionDto.Amount
                : account.Balance - transactionDto.Amount;

            if (account.Balance < 0)
                throw new InvalidOperationException(
                    $"El archivo de seed deja la cuenta {accountDto.AccountNumber} con saldo negativo.");

            account.Transactions.Add(new Transaction
            {
                Type = transactionDto.Type,
                Amount = transactionDto.Amount,
                City = transactionDto.City,
                BalanceAfter = account.Balance,
                CreatedAt = transactionDto.CreatedAt
            });
        }

        return account;
    }
}
