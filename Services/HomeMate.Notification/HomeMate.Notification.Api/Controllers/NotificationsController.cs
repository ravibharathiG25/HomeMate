using HomeMate.Notification.Application.Common.Interfaces;
using HomeMate.Notification.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HomeMate.Notification.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationLogRepository _repository;
    private readonly IEmailService _emailService;

    public NotificationsController(
        INotificationLogRepository repository,
        IEmailService emailService)
    {
        _repository = repository;
        _emailService = emailService;
    }

    /// <summary>
    /// Gets notification logs for a specific user.
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<NotificationLog>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var logs = await _repository.GetByUserIdAsync(userId, cancellationToken);
        return Ok(logs);
    }

    /// <summary>
    /// Gets the most recent notification logs across the system.
    /// </summary>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(IReadOnlyList<NotificationLog>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecent([FromQuery] int count = 50, CancellationToken cancellationToken = default)
    {
        var logs = await _repository.GetRecentAsync(Math.Clamp(count, 1, 100), cancellationToken);
        return Ok(logs);
    }

    /// <summary>
    /// Dispatches a test email directly (useful for smoke tests).
    /// </summary>
    [HttpPost("test-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SendTestEmail([FromBody] TestEmailRequest request, CancellationToken cancellationToken)
    {
        await _emailService.SendEmailAsync(request.To, request.Subject, request.Body, cancellationToken);

        var log = NotificationLog.CreateEmail(Guid.NewGuid(), request.To, request.Subject, request.Body);
        log.MarkAsSent();
        await _repository.AddAsync(log, cancellationToken);

        return Ok(new { Message = $"Test email sent successfully to {request.To}", LogId = log.Id });
    }
}

public record TestEmailRequest(string To, string Subject, string Body);
