using KadreeBank.API.Common.Results;

namespace KadreeBank.API.Common.Errors;

public class CommonErrors
{
    public static readonly Error ValidationError =
    new("ERROR_DE_VALIDACION", "Hay uno o más errores de validación.");
}
