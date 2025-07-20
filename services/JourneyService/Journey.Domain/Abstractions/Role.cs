using Microsoft.AspNetCore.Identity;

namespace Journey.Domain.Abstractions;

public class Role : IdentityRole<Guid>
{
    public Role() : base() { }

    public Role(string roleName) : base(roleName)
    {
        Id = Guid.NewGuid();
    }
}