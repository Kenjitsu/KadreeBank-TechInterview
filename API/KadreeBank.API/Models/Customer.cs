using KadreeBank.API.Models.Enums;

namespace KadreeBank.API.Models;

public class Customer
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public CustomerType Type { get; set; }

    public string PinHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public ICollection<Account> Accounts { get; set; } = [];
}
