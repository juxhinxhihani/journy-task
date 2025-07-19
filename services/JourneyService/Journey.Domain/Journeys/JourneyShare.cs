using System.ComponentModel.DataAnnotations.Schema;
using Journey.Domain.Abstractions;
using Journey.Domain.Users;

namespace Journey.Domain.Journeys;

public class JourneyShare : BaseAuditableEntity<Guid>
{
    public JourneyShare() { }
    
    public JourneyShare(Guid id): base(id) { }
    
    private JourneyShare(Guid journeyId, Guid userId)
    {
        JourneyId = journeyId;
        UserId = userId;
    }
    
    [ForeignKey(nameof(Journey))]
    public Guid JourneyId { get; private set; }
    public Journey Journey { get; private set; } 

    [ForeignKey(nameof(User))]
    public Guid UserId { get; private set; }
    public User User { get; private set; } 
    

    public static JourneyShare Create(Guid journeyId, Guid userId)
    {
        return new JourneyShare(journeyId, userId);
    }
}