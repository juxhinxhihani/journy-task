using Journey.Domain.Journeys;
using Microsoft.AspNetCore.Identity;

namespace Journey.Domain.Users;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; }
    public DateTime DateOfBirth { get; set; }
    public ICollection<JourneyShare> JournyShares { get; set; } = new List<JourneyShare>();
}