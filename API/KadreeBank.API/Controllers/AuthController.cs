using KadreeBank.API.DTOs.Auth;
using KadreeBank.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KadreeBank.API.Controllers;

public class AuthController(IAuthService authService) : BaseApiController
{
    /// <summary>Ingreso simulado con número de documento y PIN. Devuelve el cliente con sus cuentas.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);

        return result.Match(
            onSuccess: successResult => Ok(successResult),
            onFailure: failureResult => StatusCode(failureResult.StatusCode, failureResult)
        );
    }
}
