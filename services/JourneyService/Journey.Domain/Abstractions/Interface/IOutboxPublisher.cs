using Journey.Domain.OutboxMessages;

namespace Journey.Domain.Abstractions.Interface;

public interface IOutboxPublisher
{
    Task PublishAsync(string eventType, string payload);
    Task PublishAsync(OutboxMessage message);
}