using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Journey.Application.Abstractions.Pagination;

public static class MappingExtensions
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable, int pageNumber, int pageSize)
        where TDestination : class
        => PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize);

    public static PaginatedList<TDestination> PaginatedListAsync<TDestination>(
        this IOrderedEnumerable<TDestination> list, int pageNumber, int pageSize)
        where TDestination : class
    => PaginatedList<TDestination>.Create(list, pageNumber, pageSize);
    
    public static IOrderedQueryable<T> SortAndOrderBy<T>(this IQueryable<T> source, string? sortBy, bool isDescending)
    {

        if (string.IsNullOrEmpty(sortBy))
        {
            sortBy = "Id";
        }

        var param = Expression.Parameter(typeof(T), "item");

        try
        {
            var keySelector = Expression.Lambda<Func<T, object>>
            (Expression.Convert(Expression.Property(param, sortBy.ToLower()), typeof(object)), param);

            if (isDescending)
            {
                return source.OrderByDescending(keySelector);
            }
            else
            {
                return source.OrderBy(keySelector);
            }
        }
        catch (Exception)
        {
            throw new ApplicationException($"You cannot sort by property {sortBy} because it does not exist");
        }

    }

    public static IOrderedEnumerable<T> SortAndOrderBy<T>(this IEnumerable<T> source, string? sortBy, bool isDescending)
    {
        if (string.IsNullOrEmpty(sortBy))
        {
            sortBy = "Id"; // Default to "Id" if no column is provided
        }

        var param = Expression.Parameter(typeof(T), "item");

        try
        {
            var property = typeof(T).GetProperty(sortBy);
            if (property == null)
            {
                throw new ApplicationException($"You cannot sort by property '{sortBy}' because it does not exist.");
            }

            Func<T, object> keySelector = item => property.GetValue(item);

            return isDescending ? source.OrderByDescending(keySelector) : source.OrderBy(keySelector);
        }
        catch (Exception)
        {
            throw new ApplicationException($"You cannot sort by property '{sortBy}' because it does not exist or is invalid.");
        }
    }


}