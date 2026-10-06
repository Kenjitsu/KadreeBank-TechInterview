using System.ComponentModel.DataAnnotations;

namespace KadreeBank.API.DTOs.Transactions;

// Parámetros de consulta: GET /api/Accounts/{accountId}/transactions?take=10
public sealed record RecentTransactionsRequestDto
{
    // Si no se envía, se usan los 10 más recientes.
    [Range(1, 100, ErrorMessage = "La cantidad de movimientos debe estar entre 1 y 100.")]
    public int Take { get; init; } = 10;
}
