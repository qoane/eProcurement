using System.Security.Claims;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
public sealed class NotificationsController(INotificationApplicationService notifications, IDeadlineReminderService reminders) : ControllerBase
{
    string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Missing authenticated user identity.");
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpGet("api/notifications/my")]
    public Task<IActionResult> My(CancellationToken ct) => OkAsync(notifications.GetMyNotificationsAsync(UserId, ct));
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpGet("api/notifications/my/unread-count")]
    public async Task<IActionResult> Unread(CancellationToken ct) => Ok(new { count = await notifications.GetUnreadCountAsync(UserId, ct) });
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpPost("api/notifications/{id:guid}/mark-read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct) { await notifications.MarkAsReadAsync(id, UserId, ct); return Ok(new { success = true }); }
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpPost("api/notifications/mark-all-read")]
    public async Task<IActionResult> MarkAll(CancellationToken ct) { await notifications.MarkAllAsReadAsync(UserId, ct); return Ok(new { success = true }); }
    [Authorize(Policy = "NotificationTemplates.Manage")]
    [HttpGet("api/notification-templates")]
    public Task<IActionResult> Templates(CancellationToken ct) => OkAsync(notifications.GetTemplatesAsync(ct));
    [Authorize(Policy = "NotificationTemplates.Manage")]
    [HttpPost("api/notification-templates")]
    public Task<IActionResult> CreateTemplate(SaveNotificationTemplateDto dto, CancellationToken ct) => OkAsync(notifications.SaveTemplateAsync(dto, ct));
    [Authorize(Policy = "NotificationTemplates.Manage")]
    [HttpPut("api/notification-templates/{id:guid}")]
    public Task<IActionResult> UpdateTemplate(Guid id, SaveNotificationTemplateDto dto, CancellationToken ct) => OkAsync(notifications.SaveTemplateAsync(dto with { Id = id }, ct));
    [Authorize(Policy = "NotificationTemplates.Manage")]
    [HttpPost("api/notification-templates/{id:guid}/test")]
    public Task<IActionResult> TestTemplate(Guid id, Dictionary<string,string?> model, CancellationToken ct) => OkAsync(notifications.TestTemplateAsync(id, model, ct));
    [Authorize(Policy = "NotificationLogs.View")]
    [HttpGet("api/notification-delivery-logs")]
    public Task<IActionResult> Logs(CancellationToken ct) => OkAsync(notifications.GetDeliveryLogsAsync(ct));
    [Authorize(Policy = "NotificationLogs.View")]
    [HttpPost("api/notification-delivery-logs/{id:guid}/retry")]
    public Task<IActionResult> Retry(Guid id, CancellationToken ct) => OkAsync(notifications.RetryDeliveryAsync(id, ct));
    [Authorize(Policy = "NotificationEventMappings.Manage")]
    [HttpGet("api/notification-event-mappings")]
    public Task<IActionResult> EventMappings(CancellationToken ct) => OkAsync(notifications.GetEventMappingsAsync(ct));
    [Authorize(Policy = "NotificationEventMappings.Manage")]
    [HttpPost("api/notification-event-mappings")]
    public Task<IActionResult> CreateEventMapping(Lca.EProcurement.Domain.NotificationEventMapping dto, CancellationToken ct) => OkAsync(notifications.SaveEventMappingAsync(dto, ct));
    [Authorize(Policy = "NotificationEventMappings.Manage")]
    [HttpPut("api/notification-event-mappings/{id:guid}")]
    public Task<IActionResult> UpdateEventMapping(Guid id, Lca.EProcurement.Domain.NotificationEventMapping dto, CancellationToken ct) => OkAsync(notifications.SaveEventMappingAsync(dto with { Id = id }, ct));
    [Authorize(Policy = "DeadlineReminders.Manage")]
    [HttpPost("api/notifications/reminders/run")]
    public Task<IActionResult> RunReminders(CancellationToken ct) => OkAsync(reminders.RunDueRemindersAsync(ct));
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpGet("api/notification-preferences/my")]
    public Task<IActionResult> Preferences(CancellationToken ct) => OkAsync(notifications.GetPreferencesAsync(UserId, ct));
    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpPut("api/notification-preferences/my")]
    public Task<IActionResult> SavePreferences(List<NotificationPreferenceDto> dto, CancellationToken ct) => OkAsync(notifications.SavePreferencesAsync(UserId, dto, ct));
    static async Task<IActionResult> OkAsync<T>(Task<T> task) => new OkObjectResult(await task);
}
