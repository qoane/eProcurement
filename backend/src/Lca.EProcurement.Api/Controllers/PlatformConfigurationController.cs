using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/platform-configuration")]
[RequirePermission("Configuration.View")]
public sealed class PlatformConfigurationController(IPlatformConfigurationApplicationService configuration) : ControllerBase
{
    [HttpGet("workflow-mappings")] public async Task<IActionResult> WorkflowMappings(CancellationToken ct) => Ok(await configuration.GetWorkflowMappingsAsync(ct));
    [HttpPost("workflow-mappings")] public async Task<IActionResult> CreateWorkflowMapping(WorkflowMappingDto dto, CancellationToken ct) => Created("/api/platform-configuration/workflow-mappings", await configuration.CreateWorkflowMappingAsync(dto, ct));
    [HttpGet("transition-effects")] public async Task<IActionResult> TransitionEffects(CancellationToken ct) => Ok(await configuration.GetTransitionEffectsAsync(ct));
    [HttpPost("transition-effects")] public async Task<IActionResult> CreateTransitionEffect(CreateTransitionEffectDto dto, CancellationToken ct) => Created("/api/platform-configuration/transition-effects", await configuration.CreateTransitionEffectAsync(dto, ct));
    [HttpGet("document-type-requirements")] public async Task<IActionResult> DocumentTypeRequirements(CancellationToken ct) => Ok(await configuration.GetDocumentTypeRequirementsAsync(ct));
    [HttpPost("document-type-requirements")] public async Task<IActionResult> CreateDocumentTypeRequirement(DocumentTypeRequirementDto dto, CancellationToken ct) => Created("/api/platform-configuration/document-type-requirements", await configuration.CreateDocumentTypeRequirementAsync(dto, ct));
    [HttpGet("lookup-values")] public async Task<IActionResult> LookupValues([FromQuery] string? lookupType, CancellationToken ct) => Ok(await configuration.GetLookupValuesAsync(lookupType, ct));
    [HttpPost("lookup-values")] public async Task<IActionResult> CreateLookupValue(LookupValueDto dto, CancellationToken ct) => Created("/api/platform-configuration/lookup-values", await configuration.CreateLookupValueAsync(dto, ct));
    [HttpGet("supplier-categories")] public async Task<IActionResult> SupplierCategories(CancellationToken ct) => Ok(await configuration.GetSupplierCategoriesAsync(ct));
    [HttpPost("supplier-categories")] public async Task<IActionResult> CreateSupplierCategory(SupplierCategoryDto dto, CancellationToken ct) => Created("/api/platform-configuration/supplier-categories", await configuration.CreateSupplierCategoryAsync(dto, ct));
}
