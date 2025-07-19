using Journey.Application.Abstractions.Messaging;

namespace Journey.Application.Journys.ShareJourneyPublicLink;

public record ShareJourneyPublicLinkCommand(Guid JourneyId) : ICommand<string>;