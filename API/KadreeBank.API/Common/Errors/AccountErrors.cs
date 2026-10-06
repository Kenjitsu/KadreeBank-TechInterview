using KadreeBank.API.Common.Results;

namespace KadreeBank.API.Common.Errors;

public static class AccountErrors
{
    public static readonly Error NotFound =
        new("CUENTA_NO_ENCONTRADA", "La cuenta no existe.");

    public static readonly Error TypeNotAllowed =
        new("TIPO_DE_CUENTA_NO_PERMITIDO", "Las personas naturales solo pueden abrir cuentas de ahorro y las empresas solo pueden abrir cuentas corrientes.");

    public static readonly Error InsufficientFunds =
        new("CUENTA_SALDO_INSUFICIENTE", "El saldo de la cuenta no es suficiente para completar el retiro.");

    public static readonly Error InvalidAmount =
        new("CUENTA_CANTIDAD_INVALIDA", "La cantidad debe ser mayor que cero y tener a lo sumo dos decimales.");
}
