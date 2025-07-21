using System.Data;
using System.Reflection;
using Journey.Application.Abstractions.DbContext;
using Journey.Domain.Abstractions;
using Journey.Domain.OutboxMessages;
using Journey.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Journey.Infrastructure;

public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>, IApplicationDbContext
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
        builder.Entity<Domain.Journeys.JourneyShare>().HasQueryFilter(j => !j.Journey.IsDeleted);
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.ClrType.GetProperties())
            {
                if (property.PropertyType == typeof(DateTime))
                {
                    builder.Entity(entityType.ClrType)
                        .Property(property.Name)
                        .HasConversion(dateTimeConverter);
                }
                else if (property.PropertyType == typeof(DateTime?))
                {
                    builder.Entity(entityType.ClrType)
                        .Property(property.Name)
                        .HasConversion(nullableDateTimeConverter);
                }
            }
        }
        
        base.OnModelCreating(builder);
    }
}