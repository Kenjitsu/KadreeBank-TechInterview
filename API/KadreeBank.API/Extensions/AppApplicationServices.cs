using KadreeBank.API.Common.Errors;
using KadreeBank.API.Common.Results;
using KadreeBank.API.Data;
using KadreeBank.API.Data.Seeding;
using KadreeBank.API.Interfaces;
using KadreeBank.API.Models;
using KadreeBank.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json.Serialization;

namespace KadreeBank.API.Extensions;

public static class AppApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersConfig();
        services.AddCorsConfig();
        services.AddDbConfig(configuration);

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IPasswordHasher<Customer>, PasswordHasher<Customer>>();

        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IReportService, ReportService>();

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
        services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
            .ConfigureApiBehaviorOptions(options =>
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

    private static IServiceCollection AddCorsConfig(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DevCorsPolicy", policy =>
            {
                policy.AllowAnyOrigin() // solo por temas de desarrollo
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        return services;
    }
}

public static class AppInitializer
{
    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        await DatabaseSeeder.SeedAsync(serviceProvider);
    }
}
