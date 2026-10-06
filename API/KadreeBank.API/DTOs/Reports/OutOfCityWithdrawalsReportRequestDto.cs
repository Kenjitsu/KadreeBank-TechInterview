using System.ComponentModel.DataAnnotations;

namespace KadreeBank.API.DTOs.Reports;

// Parámetros de consulta opcionales: GET /api/Reports/out-of-city-withdrawals?year=2026&month=10
// Son int? porque null significa "sin filtro" (todo el histórico o el año completo).
public sealed record OutOfCityWithdrawalsReportRequestDto
{
    [Range(2000, 2100, ErrorMessage = "El año debe estar entre 2000 y 2100.")]
    public int? Year { get; init; }

    [Range(1, 12, ErrorMessage = "El mes debe estar entre 1 y 12.")]
    public int? Month { get; init; }
}
