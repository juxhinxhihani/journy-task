namespace Journey.Application.DTOs.Response;

public class UsersResponse
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } 

    public string LastName { get; set; } 

    public string Email { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string Role { get; set; } 

    public string Status { get; set; } 

    public bool IsDeleted { get; set; }
}

public class UserByIdResponse : UsersResponse
{
    public ICollection<JourneyShareResponse> JourneySharedWithMe { get; set; } = new List<JourneyShareResponse>();
    public ICollection<JourneysResponse> MyJourneys { get; set; } = new List<JourneysResponse>();
}