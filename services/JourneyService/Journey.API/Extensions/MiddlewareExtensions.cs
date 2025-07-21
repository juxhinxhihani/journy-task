using CorrelationId;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Journey.API.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseExceptionHandler();
        app.UseCors("AllowOrigin");
        app.UseRateLimiter();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseHealthChecksUI(config =>
        {
            config.UIPath = "/health-ui";
        });
        app.UseCorrelationId();

        return app;
    }
}