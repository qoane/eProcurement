using System.Security.Claims;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
public sealed class NotificationsController(INotificationApplicationService notifications) : ControllerBase
{
    string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name ?? "demo-user";
    [HttpGet("api/notifications/my")]
    public Task<IActionResult> My(CancellationToken ct) => OkAsync(notifications.GetMyNotificationsAsync(UserId, ct));
    [HttpGet("api/notifications/my/unread-count")]
    public async Task<IActionResult> Unread(CancellationToken ct) => Ok(new { count = await notifications.GetUnreadCountAsync(UserId, ct) });
    [HttpPost("api/notifications/{id:guid}/mark-read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct) { await notifications.MarkAsReadAsync(id, UserId, ct); return Ok(new { success = true }); }
    [HttpPost("api/notifications/mark-all-read")]
    public async Task<IActionResult> MarkAll(CancellationToken ct) { await notifications.MarkAllAsReadAsync(UserId, ct); return Ok(new { success = true }); }
    [Authorize(Policy = "Notifications.Manage")]
    [HttpGet("api/notification-templates")]
    public Task<IActionResult> Templates(CancellationToken ct) => OkAsync(notifications.GetTemplatesAsync(ct));
    [Authorize(Policy = "Notifications.Manage")]
    [HttpPost("api/notification-templates")]
    public Task<IActionResult> CreateTemplate(SaveNotificationTemplateDto dto, CancellationToken ct) => OkAsync(notifications.SaveTemplateAsync(dto, ct));
    [Authorize(Policy = "Notifications.Manage")]
    [HttpPut("api/notification-templates/{id:guid}")]
    public Task<IActionResult> UpdateTemplate(Guid id, SaveNotificationTemplateDto dto, CancellationToken ct) => OkAsync(notifications.SaveTemplateAsync(dto with { Id = id }, ct));
    [Authorize(Policy = "Notifications.Manage")]
    [HttpPost("api/notification-templates/{id:guid}/test")]
    public Task<IActionResult> TestTemplate(Guid id, Dictionary<string,string?> model, CancellationToken ct) => OkAsync(notifications.TestTemplateAsync(id, model, ct));
    [HttpGet("api/notification-delivery-logs")]
    public Task<IActionResult> Logs(CancellationToken ct) => OkAsync(notifications.GetDeliveryLogsAsync(ct));
    [HttpGet("api/notification-preferences/my")]
    public Task<IActionResult> Preferences(CancellationToken ct) => OkAsync(notifications.GetPreferencesAsync(UserId, ct));
    [HttpPut("api/notification-preferences/my")]
    public Task<IActionResult> SavePreferences(List<NotificationPreferenceDto> dto, CancellationToken ct) => OkAsync(notifications.SavePreferencesAsync(UserId, dto, ct));
    static async Task<IActionResult> OkAsync<T>(Task<T> task) => new OkObjectResult(await task);
}
