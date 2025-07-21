using System.Security.Claims;
using Journey.Domain.Abstractions.Interface;

namespace Journey.API.Services;

public class ActualUser : IActualUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ActualUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? Role => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
}