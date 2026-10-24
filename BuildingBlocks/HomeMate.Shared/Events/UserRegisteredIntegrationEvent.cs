using HomeMate.Shared.Messaging;

namespace HomeMate.Shared.Events;

/// <summary>
/// Published by HomeMate.Auth when a new user successfully registers.
/// </summary>
public record UserRegisteredIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime RegisteredAtUtc { get; init; } = DateTime.UtcNow;

    public UserRegisteredIntegrationEvent() { }

    public UserRegisteredIntegrationEvent(
        Guid userId,
        string email,
        string firstName,
        string lastName,
        string role)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Role = role;
        RegisteredAtUtc = DateTime.UtcNow;
    }
}
