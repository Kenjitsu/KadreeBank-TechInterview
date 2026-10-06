using KadreeBank.API.Common.Errors;
using KadreeBank.API.Common.Results;
using KadreeBank.API.Data;
using KadreeBank.API.Interfaces;
using KadreeBank.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace KadreeBank.API.Extensions;

public static class AppApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersConfig();
        services.AddDbConfig(configuration);

        services.AddSingleton(TimeProvider.System);

        services.AddScoped<IAccountService, AccountService>();
        return services;
    }

    private static IServiceCollection AddDbConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("KadreeBank")
            ?? throw new InvalidOperationException("Connection string 'KadreeBank' is not configured.");

        services.AddDbContext<KadreeBankDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    private static IServiceCollection AddControllersConfig(this IServiceCollection services)
    {
        services.AddControllers().ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var validationErrors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                var errorMessage = string.Join(", ", validationErrors);

                var result = Result<object>.Failure(CommonErrors.ValidationError, HttpStatusCode.BadRequest, errorMessage);

                return new BadRequestObjectResult(result);
            };
        });
        return services;
    }
}
