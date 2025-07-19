using Journey.Application.Abstractions.Messaging;

namespace Journey.Application.Journys.RevokePublicLink;

public record class RevokeJourneyPublicLinkCommand(Guid JourneyId) : ICommand;