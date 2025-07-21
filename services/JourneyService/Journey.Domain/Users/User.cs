using Journey.Domain.Abstractions;
using Journey.Domain.Abstractions.Interface;
using Journey.Domain.Journeys;
using Journey.Domain.Users.Events;
using Microsoft.AspNetCore.Identity;

namespace Journey.Domain.Users;

public class User : IdentityEntity
{
    public string FirstName { get; set; } 
    public string LastName { get; set; } 
    public DateTime DateOfBirth { get; set; }
    public string Role { get; set; }
    public UserStatus Status { get; set; }
    public bool IsLocked { get; private set; } = false;
    public int LoginRetry { get; private set; } = 0;
    public bool IsDeleted { get; private set; }
    public ICollection<JourneyShare> JournyShares { get; set; } = new List<JourneyShare>();
    public ICollection<Journeys.Journey> Journys { get; set; } = new List<Journeys.Journey>();

    private User() { }

    public User(string firstName, string lastName, string email, string role, DateTime dateOfBirth, UserStatus userStatus, string securityStamp) 
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Email = email;
        UserName = email;
        Role = role;
        Status = userStatus;
        IsDeleted = false;
        SecurityStamp = securityStamp;
    }
    
    public static User Create(string firstName, string lastName, string email, string role,DateTime dateOfBirth)
    {
        var user = new User(firstName, lastName, email, role, dateOfBirth, UserStatus.Suspended, Guid.NewGuid().ToString());
        
        return user;
    }
    public void Update(string firstName, string lastName, DateTime dateOfBirth, string role)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Role = role;
    }
    public void ChangeStatus(Guid userId, string email, UserStatus oldStatus, UserStatus newStatus)
    {
        Status = newStatus;
        RaiseDomainEvent(new UserStatusChangedDomainEvent(email, oldStatus, newStatus));
    }
    
    public static void SendConfirmEmail(User user, string token)
    {
        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Email, user.Id, token));
    }
    public static User ConfirmEmail(User user)
    {
        user.Status = UserStatus.Active;
        return user;
    }
    public static User ResendConfirmEmail(User user, string token)
    {
        user.Status = UserStatus.Suspended;
        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Email, user.Id, token));

        return user;
    }
    public void Delete()
    {
        IsDeleted = true;
        Status = UserStatus.Deactivated;
    }
    
    public void UnlockUser()
    {
        IsLocked = false;
        LoginRetry = 0;
    }
    public void RetryLogin(int? numberOfRetry)
    {
        if (numberOfRetry is null)
        {
            LoginRetry = 1;
        }
        else if (numberOfRetry == 2)
        {
            IsLocked = true;
            LoginRetry = 3;
        }
        else
        {
            LoginRetry = LoginRetry + 1;
        }
    }
    public void UnDelete()
    {
        IsDeleted = false;
    }

    public void DailyGoalAchieved(User user, decimal totalDistance, DateTime messageAchievedOn)
    {
        user.RaiseDomainEvent(new DailyGoalAchievedDomain(user.Email, totalDistance, messageAchievedOn));
    }
}