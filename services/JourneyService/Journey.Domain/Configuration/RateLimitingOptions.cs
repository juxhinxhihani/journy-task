namespace Journey.Domain.Configuration;

public class RateLimitingOptions
{
    public int TokenLimit { get; set; }
    public int TokensPerPeriod { get; set; }
    public int ReplenishmentPeriodSeconds { get; set; }
}