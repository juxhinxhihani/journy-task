using Journey.Application.Abstractions.Messaging;
using Journey.Application.Abstractions.Pagination;
using Journey.Application.DTOs.Response;

namespace Journey.Application.Journys.GetJourneys;

public record class GetJourneysQuery : IQuery<PaginatedList<AllJourneysResponse>>, IGetAllBaseRequest
{
    public Guid? UserId { get; init; }
    public string? TransportationType { get; init; }
    public DateTime? StartDateFrom { get; init; }
    public DateTime? StartDateTo { get; init; }
    public DateTime? ArrivalDateFrom { get; init; }
    public DateTime? ArrivalDateTo { get; init; }

    public string? SearchKey { get; init; }
    public string? ColumnName { get; init; }
    public bool IsDescending { get; init; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}