using KadreeBank.API.Models.Enums;

namespace KadreeBank.API.Data.Seeding.DTOs;

// Estructura de Data/seedData.json: clientes -> cuentas -> movimientos.
// No incluye Ids (son IDENTITY) ni saldos: el seeder los calcula a partir de los movimientos.
public class SeedDataDto
{
    public List<SeedCustomerDto> Customers { get; set; } = [];
}

public class SeedCustomerDto
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public CustomerType Type { get; set; }
    public string Pin { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<SeedAccountDto> Accounts { get; set; } = [];
}

public class SeedAccountDto
{
    public string AccountNumber { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public string City { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<SeedTransactionDto> Transactions { get; set; } = [];
}

public class SeedTransactionDto
{
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string City { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
