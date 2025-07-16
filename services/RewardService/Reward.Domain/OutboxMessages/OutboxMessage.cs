namespace Reward.Domain.OutboxMessages;

public class OutboxMessage
{
    public Guid Id { get; private set; }

    public string Type { get; private set; }

    public string Payload { get; private set; }

    public DateTime OccurredOn { get; private set; }

    public bool Processed { get; private set; }

    private OutboxMessage() { }

    private OutboxMessage(string type, string payload)
    {
        Id = Guid.NewGuid();
        Type = type;
        Payload = payload;
        OccurredOn = DateTime.UtcNow;
        Processed = false;
    }

    public static OutboxMessage Create(string type, string payload)
    {
        return new OutboxMessage(type, payload);
    }

    public void MarkAsProcessed()
    {
        Processed = true;
    }
}
