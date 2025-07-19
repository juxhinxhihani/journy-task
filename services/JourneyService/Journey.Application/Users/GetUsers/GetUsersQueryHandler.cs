using Journey.Application.Abstractions.DbContext;
using Journey.Application.Abstractions.Messaging;
using Journey.Application.Abstractions.Pagination;
using Journey.Application.DTOs.Response;
using Journey.Domain.Abstractions;
using Journey.Domain.Users;

namespace Journey.Application.Users.GetUsers;

public class GetUsersQueryHandler(
    IUserRepository _userRepository,
    IApplicationDbContext _context) 
    : IQueryHandler<GetUsersQuery, PaginatedList<UsersResponse>>
{ 
    
    public async Task<Result<PaginatedList<UsersResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = _context.Users.AsQueryable();       
        if (!string.IsNullOrWhiteSpace(request.SearchKey))
        {
            users = users.Where(u =>
                u.FirstName.Contains(request.SearchKey) ||
                u.LastName.Contains(request.SearchKey) ||
                u.Email.Contains(request.SearchKey));
        }
        var result = await users
            .SortAndOrderBy(request.ColumnName, request.IsDescending)
            .Select(u => new UsersResponse
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                DateOfBirth = u.DateOfBirth,
                Role = u.Role.ToString(),
                IsDeleted = u.IsDeleted
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize);
        
        return result;
    }
}