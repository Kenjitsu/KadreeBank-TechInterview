using System.ComponentModel.DataAnnotations;

namespace KadreeBank.API.DTOs.Transactions;

public sealed record TransactionRequestDto
{
    [Range(typeof(decimal), "0.01", "9999999999999999.99",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true)]
    public decimal Amount { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string City { get; init; } = string.Empty;
}
