using KadreeBank.API.Common.Results;
using KadreeBank.API.DTOs.Customers;

namespace KadreeBank.API.Interfaces;

public interface ICustomerService
{
    Task<Result<CustomerResponseDto>> CreateCustomerAsync(CreateCustomerRequestDto request, CancellationToken cancellationToken);

    Task<Result<CustomerResponseDto>> GetCustomerAsync(int customerId, CancellationToken cancellationToken);
}
