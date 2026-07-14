using Lca.EProcurement.Application;
using Lca.EProcurement.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[Route("api/documents")]
public sealed class DocumentsController(IDocumentApplicationService documents, IDocumentIntegrationService integration) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<ActionResult<DocumentUploadResponse>> Upload([FromForm] DocumentUploadForm form, CancellationToken ct)
    { await using var s=form.File.OpenReadStream(); return Ok(await documents.UploadAsync(form.ToRequest(s, User.Identity?.Name ?? "system"), ct)); }
    [HttpGet]
    public Task<List<DocumentSummaryDto>> Get([FromQuery]string? entityType,[FromQuery]Guid? entityId,[FromQuery]string? documentType,[FromQuery]DocumentClassification? classification,[FromQuery]DocumentStatus? status,[FromQuery]bool? isPublic,CancellationToken ct) => documents.GetAsync(entityType,entityId,documentType,classification,status,isPublic,ct);
    [HttpGet("{id:guid}")] public async Task<ActionResult<DocumentDetailDto>> Get(Guid id,CancellationToken ct)=> await documents.GetAsync(id,ct) is { } d ? Ok(d) : NotFound();
    [HttpGet("{id:guid}/download")] public async Task<IActionResult> Download(Guid id,CancellationToken ct){ var d=await documents.DownloadAsync(id,null,User.Identity?.Name??"anonymous",User.Identity?.Name??"anonymous",User.Claims.Where(c=>c.Type.Contains("role")).Select(c=>c.Value),User.Claims.Where(c=>c.Type=="permission").Select(c=>c.Value),ct); return File(d.Content,d.Version.ContentType,d.Version.OriginalFileName); }
    [HttpGet("{id:guid}/versions")] public Task<List<DocumentVersionDto>> Versions(Guid id,CancellationToken ct)=>documents.GetVersionsAsync(id,ct);
    [HttpGet("{id:guid}/versions/{versionId:guid}/download")] public async Task<IActionResult> DownloadVersion(Guid id,Guid versionId,CancellationToken ct){ var d=await documents.DownloadAsync(id,versionId,User.Identity?.Name??"anonymous",User.Identity?.Name??"anonymous",User.Claims.Where(c=>c.Type.Contains("role")).Select(c=>c.Value),User.Claims.Where(c=>c.Type=="permission").Select(c=>c.Value),ct); return File(d.Content,d.Version.ContentType,d.Version.OriginalFileName); }
    [HttpPost("{id:guid}/versions")] public async Task<ActionResult<DocumentUploadResponse>> Version(Guid id,[FromForm] DocumentUploadForm form,[FromQuery]string? reason,CancellationToken ct){ await using var s=form.File.OpenReadStream(); return Ok(await documents.UploadNewVersionAsync(id,form.ToRequest(s,User.Identity?.Name??"system"),reason,ct)); }
    [HttpPut("{id:guid}/metadata")] public IActionResult Metadata(Guid id) => Accepted(new { id, status="Metadata update endpoint ready for Phase 1 extension" });
    [HttpPost("{id:guid}/publish")] public async Task<IActionResult> Publish(Guid id,CancellationToken ct)=>Ok(await documents.PublishAsync(id,User.Identity?.Name??"system",true,ct));
    [HttpPost("{id:guid}/unpublish")] public async Task<IActionResult> Unpublish(Guid id,CancellationToken ct)=>Ok(await documents.PublishAsync(id,User.Identity?.Name??"system",false,ct));
    [HttpPost("{id:guid}/archive")] public async Task<IActionResult> Archive(Guid id,[FromBody] ArchiveRequest? r,CancellationToken ct)=>Ok(await documents.ArchiveAsync(id,User.Identity?.Name??"system",r?.Reason??"Manual archive",ct));
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id,CancellationToken ct)=>Ok(await documents.DeleteAsync(id,User.Identity?.Name??"system",ct));
    [HttpGet("{id:guid}/access-log")] public Task<List<DocumentAccessLogDto>> Log(Guid id,CancellationToken ct)=>documents.GetAccessLogAsync(id,ct);
    [HttpPost("{id:guid}/sync-to-external-dms")] public Task<Lca.EProcurement.Domain.IntegrationMessage> Sync(Guid id,CancellationToken ct)=>integration.SyncToExternalDmsAsync(id,User.Identity?.Name??"system",ct);
}
public sealed record ArchiveRequest(string Reason);
public sealed class DocumentUploadForm { public string EntityType { get; set; }=""; public Guid EntityId { get; set; } public string DocumentType { get; set; }=""; public string Title { get; set; }=""; public string? Description { get; set; } public DocumentClassification Classification { get; set; }=DocumentClassification.Internal; public bool IsPublic { get; set; } public IFormFile File { get; set; }=default!; public DocumentUploadRequest ToRequest(Stream stream,string actor)=>new(EntityType,EntityId,DocumentType,Title,Description,Classification,IsPublic,File.FileName,File.ContentType,stream,actor); }

[ApiController]
[AllowAnonymous]
[Route("api/public/documents")]
public sealed class PublicDocumentsController(IDocumentApplicationService documents) : ControllerBase
{ [HttpGet("{id:guid}/download")] public async Task<IActionResult> Download(Guid id,CancellationToken ct){ var detail=await documents.GetAsync(id,ct); if(detail?.Record is not { IsPublic:true, Classification:DocumentClassification.Public, Status:DocumentStatus.Active }) return NotFound(); var d=await documents.DownloadAsync(id,null,"public","public",[],[],ct); return File(d.Content,d.Version.ContentType,d.Version.OriginalFileName); } }

[ApiController]
[Route("api/document-requirements")]
public sealed class DocumentRequirementsController(IDocumentRequirementValidationService validation) : ControllerBase
{ [HttpGet("{entityType}/{entityId:guid}/validate")] [HttpPost("{entityType}/{entityId:guid}/validate")] public Task<DocumentRequirementValidationResultDto> Validate(string entityType,Guid entityId,[FromQuery]Guid documentRequirementSetId,CancellationToken ct)=>validation.ValidateRequiredDocumentsAsync(entityType,entityId,documentRequirementSetId,ct); }

[ApiController]
[Route("api/document-retention")]
public sealed class DocumentRetentionController(IDocumentRetentionService retention) : ControllerBase
{ [HttpGet("/api/document-retention-policies")] public Task<List<DocumentRetentionPolicy>> Get(CancellationToken ct)=>retention.GetPoliciesAsync(ct); [HttpPost("/api/document-retention-policies")] public Task<DocumentRetentionPolicy> Post(DocumentRetentionPolicyDto dto,CancellationToken ct)=>retention.SavePolicyAsync(dto,ct); [HttpPut("/api/document-retention-policies/{id:guid}")] public Task<DocumentRetentionPolicy> Put(Guid id,DocumentRetentionPolicyDto dto,CancellationToken ct)=>retention.SavePolicyAsync(dto with { Id=id },ct); [HttpPost("run-archive-check")] public Task<int> Run(CancellationToken ct)=>retention.RunArchiveCheckAsync(User.Identity?.Name??"system",ct); }

[ApiController]
[Route("api/document-imports")]
public sealed class DocumentImportsController : ControllerBase
{ [HttpPost] public IActionResult Post() => StatusCode(StatusCodes.Status501NotImplemented, new { message="Document import batch metadata structures are available; CSV/file migration importer is scheduled for a later phase." }); }
