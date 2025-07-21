using Journey.Infrastructure;
using Journey.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Journey.API.Extensions;

public static class StartupTaskExtensions
{
    public static async Task RunStartupTasksAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var serviceProvider = scope.ServiceProvider;

        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        await DbSeeder.SeedRolesAsync(serviceProvider);
    }
}