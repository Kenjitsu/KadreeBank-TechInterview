using KadreeBank.API.Models.Enums;

namespace KadreeBank.API.Models;

public class Transaction
{
    public long Id { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }

    public string City { get; set; } = string.Empty;

    public decimal BalanceAfter { get; set; }

    public DateTime CreatedAt { get; set; }

    public int AccountId { get; set; }
    public Account Account { get; set; } = null!;
}
