namespace Journey.Domain.OutboxMessages.Interface;

public interface IOutboxMessageRepository
{
    void Add(OutboxMessage crmPatient);
    void Update(OutboxMessage crmPatient);
    void Delete(OutboxMessage crmPatient);
    Task<OutboxMessage?> Get(Guid id);

    Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync();
    Task MarkAsProcessedAsync(Guid id);
}