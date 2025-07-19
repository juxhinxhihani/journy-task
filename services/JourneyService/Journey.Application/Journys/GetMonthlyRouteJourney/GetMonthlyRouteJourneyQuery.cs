using Journey.Application.Abstractions.Messaging;
using Journey.Application.Abstractions.Pagination;
using Journey.Application.DTOs.Response;

namespace Journey.Application.Journys.GetMonthlyRouteJourney;

public class GetMonthlyRouteJourneyQuery: IQuery<PaginatedList<MonthlyRouteDistanceResponse>>, IGetAllBaseRequest
{
    public int? Year { get; init; }
    public int? Month { get; init; }
    public string? SearchKey { get; init; }
    public string? ColumnName { get; init; }
    public bool IsDescending { get; init; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}