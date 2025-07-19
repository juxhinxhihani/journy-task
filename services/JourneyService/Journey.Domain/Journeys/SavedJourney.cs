using System.ComponentModel.DataAnnotations.Schema;
using Journey.Domain.Abstractions;
using Journey.Domain.Users;

namespace Journey.Domain.Journeys;

public class SavedJourney : BaseAuditableEntity<Guid>
{
    [ForeignKey(nameof(Journey))]
    public Guid JourneyId { get; private set; }
    public Journey Journey { get; private set; }
    
    [ForeignKey(nameof(User))]
    public Guid UserId { get; private set; }
    public User User { get; private set; }

    private SavedJourney() { }
    
    private SavedJourney(Guid id) : base(id) { }
    
    public SavedJourney(Guid journeyId, Guid userId)
    {
        Id = Guid.NewGuid();
        JourneyId = journeyId;
        UserId = userId;
    }
    
    public static SavedJourney Create(Guid journeyId, Guid userId)
    {
        return new SavedJourney(journeyId, userId);
    }
}