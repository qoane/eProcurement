using System.Net;
using System.Net.Mail;
using System.Net.Http.Json;
using System.Text.Json;
using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lca.EProcurement.Application;

public sealed record NotificationRecipientDto(string UserId, string Name, string? Email = null, string? PhoneNumber = null, string RecipientType = "User", string? RoleCode = null);
public sealed record SaveNotificationTemplateDto(Guid? Id, string Code, string Name, string Description, string EventCode, NotificationChannel Channel, string SubjectTemplate, string BodyTemplate, bool IsActive = true);
public sealed record NotificationPreferenceDto(string EventCode, bool InAppEnabled, bool EmailEnabled, bool SmsEnabled);
public sealed record SettingDto(string Key, string Value, bool IsSecret, string Category, bool IsOverridden);
public sealed record SendResult(bool Success, string Response, string? Error = null, JsonElement? RawJson = null);
public sealed record SmtpSettings(string? Host, int Port, string? UserName, string? Password, string? From, bool EnableSsl, bool IsEnabled);
public sealed record SmsSettings(string? BaseUrl, string? ApiKey, string? ApiKeyHeaderName, string? AccountId, string? ClientReferencePrefix, bool IsEnabled);

public interface INotificationApplicationService
{
    Task SendAsync(string eventCode, string entityType, Guid? entityId, object model, List<NotificationRecipientDto> recipients, CancellationToken ct = default);
    Task QueueAsync(string eventCode, string entityType, Guid? entityId, object model, List<NotificationRecipientDto> recipients, CancellationToken ct = default);
    Task<NotificationMessage> SendEmailAsync(string subject, string body, NotificationRecipientDto recipient, CancellationToken ct = default);
    Task<NotificationMessage> SendSmsAsync(string body, NotificationRecipientDto recipient, CancellationToken ct = default);
    Task<NotificationMessage> CreateInAppAsync(string eventCode, string entityType, Guid? entityId, string subject, string body, NotificationPriority priority, string? relatedUrl, NotificationRecipientDto recipient, CancellationToken ct = default);
    Task<List<NotificationMessage>> GetMyNotificationsAsync(string userId, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(string userId, CancellationToken ct = default);
    Task MarkAsReadAsync(Guid notificationId, string userId, CancellationToken ct = default);
    Task MarkAllAsReadAsync(string userId, CancellationToken ct = default);
    Task<List<NotificationTemplate>> GetTemplatesAsync(CancellationToken ct = default);
    Task<NotificationTemplate> SaveTemplateAsync(SaveNotificationTemplateDto dto, CancellationToken ct = default);
    Task<string> TestTemplateAsync(Guid id, Dictionary<string,string?> model, CancellationToken ct = default);
    Task<List<NotificationDeliveryLog>> GetDeliveryLogsAsync(CancellationToken ct = default);
    Task<NotificationDeliveryLog> RetryDeliveryAsync(Guid id, CancellationToken ct = default);
    Task<List<NotificationEventMapping>> GetEventMappingsAsync(CancellationToken ct = default);
    Task<NotificationEventMapping> SaveEventMappingAsync(NotificationEventMapping dto, CancellationToken ct = default);
    Task<List<NotificationPreferenceDto>> GetPreferencesAsync(string userId, CancellationToken ct = default);
    Task<List<NotificationPreferenceDto>> SavePreferencesAsync(string userId, List<NotificationPreferenceDto> preferences, CancellationToken ct = default);
}
public interface INotificationSender { Task SendAsync(string eventCode, string entityType, Guid? entityId, object model, List<NotificationRecipientDto> recipients, CancellationToken ct = default); }
public interface IEmailSender { Task<SendResult> SendEmailAsync(string to, string subject, string body, CancellationToken ct = default); }
public interface ISmsSender { Task<SendResult> SendSmsAsync(string phoneNumber, string message, CancellationToken ct = default); }
public interface IInAppNotificationService { Task<NotificationMessage> CreateInAppAsync(string eventCode, string entityType, Guid? entityId, string subject, string body, NotificationPriority priority, string? relatedUrl, NotificationRecipientDto recipient, CancellationToken ct = default); }
public interface ISystemSettingsApplicationService
{
    Task<List<SettingDto>> GetEffectiveSettingsAsync(CancellationToken ct = default);
    Task<List<SettingDto>> GetSettingsByCategoryAsync(string category, CancellationToken ct = default);
    Task<SettingDto> SaveSettingOverrideAsync(string key, string value, bool isSecret, string category, string updatedBy = "admin", CancellationToken ct = default);
    Task DeleteSettingOverrideAsync(string key, CancellationToken ct = default);
    Task<SendResult> TestEmailSettingsAsync(string to, CancellationToken ct = default);
    Task<SendResult> TestSmsSettingsAsync(string phoneNumber, CancellationToken ct = default);
    Task<SmtpSettings> GetSmtpSettingsAsync(CancellationToken ct = default);
    Task<SmsSettings> GetSmsSettingsAsync(CancellationToken ct = default);
}

public sealed class SystemSettingsApplicationService(EProcurementDbContext db, IConfiguration configuration) : ISystemSettingsApplicationService
{
    static readonly (string Key,string Category,bool Secret)[] Keys = [
        ("SmtpSettings:Host","Email Settings",false),("SmtpSettings:Port","Email Settings",false),("SmtpSettings:UserName","Email Settings",false),("SmtpSettings:Password","Email Settings",true),("SmtpSettings:From","Email Settings",false),("SmtpSettings:EnableSsl","Email Settings",false),("SmtpSettings:IsEnabled","Email Settings",false),
        ("SmsSettings:BaseUrl","SMS Settings",false),("SmsSettings:ApiKey","SMS Settings",true),("SmsSettings:ApiKeyHeaderName","SMS Settings",false),("SmsSettings:AccountId","SMS Settings",false),("SmsSettings:ClientReferencePrefix","SMS Settings",false),("SmsSettings:IsEnabled","SMS Settings",false),
        ("NotificationSettings:DefaultChannels","Notification Settings",false),("NotificationSettings:EnableInApp","Notification Settings",false),("NotificationSettings:EnableEmail","Notification Settings",false),("NotificationSettings:EnableSms","Notification Settings",false),
        ("SystemSettings:ApplicationName","System Settings",false),("SystemSettings:TenantName","System Settings",false),("SystemSettings:DefaultPortalUrl","System Settings",false),("SystemSettings:SupportEmail","System Settings",false),("SystemSettings:TimeZone","System Settings",false)];
    public async Task<List<SettingDto>> GetEffectiveSettingsAsync(CancellationToken ct = default)
    {
        var overrides = await db.SystemSettingOverrides.AsNoTracking().ToDictionaryAsync(x => x.Key, ct);
        return Keys.Select(k => { var ov = overrides.GetValueOrDefault(k.Key); var v = ov?.Value ?? configuration[k.Key] ?? ""; return new SettingDto(k.Key, (ov?.IsSecret ?? k.Secret) && !string.IsNullOrWhiteSpace(v) ? "********" : v, ov?.IsSecret ?? k.Secret, ov?.Category ?? k.Category, ov is not null); }).ToList();
    }
    public async Task<List<SettingDto>> GetSettingsByCategoryAsync(string category, CancellationToken ct = default) => (await GetEffectiveSettingsAsync(ct)).Where(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
    public async Task<SettingDto> SaveSettingOverrideAsync(string key, string value, bool isSecret, string category, string updatedBy = "admin", CancellationToken ct = default)
    { var existing = await db.SystemSettingOverrides.SingleOrDefaultAsync(x => x.Key == key, ct); if (existing is null) db.SystemSettingOverrides.Add(new(key, value, isSecret, category, updatedBy, DateTimeOffset.UtcNow)); else { db.Entry(existing).CurrentValues[nameof(SystemSettingOverride.Value)] = value; db.Entry(existing).CurrentValues[nameof(SystemSettingOverride.IsSecret)] = isSecret; db.Entry(existing).CurrentValues[nameof(SystemSettingOverride.Category)] = category; db.Entry(existing).CurrentValues[nameof(SystemSettingOverride.UpdatedBy)] = updatedBy; db.Entry(existing).CurrentValues[nameof(SystemSettingOverride.UpdatedAt)] = DateTimeOffset.UtcNow; } await db.SaveChangesAsync(ct); return new(key, isSecret ? "********" : value, isSecret, category, true); }
    public async Task DeleteSettingOverrideAsync(string key, CancellationToken ct = default) { var s = await db.SystemSettingOverrides.SingleOrDefaultAsync(x => x.Key == key, ct); if (s is not null) { db.SystemSettingOverrides.Remove(s); await db.SaveChangesAsync(ct); } }
    async Task<string?> Value(string key, CancellationToken ct) => (await db.SystemSettingOverrides.AsNoTracking().SingleOrDefaultAsync(x => x.Key == key, ct))?.Value ?? configuration[key];
    public async Task<SmtpSettings> GetSmtpSettingsAsync(CancellationToken ct = default) => new(await Value("SmtpSettings:Host", ct), int.TryParse(await Value("SmtpSettings:Port", ct), out var p) ? p : 25, await Value("SmtpSettings:UserName", ct), await Value("SmtpSettings:Password", ct), await Value("SmtpSettings:From", ct), bool.TryParse(await Value("SmtpSettings:EnableSsl", ct), out var ssl) && ssl, bool.TryParse(await Value("SmtpSettings:IsEnabled", ct), out var en) && en);
    public async Task<SmsSettings> GetSmsSettingsAsync(CancellationToken ct = default)
    {
        var apiKey = await Value("SmsSettings:ApiKey", ct);
        var apiKeyHeaderName = await Value("SmsSettings:ApiKeyHeaderName", ct);
        var clientReferencePrefix = await Value("SmsSettings:ClientReferencePrefix", ct);
        return new(
            await Value("SmsSettings:BaseUrl", ct),
            string.IsNullOrWhiteSpace(apiKey) || apiKey == "********" ? "bee-dev-secret-key" : apiKey,
            string.IsNullOrWhiteSpace(apiKeyHeaderName) ? "x-api-key" : apiKeyHeaderName,
            await Value("SmsSettings:AccountId", ct),
            string.IsNullOrWhiteSpace(clientReferencePrefix) ? "LCA" : clientReferencePrefix,
            bool.TryParse(await Value("SmsSettings:IsEnabled", ct), out var en) && en);
    }
    public async Task<SendResult> TestEmailSettingsAsync(string to, CancellationToken ct = default) { var s = await GetSmtpSettingsAsync(ct); return new(s.IsEnabled && !string.IsNullOrWhiteSpace(s.Host), $"SMTP host {(string.IsNullOrWhiteSpace(s.Host) ? "not configured" : "configured")}; secret values are masked in settings responses."); }
    public async Task<SendResult> TestSmsSettingsAsync(string phoneNumber, CancellationToken ct = default) { var s = await GetSmsSettingsAsync(ct); var missing = new List<string>(); if (string.IsNullOrWhiteSpace(s.BaseUrl)) missing.Add("BaseUrl"); if (string.IsNullOrWhiteSpace(s.AccountId)) missing.Add("AccountId"); if (string.IsNullOrWhiteSpace(phoneNumber)) missing.Add("destinationAddress"); return new(s.IsEnabled && missing.Count == 0, missing.Count == 0 ? $"SMS settings configured for destination {phoneNumber}; secret values are masked in settings responses." : $"SMS settings missing {string.Join(", ", missing)}; secret values are masked in settings responses."); }
}

public sealed class SmtpEmailSender(ISystemSettingsApplicationService settings, ILogger<SmtpEmailSender> logger) : IEmailSender
{ public async Task<SendResult> SendEmailAsync(string to, string subject, string body, CancellationToken ct = default) { var s = await settings.GetSmtpSettingsAsync(ct); if (!s.IsEnabled) return new(false, "disabled", "SMTP is disabled"); try { using var client = new SmtpClient(s.Host, s.Port) { EnableSsl = s.EnableSsl }; if (!string.IsNullOrWhiteSpace(s.UserName)) client.Credentials = new NetworkCredential(s.UserName, s.Password); using var msg = new MailMessage(s.From ?? s.UserName ?? "no-reply@localhost", to, subject, body) { IsBodyHtml = true }; await client.SendMailAsync(msg, ct); return new(true, "sent"); } catch (Exception ex) { logger.LogWarning(ex, "Email notification failed"); return new(false, "failed", ex.Message); } } }
public sealed class HttpSmsSender(ISystemSettingsApplicationService settings, IHttpClientFactory clients, ILogger<HttpSmsSender> logger) : ISmsSender
{ public async Task<SendResult> SendSmsAsync(string phoneNumber, string message, CancellationToken ct = default) { var s = await settings.GetSmsSettingsAsync(ct); if (!s.IsEnabled) return new(false, "disabled", "SMS is disabled"); if (string.IsNullOrWhiteSpace(s.BaseUrl)) return new(false, "missing BaseUrl", "SMS BaseUrl is not configured"); if (string.IsNullOrWhiteSpace(s.AccountId)) return new(false, "missing AccountId", "SMS AccountId is not configured"); if (string.IsNullOrWhiteSpace(phoneNumber)) return new(false, "missing destinationAddress", "SMS destinationAddress is not configured"); try { var client = clients.CreateClient("sms"); if (!string.IsNullOrWhiteSpace(s.ApiKey)) client.DefaultRequestHeaders.TryAddWithoutValidation(s.ApiKeyHeaderName ?? "x-api-key", s.ApiKey); var payload = new { accountId = s.AccountId, destinationAddress = phoneNumber, body = message, clientReference = $"{(string.IsNullOrWhiteSpace(s.ClientReferencePrefix) ? "LCA" : s.ClientReferencePrefix)}-{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}" }; var response = await client.PostAsJsonAsync(s.BaseUrl, payload, ct); var text = await response.Content.ReadAsStringAsync(ct); JsonElement? rawJson = null; try { rawJson = JsonDocument.Parse(text).RootElement.Clone(); } catch (JsonException) { } return new(response.IsSuccessStatusCode, text, response.IsSuccessStatusCode ? null : text, rawJson); } catch (Exception ex) { logger.LogWarning(ex, "SMS notification failed"); return new(false, "failed", ex.Message); } } }

public sealed class NotificationApplicationService(EProcurementDbContext db, IEmailSender email, ISmsSender sms) : INotificationApplicationService, INotificationSender, IInAppNotificationService
{
    public Task QueueAsync(string eventCode, string entityType, Guid? entityId, object model, List<NotificationRecipientDto> recipients, CancellationToken ct = default) => SendAsync(eventCode, entityType, entityId, model, recipients, ct);
    public async Task SendAsync(string eventCode, string entityType, Guid? entityId, object model, List<NotificationRecipientDto> recipients, CancellationToken ct = default)
    { var templates = await db.NotificationTemplates.Where(x => x.EventCode == eventCode && x.IsActive).ToListAsync(ct); if (!templates.Any()) templates = [new($"{eventCode}InApp", eventCode, "Generated in-app template", eventCode, NotificationChannel.InApp, eventCode, "{{EntityReference}} {{Status}}", true, DateTimeOffset.UtcNow)]; foreach (var r in recipients) foreach (var t in templates) { var pref = await db.NotificationPreferences.AsNoTracking().SingleOrDefaultAsync(x => x.UserId == r.UserId && x.EventCode == eventCode, ct); if (pref is not null && ((t.Channel == NotificationChannel.InApp && !pref.InAppEnabled) || (t.Channel == NotificationChannel.Email && !pref.EmailEnabled) || (t.Channel == NotificationChannel.Sms && !pref.SmsEnabled))) continue; var subject = Render(t.SubjectTemplate, model); var body = Render(t.BodyTemplate, model); if (t.Channel == NotificationChannel.InApp) await CreateInAppAsync(eventCode, entityType, entityId, subject, body, NotificationPriority.Normal, ModelValue(model, "RelatedUrl"), r, ct); if (t.Channel == NotificationChannel.Email && !string.IsNullOrWhiteSpace(r.Email)) await SendEmailAsync(subject, body, r, ct); if (t.Channel == NotificationChannel.Sms && !string.IsNullOrWhiteSpace(r.PhoneNumber)) await SendSmsAsync(body, r, ct); } }
    public async Task<NotificationMessage> SendEmailAsync(string subject, string body, NotificationRecipientDto recipient, CancellationToken ct = default) { var msg = await AddMessage("ManualEmail", "Notification", null, NotificationChannel.Email, subject, body, recipient, NotificationStatus.Pending, ct); var result = await email.SendEmailAsync(recipient.Email ?? "", subject, body, ct); await Log(msg, NotificationChannel.Email, JsonSerializer.Serialize(new { recipient.Email, subject }), result.Response, result.Success ? NotificationStatus.Sent : NotificationStatus.Failed, result.Error, ct); db.Entry(msg).CurrentValues[nameof(NotificationMessage.Status)] = result.Success ? NotificationStatus.Sent : NotificationStatus.Failed; db.Entry(msg).CurrentValues[nameof(NotificationMessage.SentAt)] = DateTimeOffset.UtcNow; db.Entry(msg).CurrentValues[nameof(NotificationMessage.FailureReason)] = result.Error; await db.SaveChangesAsync(ct); return msg; }
    public async Task<NotificationMessage> SendSmsAsync(string body, NotificationRecipientDto recipient, CancellationToken ct = default) { var msg = await AddMessage("ManualSms", "Notification", null, NotificationChannel.Sms, "SMS", body, recipient, NotificationStatus.Pending, ct); var result = await sms.SendSmsAsync(recipient.PhoneNumber ?? "", body, ct); await Log(msg, NotificationChannel.Sms, JsonSerializer.Serialize(new { destinationAddress = recipient.PhoneNumber, body }), result.Response, result.Success ? NotificationStatus.Sent : NotificationStatus.Failed, result.Error, ct); return msg; }
    public Task<NotificationMessage> CreateInAppAsync(string eventCode, string entityType, Guid? entityId, string subject, string body, NotificationPriority priority, string? relatedUrl, NotificationRecipientDto recipient, CancellationToken ct = default) => AddMessage(eventCode, entityType, entityId, NotificationChannel.InApp, subject, body, recipient, NotificationStatus.Unread, ct, priority, relatedUrl);
    async Task<NotificationMessage> AddMessage(string eventCode, string entityType, Guid? entityId, NotificationChannel channel, string subject, string body, NotificationRecipientDto r, NotificationStatus status, CancellationToken ct, NotificationPriority priority = NotificationPriority.Normal, string? relatedUrl = null) { var msg = new NotificationMessage(eventCode, entityType, entityId, channel, subject, body, priority, status, DateTimeOffset.UtcNow, channel == NotificationChannel.InApp ? null : DateTimeOffset.UtcNow, null, relatedUrl); msg.Recipients.Add(new(msg.Id, r.UserId, r.RecipientType, r.Name, r.Email, r.PhoneNumber, r.RoleCode, status)); db.NotificationMessages.Add(msg); await db.SaveChangesAsync(ct); return msg; }
    async Task Log(NotificationMessage m, NotificationChannel c, string req, string res, NotificationStatus st, string? err, CancellationToken ct) { db.NotificationDeliveryLogs.Add(new(m.Id, c, req, res, st, DateTimeOffset.UtcNow, err)); await db.SaveChangesAsync(ct); }
    public Task<List<NotificationMessage>> GetMyNotificationsAsync(string userId, CancellationToken ct = default) => db.NotificationMessages.AsNoTracking().Include(x => x.Recipients).Where(x => x.Channel == NotificationChannel.InApp && x.Recipients.Any(r => r.UserId == userId)).OrderByDescending(x => x.CreatedAt).Take(100).ToListAsync(ct);
    public Task<int> GetUnreadCountAsync(string userId, CancellationToken ct = default) => db.NotificationRecipients.CountAsync(x => x.UserId == userId && x.Status == NotificationStatus.Unread, ct);
    public async Task MarkAsReadAsync(Guid notificationId, string userId, CancellationToken ct = default) { var rs = await db.NotificationRecipients.Where(x => x.NotificationMessageId == notificationId && x.UserId == userId).ToListAsync(ct); foreach (var r in rs) { db.Entry(r).CurrentValues[nameof(NotificationRecipient.Status)] = NotificationStatus.Read; db.Entry(r).CurrentValues[nameof(NotificationRecipient.ReadAt)] = DateTimeOffset.UtcNow; } var m = await db.NotificationMessages.FindAsync([notificationId], ct); if (m is not null) db.Entry(m).CurrentValues[nameof(NotificationMessage.Status)] = NotificationStatus.Read; await db.SaveChangesAsync(ct); }
    public async Task MarkAllAsReadAsync(string userId, CancellationToken ct = default) { var rs = await db.NotificationRecipients.Where(x => x.UserId == userId && x.Status == NotificationStatus.Unread).ToListAsync(ct); foreach (var r in rs) { db.Entry(r).CurrentValues[nameof(NotificationRecipient.Status)] = NotificationStatus.Read; db.Entry(r).CurrentValues[nameof(NotificationRecipient.ReadAt)] = DateTimeOffset.UtcNow; } await db.SaveChangesAsync(ct); }
    public Task<List<NotificationTemplate>> GetTemplatesAsync(CancellationToken ct = default) => db.NotificationTemplates.AsNoTracking().OrderBy(x => x.EventCode).ThenBy(x => x.Channel).ToListAsync(ct);
    public async Task<NotificationTemplate> SaveTemplateAsync(SaveNotificationTemplateDto dto, CancellationToken ct = default) { var t = dto.Id is null ? null : await db.NotificationTemplates.FindAsync([dto.Id.Value], ct); if (t is null) { t = new(dto.Code, dto.Name, dto.Description, dto.EventCode, dto.Channel, dto.SubjectTemplate, dto.BodyTemplate, dto.IsActive, DateTimeOffset.UtcNow); db.NotificationTemplates.Add(t); } else { db.Entry(t).CurrentValues[nameof(NotificationTemplate.SubjectTemplate)] = dto.SubjectTemplate; db.Entry(t).CurrentValues[nameof(NotificationTemplate.BodyTemplate)] = dto.BodyTemplate; db.Entry(t).CurrentValues[nameof(NotificationTemplate.IsActive)] = dto.IsActive; db.Entry(t).CurrentValues[nameof(NotificationTemplate.UpdatedAt)] = DateTimeOffset.UtcNow; } await db.SaveChangesAsync(ct); return t; }
    public async Task<string> TestTemplateAsync(Guid id, Dictionary<string,string?> model, CancellationToken ct = default) { var t = await db.NotificationTemplates.FindAsync([id], ct) ?? throw new InvalidOperationException("Template not found"); return Render(t.BodyTemplate, model); }
    public Task<List<NotificationDeliveryLog>> GetDeliveryLogsAsync(CancellationToken ct = default) => db.NotificationDeliveryLogs.AsNoTracking().OrderByDescending(x => x.SentAt).Take(200).ToListAsync(ct);
    public async Task<NotificationDeliveryLog> RetryDeliveryAsync(Guid id, CancellationToken ct = default) { var log = await db.NotificationDeliveryLogs.FindAsync([id], ct) ?? throw new InvalidOperationException("Delivery log not found"); var msg = await db.NotificationMessages.Include(x=>x.Recipients).SingleAsync(x=>x.Id==log.NotificationMessageId, ct); var r = msg.Recipients.FirstOrDefault(); SendResult result = log.Channel switch { NotificationChannel.Email => await email.SendEmailAsync(r?.Email ?? "", msg.Subject, msg.Body, ct), NotificationChannel.Sms => await sms.SendSmsAsync(r?.PhoneNumber ?? "", msg.Body, ct), _ => new(true,"in-app retry not required") }; var status = result.Success ? NotificationStatus.Sent : NotificationStatus.Failed; var retried = new NotificationDeliveryLog(msg.Id, log.Channel, log.RequestPayload, result.Response, status, DateTimeOffset.UtcNow, result.Error); db.NotificationDeliveryLogs.Add(retried); db.AuditEvents.Add(new AuditEvent("Notification retried", nameof(NotificationDeliveryLog), id, log.Channel.ToString(), "system", result.Success ? "Retry succeeded" : result.Error ?? "Retry failed", DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return retried; }
    public Task<List<NotificationEventMapping>> GetEventMappingsAsync(CancellationToken ct = default) => db.NotificationEventMappings.AsNoTracking().OrderBy(x=>x.EventCode).ToListAsync(ct);
    public async Task<NotificationEventMapping> SaveEventMappingAsync(NotificationEventMapping dto, CancellationToken ct = default) { var e = await db.NotificationEventMappings.FindAsync([dto.Id], ct); if (e is null) { db.NotificationEventMappings.Add(dto); db.AuditEvents.Add(new AuditEvent("Notification event mapping created", nameof(NotificationEventMapping), dto.Id, dto.EventCode, "system", dto.TemplateCode, DateTimeOffset.UtcNow)); } else db.Entry(e).CurrentValues.SetValues(dto); await db.SaveChangesAsync(ct); return dto; }
    public async Task<List<NotificationPreferenceDto>> GetPreferencesAsync(string userId, CancellationToken ct = default) => await db.NotificationPreferences.AsNoTracking().Where(x => x.UserId == userId).Select(x => new NotificationPreferenceDto(x.EventCode, x.InAppEnabled, x.EmailEnabled, x.SmsEnabled)).ToListAsync(ct);
    public async Task<List<NotificationPreferenceDto>> SavePreferencesAsync(string userId, List<NotificationPreferenceDto> prefs, CancellationToken ct = default) { foreach (var p in prefs) { var e = await db.NotificationPreferences.SingleOrDefaultAsync(x => x.UserId == userId && x.EventCode == p.EventCode, ct); if (e is null) db.NotificationPreferences.Add(new(userId, p.EventCode, p.InAppEnabled, p.EmailEnabled, p.SmsEnabled)); else { db.Entry(e).CurrentValues[nameof(NotificationPreference.InAppEnabled)] = p.InAppEnabled; db.Entry(e).CurrentValues[nameof(NotificationPreference.EmailEnabled)] = p.EmailEnabled; db.Entry(e).CurrentValues[nameof(NotificationPreference.SmsEnabled)] = p.SmsEnabled; } } await db.SaveChangesAsync(ct); return prefs; }
    static string Render(string template, object model) { var json = JsonSerializer.Serialize(model); var values = JsonSerializer.Deserialize<Dictionary<string,object?>>(json) ?? []; foreach (var k in new[] {"UserName","SupplierName","TenderNumber","TenderTitle","ClosingDate","WorkflowTask","ActionRequired","PortalLink","EntityReference","Status"}) template = template.Replace("{{" + k + "}}", values.GetValueOrDefault(k)?.ToString() ?? ""); return template; }
    static string? ModelValue(object model, string key) { var values = JsonSerializer.Deserialize<Dictionary<string,object?>>(JsonSerializer.Serialize(model)) ?? []; return values.GetValueOrDefault(key)?.ToString(); }
}


public sealed record RenderedNotificationTemplate(string Subject, string Body);
public interface INotificationTemplateRenderer { Task<RenderedNotificationTemplate> RenderAsync(string code, NotificationChannel channel, Dictionary<string,string?> model, CancellationToken ct = default); }
public sealed class NotificationTemplateRenderer(EProcurementDbContext db) : INotificationTemplateRenderer
{
    public async Task<RenderedNotificationTemplate> RenderAsync(string code, NotificationChannel channel, Dictionary<string,string?> model, CancellationToken ct = default)
    {
        var t = await db.NotificationTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Code == code && x.Channel == channel && x.IsActive, ct) ?? throw new InvalidOperationException($"Notification template '{code}' for {channel} was not found.");
        return new(RenderStrict(t.SubjectTemplate, model), RenderStrict(t.BodyTemplate, model));
    }
    static string RenderStrict(string template, Dictionary<string,string?> model) => System.Text.RegularExpressions.Regex.Replace(template, "{{\s*(?<key>[A-Za-z0-9_]+)\s*}}", m => model.TryGetValue(m.Groups["key"].Value, out var v) ? v ?? "" : throw new InvalidOperationException($"Missing notification template placeholder '{m.Groups["key"].Value}'."));
}

public interface INotificationRecipientResolver { Task<List<NotificationRecipientDto>> ResolveAsync(string recipientRule, object context, CancellationToken ct = default); }
public sealed class NotificationRecipientResolver(EProcurementDbContext db) : INotificationRecipientResolver
{
    public async Task<List<NotificationRecipientDto>> ResolveAsync(string recipientRule, object context, CancellationToken ct = default)
    {
        IQueryable<ApplicationUser> users = db.ApplicationUsers.AsNoTracking().Where(x => x.IsActive);
        users = recipientRule switch
        {
            "ProcurementOfficers" => users.Where(u => u.UserRoles.Any(ur => db.Roles.Any(r => r.Id == ur.RoleId && r.Name == "ProcurementOfficer"))),
            "FinanceUsers" => users.Where(u => u.UserRoles.Any(ur => db.Roles.Any(r => r.Id == ur.RoleId && r.Name == "FinanceOfficer"))),
            "SystemAdministrators" => users.Where(u => u.UserRoles.Any(ur => db.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Administrator"))),
            "SupplierUsers" or "SupplierOwner" => users.Where(u => u.UserType == UserType.Supplier),
            _ => users.Take(20)
        };
        return await users.Select(u => new NotificationRecipientDto(u.Id.ToString(), u.FullName, u.Email, u.PhoneNumber, u.UserType.ToString())).ToListAsync(ct);
    }
}

public sealed record CreateCommunicationThreadDto(string EntityType, Guid EntityId, string EntityReference, string Subject, string Body, string Visibility = "Internal", Guid? SupplierId = null, bool IsInternal = true, bool IsPublic = false);
public sealed record CreateCommunicationMessageDto(string Body, bool IsInternal = false, bool IsPublic = false);
public interface ICommunicationApplicationService { Task<List<CommunicationThread>> GetThreadsAsync(string userId, bool supplier, Guid? supplierId, CancellationToken ct=default); Task<CommunicationThread?> GetThreadAsync(Guid id, string userId, bool supplier, Guid? supplierId, CancellationToken ct=default); Task<CommunicationThread> CreateThreadAsync(CreateCommunicationThreadDto dto, string userId, string name, string senderType, CancellationToken ct=default); Task<CommunicationMessage> AddMessageAsync(Guid threadId, CreateCommunicationMessageDto dto, string userId, string name, string senderType, bool supplier, Guid? supplierId, CancellationToken ct=default); Task CloseAsync(Guid threadId, string userId, CancellationToken ct=default); }
public sealed class CommunicationApplicationService(EProcurementDbContext db) : ICommunicationApplicationService
{
    public Task<List<CommunicationThread>> GetThreadsAsync(string userId, bool supplier, Guid? supplierId, CancellationToken ct=default) => db.CommunicationThreads.AsNoTracking().Include(x=>x.Messages).Where(x => !supplier || x.SupplierId == supplierId).OrderByDescending(x=>x.CreatedAt).Take(100).ToListAsync(ct);
    public async Task<CommunicationThread?> GetThreadAsync(Guid id, string userId, bool supplier, Guid? supplierId, CancellationToken ct=default) { var t = await db.CommunicationThreads.AsNoTracking().Include(x=>x.Messages).SingleOrDefaultAsync(x=>x.Id==id && (!supplier || x.SupplierId==supplierId), ct); if (t is not null && supplier) t.Messages.RemoveAll(m => m.IsInternal); return t; }
    public async Task<CommunicationThread> CreateThreadAsync(CreateCommunicationThreadDto dto, string userId, string name, string senderType, CancellationToken ct=default) { var t = new CommunicationThread($"COM-{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}", dto.EntityType, dto.EntityId, dto.EntityReference, dto.Subject, dto.Visibility, userId, DateTimeOffset.UtcNow, SupplierId:dto.SupplierId); t.Messages.Add(new CommunicationMessage(t.Id, userId, name, senderType, dto.Body, dto.IsInternal, dto.IsPublic, DateTimeOffset.UtcNow)); db.CommunicationThreads.Add(t); db.AuditEvents.Add(new AuditEvent("Communication thread created", dto.EntityType, dto.EntityId, dto.EntityReference, userId, dto.Subject, DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return t; }
    public async Task<CommunicationMessage> AddMessageAsync(Guid threadId, CreateCommunicationMessageDto dto, string userId, string name, string senderType, bool supplier, Guid? supplierId, CancellationToken ct=default) { var t = await db.CommunicationThreads.SingleOrDefaultAsync(x=>x.Id==threadId && (!supplier || x.SupplierId==supplierId), ct) ?? throw new UnauthorizedAccessException("Communication thread not available."); if (supplier && dto.IsInternal) throw new UnauthorizedAccessException("Supplier users cannot create internal messages."); var m = new CommunicationMessage(threadId, userId, name, senderType, dto.Body, dto.IsInternal, dto.IsPublic, DateTimeOffset.UtcNow); db.CommunicationMessages.Add(m); db.AuditEvents.Add(new AuditEvent(dto.IsInternal ? "Internal communication sent" : supplier ? "Supplier communication sent" : "Communication message sent", t.EntityType, t.EntityId, t.EntityReference, userId, t.Subject, DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return m; }
    public async Task CloseAsync(Guid threadId, string userId, CancellationToken ct=default) { var t=await db.CommunicationThreads.FindAsync([threadId], ct) ?? throw new InvalidOperationException("Thread not found"); db.Entry(t).CurrentValues[nameof(CommunicationThread.Status)] = "Closed"; db.Entry(t).CurrentValues[nameof(CommunicationThread.ClosedAt)] = DateTimeOffset.UtcNow; await db.SaveChangesAsync(ct); }
}

public interface IDeadlineReminderService { Task<List<DeadlineReminderRun>> GetDueRemindersAsync(CancellationToken ct=default); Task<List<DeadlineReminderRun>> RunDueRemindersAsync(CancellationToken ct=default); Task<List<DeadlineReminderRun>> RunRuleAsync(Guid ruleId, CancellationToken ct=default); }
public sealed class DeadlineReminderService(EProcurementDbContext db, INotificationRecipientResolver resolver, INotificationSender sender) : IDeadlineReminderService
{
    public Task<List<DeadlineReminderRun>> GetDueRemindersAsync(CancellationToken ct=default) => db.DeadlineReminderRuns.AsNoTracking().Where(x=>x.Status==NotificationStatus.Pending && x.ScheduledFor<=DateTimeOffset.UtcNow).ToListAsync(ct);
    public async Task<List<DeadlineReminderRun>> RunDueRemindersAsync(CancellationToken ct=default) { var outp = new List<DeadlineReminderRun>(); foreach (var rule in await db.DeadlineReminderRules.Where(x=>x.IsEnabled).ToListAsync(ct)) outp.AddRange(await RunRuleAsync(rule.Id, ct)); return outp; }
    public async Task<List<DeadlineReminderRun>> RunRuleAsync(Guid ruleId, CancellationToken ct=default) { var rule = await db.DeadlineReminderRules.FindAsync([ruleId], ct) ?? throw new InvalidOperationException("Reminder rule not found"); var due = DateTimeOffset.UtcNow.AddHours(rule.ReminderOffsetHours); var runs = new List<DeadlineReminderRun>(); if (rule.EntityType == nameof(Tender)) { var tenders = await db.Tenders.Where(t=>t.Status==TenderStatus.Published && t.ClosingDate<=due).ToListAsync(ct); foreach (var t in tenders) if (!await db.DeadlineReminderRuns.AnyAsync(r=>r.RuleId==rule.Id && r.EntityId==t.Id, ct)) { var run = new DeadlineReminderRun(rule.Id, nameof(Tender), t.Id, t.TenderNumber, t.ClosingDate, DateTimeOffset.UtcNow, NotificationStatus.Completed); db.DeadlineReminderRuns.Add(run); runs.Add(run); await sender.SendAsync("TenderClosingSoon", nameof(Tender), t.Id, new { TenderNumber=t.TenderNumber, TenderTitle=t.Title, ClosingDate=t.ClosingDate.ToString("u"), EntityReference=t.TenderNumber, Status="Closing soon", RelatedUrl=$"/public/opportunities/{t.TenderNumber}" }, await resolver.ResolveAsync(rule.RecipientRule, t, ct), ct); } } await db.SaveChangesAsync(ct); return runs; }
}
