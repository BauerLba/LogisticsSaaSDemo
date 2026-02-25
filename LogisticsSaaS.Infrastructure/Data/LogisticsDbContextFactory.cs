using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LogisticsSaaS.Infrastructure.Data;

public class LogisticsDbContextFactory : IDesignTimeDbContextFactory<LogisticsDbContext>
{
    public LogisticsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LogisticsDbContext>();
        // Using a dummy connection string just for migration generation
        optionsBuilder.UseNpgsql("Host=localhost;Database=dummy;Username=dummy;Password=dummy");

        return new LogisticsDbContext(optionsBuilder.Options);
    }
}
