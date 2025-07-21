using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Reward.Application.Abstractions.DbContext;
using Reward.Domain.OutboxMessages;
using Reward.Domain.Journeys;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace Reward.Infrastructure;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<Domain.Rewards.Reward> Rewards => Set<Domain.Rewards.Reward>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<Journey> Journeys => Set<Journey>();
    
    public IDbTransaction BeginTransaction()
    {
        var transaction = Database.BeginTransaction().GetDbTransaction();
        return transaction;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.Entity<OutboxMessage>().ToTable("OutboxMessages", t => t.ExcludeFromMigrations());
        builder.Entity<Journey>().ToTable("Journeys", t => t.ExcludeFromMigrations());

        base.OnModelCreating(builder);
    }
}