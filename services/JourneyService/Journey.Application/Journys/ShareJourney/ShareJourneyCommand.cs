using Journey.Application.Abstractions.Messaging;

namespace Journey.Application.Journys.ShareJourney;

public record class ShareJourneyCommand(Guid id, List<Guid> users) : ICommand;