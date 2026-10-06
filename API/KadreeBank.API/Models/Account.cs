using KadreeBank.API.Models.Enums;

namespace KadreeBank.API.Models;

public class Account
{
    public int Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public AccountType Type { get; set; }

    public string City { get; set; } = string.Empty;

    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public ICollection<Transaction> Transactions { get; set; } = [];
}
