using System.ComponentModel.DataAnnotations;

namespace KadreeBank.API.DTOs.Reports;

// Parámetros de consulta: GET /api/Reports/monthly-transactions?year=2026&month=10
// Si un valor no llega queda en 0 y [Range] lo rechaza, por eso no hace falta int? ni [Required].
public sealed record MonthlyTransactionsReportRequestDto
{
    [Range(2000, 2100, ErrorMessage = "El año es obligatorio y debe estar entre 2000 y 2100.")]
    public int Year { get; init; }

    [Range(1, 12, ErrorMessage = "El mes es obligatorio y debe estar entre 1 y 12.")]
    public int Month { get; init; }
}
