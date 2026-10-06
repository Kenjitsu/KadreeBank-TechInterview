using KadreeBank.API.DTOs.Customers;
using KadreeBank.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KadreeBank.API.Controllers;

public class CustomersController(ICustomerService customerService) : BaseApiController
{
    /// <summary>Crea un nuevo cliente con su PIN de acceso.</summary>
    [HttpPost]
    public async Task<IActionResult> CreateCustomer(CreateCustomerRequestDto request, CancellationToken cancellationToken)
    {
        var result = await customerService.CreateCustomerAsync(request, cancellationToken);

        return result.Match(
            onSuccess: successResult => CreatedAtAction(nameof(GetCustomer), new { customerId = successResult.Data!.Id }, successResult),
            onFailure: failureResult => StatusCode(failureResult.StatusCode, failureResult)
        );
    }

    /// <summary>Obtiene un cliente con sus cuentas.</summary>
    [HttpGet("{customerId:int}")]
    public async Task<IActionResult> GetCustomer(int customerId, CancellationToken cancellationToken)
    {
        var result = await customerService.GetCustomerAsync(customerId, cancellationToken);

        return result.Match(
            onSuccess: successResult => Ok(successResult),
            onFailure: failureResult => StatusCode(failureResult.StatusCode, failureResult)
        );
    }
}
