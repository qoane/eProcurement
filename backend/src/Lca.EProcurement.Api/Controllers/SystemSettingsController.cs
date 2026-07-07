using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[Route("api/settings")]
public sealed class SystemSettingsController(ISystemSettingsApplicationService settings) : ControllerBase
{
    [Authorize(Policy = "Settings.View")]
    [HttpGet]
    public Task<IActionResult> Get(CancellationToken ct) => OkAsync(settings.GetEffectiveSettingsAsync(ct));
    [Authorize(Policy = "Settings.View")]
    [HttpGet("{category}")]
    public Task<IActionResult> GetCategory(string category, CancellationToken ct) => OkAsync(settings.GetSettingsByCategoryAsync(category, ct));
    [Authorize(Policy = "Settings.Manage")]
    [HttpPut("{key}")]
    public Task<IActionResult> Save(string key, SettingDto dto, CancellationToken ct) => OkAsync(settings.SaveSettingOverrideAsync(Uri.UnescapeDataString(key), dto.Value, dto.IsSecret, dto.Category, "admin", ct));
    [Authorize(Policy = "Settings.Manage")]
    [HttpDelete("{key}")]
    public async Task<IActionResult> Delete(string key, CancellationToken ct) { await settings.DeleteSettingOverrideAsync(Uri.UnescapeDataString(key), ct); return Ok(new { success = true }); }
    [Authorize(Policy = "Settings.Manage")]
    [HttpPost("test-email")]
    public Task<IActionResult> TestEmail(Dictionary<string,string> dto, CancellationToken ct) => OkAsync(settings.TestEmailSettingsAsync(dto.GetValueOrDefault("to") ?? "admin@example.test", ct));
    [Authorize(Policy = "Settings.Manage")]
    [HttpPost("test-sms")]
    public Task<IActionResult> TestSms(Dictionary<string,string> dto, CancellationToken ct) => OkAsync(settings.TestSmsSettingsAsync(dto.GetValueOrDefault("destinationAddress") ?? dto.GetValueOrDefault("phoneNumber") ?? "+26600000000", ct));
    static async Task<IActionResult> OkAsync<T>(Task<T> task) => new OkObjectResult(await task);
}
