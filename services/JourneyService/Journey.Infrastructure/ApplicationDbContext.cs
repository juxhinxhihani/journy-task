using System.Data;
using System.Reflection;
using Journey.Application.Abstractions.DbContext;
using Journey.Domain.OutboxMessages;
using Journey.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Journey.Infrastructure;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IApplicationDbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<Domain.Journeys.Journey> Journeys => Set<Domain.Journeys.Journey>();
    public DbSet<Domain.Journeys.JourneyShare> JourneyShare => Set<Domain.Journeys.JourneyShare>();

    
    public IDbTransaction BeginTransaction()
    {
        var transaction = Database.BeginTransaction().GetDbTransaction();
        return transaction;
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.Entity<Domain.Journeys.Journey>().HasQueryFilter(j => !j.IsDeleted);
        base.OnModelCreating(builder);
    }
}