namespace Domain.OAuth;

public record LoginProcessIdentifier(
    Guid State,
    string Ip,
    string UserAgent) : IClientInfo;