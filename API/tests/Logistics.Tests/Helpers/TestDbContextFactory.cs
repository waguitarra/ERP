using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Tests.Helpers;

public static class TestDbContextFactory
{
    public static LogisticsDbContext Create()
    {
        var options = new DbContextOptionsBuilder<LogisticsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        var context = new LogisticsDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    public static LogisticsDbContext CreateWithData()
    {
        var context = Create();
        SeedTestData(context);
        return context;
    }

    private static void SeedTestData(LogisticsDbContext context)
    {
        // Seed será adicionado conforme necessidade nos testes
    }
}
