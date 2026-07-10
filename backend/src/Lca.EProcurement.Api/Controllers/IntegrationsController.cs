using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController, Route("api/integrations")]
[RequirePermission("Integrations.View")]
public sealed class IntegrationsController(IIntegrationApplicationService integrations) : ControllerBase
{
    [HttpGet("dashboard")] public Task<IntegrationDashboardDto> Dashboard(CancellationToken ct) => integrations.GetDashboardAsync(ct);
    [HttpGet("endpoints")] public Task<List<Lca.EProcurement.Domain.IntegrationEndpoint>> Endpoints(CancellationToken ct) => integrations.GetEndpointsAsync(ct);
    [HttpPost("endpoints"), RequirePermission("Integrations.Manage")] public Task<Lca.EProcurement.Domain.IntegrationEndpoint> SaveEndpoint(IntegrationEndpointDto dto, CancellationToken ct) => integrations.SaveEndpointAsync(dto, ct);
    [HttpPost("endpoints/{id:guid}/test"), RequirePermission("Integrations.Manage")] public Task<IntegrationTestResultDto> Test(Guid id, CancellationToken ct) => integrations.TestEndpointAsync(id, ct);
    [HttpGet("messages")] public Task<List<Lca.EProcurement.Domain.IntegrationMessage>> Messages(CancellationToken ct) => integrations.GetMessagesAsync(null, ct);
    [HttpGet("logs")] public Task<List<Lca.EProcurement.Domain.IntegrationLog>> Logs(CancellationToken ct) => integrations.GetLogsAsync(ct);
    [HttpGet("references")] public Task<List<Lca.EProcurement.Domain.ExternalSystemReference>> References(string? entityType, Guid? entityId, CancellationToken ct) => integrations.GetReferencesAsync(entityType, entityId, ct);
}
