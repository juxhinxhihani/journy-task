using Journey.Application.Abstractions.Messaging;

namespace Journey.Application.Journys.DeleteJourney;

public record class DeleteJourneyCommand(Guid id) : ICommand;