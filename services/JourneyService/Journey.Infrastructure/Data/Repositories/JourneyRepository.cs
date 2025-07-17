using Journey.Domain.Journeys;
using Journey.Domain.Journeys.Interface;

namespace Journey.Infrastructure.Data.Repositories;

public class JourneyRepository : Repository<Domain.Journeys.Journey, Guid>, IJourneyRepository
{
    public JourneyRepository(ApplicationDbContext context) : base(context)
    {
    }
}