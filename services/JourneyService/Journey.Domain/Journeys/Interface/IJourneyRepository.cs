namespace Journey.Domain.Journeys.Interface;

public interface IJourneyRepository
{
    void Add(Journey crmPatient);
    void Update(Journey crmPatient);
    void Delete(Journey crmPatient);
    Task<Journey?> Get(Guid id);
}