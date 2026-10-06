using KadreeBank.API.Common.Results;

namespace KadreeBank.API.Common.Errors;

public static class CustomerErrors
{
    public static readonly Error NotFound =
        new("CLIENTE_NO_ENCONTRADO", "El cliente no existe.");

    public static readonly Error DocumentAlreadyExists =
        new("CLIENTE_DOCUMENTO_YA_EXISTE", "Ya existe un cliente con el mismo número de documento.");
}
