using System.ComponentModel.DataAnnotations;

namespace KadreeBank.API.DTOs.Transactions;

// Parámetros de ruta: GET /api/Accounts/{accountId}/statements/{year}/{month}
public sealed record MonthlyStatementRequestDto
{
    [Range(2000, 2100, ErrorMessage = "El año debe estar entre 2000 y 2100.")]
    public int Year { get; init; }

    [Range(1, 12, ErrorMessage = "El mes debe estar entre 1 y 12.")]
    public int Month { get; init; }
}
