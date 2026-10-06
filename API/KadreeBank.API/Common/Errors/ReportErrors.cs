using KadreeBank.API.Common.Results;

namespace KadreeBank.API.Common.Errors;

public static class ReportErrors
{
    public static readonly Error InvalidPeriod =
        new("REPORTE_PERIODO_INVALIDO", "Para filtrar por mes también se debe indicar el año.");
}
