using Journey.Application.Abstractions.Messaging;

namespace Journey.Application.Journys.ShareJourney;

public record class ShareJourneyCommand(Guid Id, List<Guid> Users) : ICommand;