namespace Journey.Application.DTOs.Response;

public sealed class MonthlyRouteDistanceResponse
{
    public Guid UserId { get; set; }
    public string? FullName { get; set; }
    public decimal TotalDistanceKm { get; set; }
}