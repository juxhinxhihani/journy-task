namespace Journey.Application.DTOs;

public class CreateJourneyDTO
{
    public string StartLocation { get; set; } 
    public DateTime StartTime { get; set; }
    public string ArrivalLocation { get; set; } 
    public DateTime ArrivalTime { get; set; }
    public string TransportationType { get; set; } 
    public decimal RouteDistanceKm { get; set; }
}