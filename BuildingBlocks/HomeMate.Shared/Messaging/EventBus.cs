using MassTransit;
using Microsoft.Extensions.Logging;

namespace HomeMate.Shared.Messaging;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : class, IIntegrationEvent;

    Task SendAsync<T>(T command, string queueName, CancellationToken cancellationToken = default)
        where T : class;
}

public class EventBus : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly ILogger<EventBus> _logger;

    public EventBus(
        IPublishEndpoint publishEndpoint,
        ISendEndpointProvider sendEndpointProvider,
        ILogger<EventBus> logger)
    {
        _publishEndpoint = publishEndpoint;
        _sendEndpointProvider = sendEndpointProvider;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : class, IIntegrationEvent
    {
        _logger.LogInformation(
            "Publishing integration event {EventType} with ID {EventId} at {Timestamp}",
            @event.EventType,
            @event.EventId,
            @event.OccurredOnUtc);

        try
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(3));

            await _publishEndpoint.Publish(@event, timeoutCts.Token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Could not deliver integration event {EventType} to RabbitMQ within timeout. Broker may be offline.",
                @event.EventType);
        }
    }

    public async Task SendAsync<T>(T command, string queueName, CancellationToken cancellationToken = default)
        where T : class
    {
        _logger.LogInformation(
            "Sending command {CommandType} to queue {QueueName}",
            typeof(T).Name,
            queueName);

        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));
        await endpoint.Send(command, cancellationToken);
    }
}
