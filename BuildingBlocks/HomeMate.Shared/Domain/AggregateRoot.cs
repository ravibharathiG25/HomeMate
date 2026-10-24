namespace HomeMate.Shared.Domain;

/// <summary>
/// Base class representing an Aggregate Root in Domain-Driven Design.
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId>
{
    public int Version { get; protected set; } = 1;
}

public abstract class AggregateRoot : AggregateRoot<Guid>
{
    protected AggregateRoot()
    {
        Id = Guid.NewGuid();
    }

    protected AggregateRoot(Guid id)
    {
        Id = id;
    }
}
