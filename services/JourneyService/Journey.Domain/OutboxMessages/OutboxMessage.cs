namespace Journey.Domain.OutboxMessages;

public class OutboxMessage
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Type { get; private set; }
    public string Payload { get; private set; } 
    public DateTime OccurredOnUtc { get; private set; }
    public DateTime? ProcessedOnUtc { get; private set; }
    public bool Processed { get; private set; } 

    private OutboxMessage() { }

    public OutboxMessage(string type, string payload)
    {
        Type = type;
        Payload = payload;
    }

    public void MarkAsProcessed()
    {
        Processed = true;
        ProcessedOnUtc = DateTime.UtcNow;
    }
}