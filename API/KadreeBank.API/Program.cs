using KadreeBank.API.Extensions;
using KadreeBank.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "KadreeBank API v1"));
}

app.UseHttpsRedirection();

app.UseCors("DevCorsPolicy");

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    await AppInitializer.InitializeDatabaseAsync(scope.ServiceProvider);
}

app.Run();
