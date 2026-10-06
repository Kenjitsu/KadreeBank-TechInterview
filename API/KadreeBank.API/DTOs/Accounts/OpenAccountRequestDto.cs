using System.ComponentModel.DataAnnotations;
using KadreeBank.API.Models.Enums;

namespace KadreeBank.API.DTOs.Accounts;

public sealed record OpenAccountRequestDto
{
    [Range(1, int.MaxValue)]
    public int CustomerId { get; init; }

    [Required]
    [EnumDataType(typeof(AccountType))]
    public AccountType? Type { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string City { get; init; } = string.Empty;
}
