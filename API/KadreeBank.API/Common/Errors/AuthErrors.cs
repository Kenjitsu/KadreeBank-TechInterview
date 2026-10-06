using KadreeBank.API.Common.Results;

namespace KadreeBank.API.Common.Errors;

public static class AuthErrors
{
    // Mismo error si el documento no existe o si el PIN es incorrecto, para no revelar qué documentos están registrados.
    public static readonly Error InvalidCredentials =
        new("CREDENCIALES_INVALIDAS", "El número de documento o el PIN son incorrectos.");
}
