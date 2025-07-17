using System.Data;
using Journey.Domain.OutboxMessages;
using Journey.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.Abstractions.DbContext;

public interface IApplicationDbContext
{
    public DbSet<OutboxMessage> OutboxMessages { get; }
    public DbSet<Domain.Journeys.Journey> Journeys { get; }
    public DbSet<User> Users { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    IDbTransaction BeginTransaction();
}