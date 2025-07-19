using Journey.Application.Abstractions.Messaging;
using Journey.Application.DTOs.Response;

namespace Journey.Application.Journys.GetJourneyByPublicLinkToken;

public record GetPublicJourneysByTokenQuery(string Token) : IQuery<JourneysResponse>;