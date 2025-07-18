using Journey.Domain.Journeys;
using Microsoft.AspNetCore.Identity;

namespace Journey.Domain.Users;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Role { get; set; }
    public bool IsDeleted { get; private set; } = false;
    public bool IsActive { get; set; } = true;
    public ICollection<JourneyShare> JournyShares { get; set; } = new List<JourneyShare>();

    public User(string firstName, string lastName, DateTime dateOfBirth, string role)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Role = role;
        IsActive = true;
        IsDeleted = false;
    }

    public static User Create(string firstName, string lastName, DateTime dateOfBirth, string role, Action<User> raiseDomainEvent)
    {
        var user = new User(firstName, lastName, dateOfBirth, role);

        raiseDomainEvent?.Invoke(user);

        return user;
    }
    public void Update(string firstName, string lastName, DateTime dateOfBirth, string role)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Role = role;
    }
    public void Delete()
    {
        IsDeleted = true;
    }
}