namespace Journey.Domain.Configuration;

public class JwtConfiguration
{
    public string Secret { get; set; }
    public double LifeSpan { get; set; }
}