using Journey.Application.Abstractions.Messaging;
using Journey.Application.DTOs;

namespace Journey.Application.Journys.AddJourny;

public record class CreateJourneyCommand(CreateJourneyDTO dto) : ICommand<Guid>;