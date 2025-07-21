using Microsoft.EntityFrameworkCore;
using Reward.Domain.Journeys;
using Reward.Domain.OutboxMessages;
using System.Data;

namespace Reward.Application.Abstractions.DbContext;

public interface IApplicationDbContext
{
    DbSet<Domain.Rewards.Reward> Rewards { get; }
    DbSet<OutboxMessage> OutboxMessages { get; }
    DbSet<Journey> Journeys { get; }
    
    IDbTransaction BeginTransaction();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}