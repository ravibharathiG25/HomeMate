namespace HomeMate.Shared.Domain;

/// <summary>
/// Marker interface representing a domain event within an aggregate boundary.
/// </summary>
public interface IDomainEvent
{
    Guid EventId => Guid.NewGuid();
    DateTime OccurredOnUtc => DateTime.UtcNow;
}
