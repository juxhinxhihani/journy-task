using Journey.Application.Abstractions.Messaging;
using Journey.Application.Abstractions.Pagination;
using Journey.Application.DTOs.Response;

namespace Journey.Application.Journys.GetPublicJourneys;

public record class GetPublicJourneysQuery : IQuery<PaginatedList<JourneysResponse>>, IGetAllBaseRequest
{
    public string? SearchKey { get; init; }
    public string? ColumnName { get; init; }
    public bool IsDescending { get; init; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}