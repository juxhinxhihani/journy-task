using Journey.Domain.OutboxMessages;
using Journey.Domain.OutboxMessages.Interface;
using Microsoft.EntityFrameworkCore;

namespace Journey.Infrastructure.Data.Repositories;

public class OutboxMessageRepository : Repository<OutboxMessage,Guid>, IOutboxMessageRepository
{
    public OutboxMessageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync()
    {
        return await _context.OutboxMessages
            .Where(x => !x.Processed)
            .OrderBy(x => x.OccurredOnUtc)
            .ToListAsync();
    }

    public async Task MarkAsProcessedAsync(Guid id)
    {
        var message = await _context.OutboxMessages
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (message != null)
        {
            message.MarkAsProcessed();
            _context.OutboxMessages.Update(message);
        }
    }
}