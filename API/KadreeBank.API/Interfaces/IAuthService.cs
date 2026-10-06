using KadreeBank.API.Common.Results;
using KadreeBank.API.DTOs.Auth;
using KadreeBank.API.DTOs.Customers;

namespace KadreeBank.API.Interfaces;

public interface IAuthService
{
    Task<Result<CustomerResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);
}
