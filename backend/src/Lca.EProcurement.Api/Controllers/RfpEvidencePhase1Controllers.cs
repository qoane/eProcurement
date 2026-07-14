using Lca.EProcurement.Application;
using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lca.EProcurement.Api.Controllers;

[ApiController, Route("api/rfp-evidence"), Authorize(Policy="RfpEvidence.View")]
public sealed class RfpEvidenceController(IRfpEvidenceApplicationService svc) : ControllerBase
{
    [HttpGet("requirements")] public Task<List<ComplianceRequirement>> Requirements(CancellationToken ct) => svc.GetRequirementsAsync(ct);
    [HttpGet("requirements/{id:guid}")] public async Task<ActionResult<ComplianceRequirement>> Requirement(Guid id, CancellationToken ct) => await svc.GetRequirementAsync(id, ct) is { } r ? r : NotFound();
    [HttpPost("requirements"), Authorize(Policy="RfpEvidence.Manage")] public Task<ComplianceRequirement> Create(ComplianceRequirement r, CancellationToken ct) => svc.CreateRequirementAsync(r, ct);
    [HttpPut("requirements/{id:guid}"), Authorize(Policy="RfpEvidence.Manage")] public async Task<ActionResult<ComplianceRequirement>> Update(Guid id, ComplianceRequirement r, CancellationToken ct) => await svc.UpdateRequirementAsync(id, r, ct) is { } x ? x : NotFound();
    [HttpPost("requirements/{id:guid}/verify"), Authorize(Policy="RfpEvidence.Manage")] public async Task<ActionResult<ComplianceRequirement>> Verify(Guid id, [FromQuery] string verifiedBy="system", CancellationToken ct=default) => await svc.VerifyRequirementAsync(id, verifiedBy, ct) is { } x ? x : NotFound();
    [HttpGet("coverage")] public Task<CoverageDto> Coverage(CancellationToken ct) => svc.GetCoverageAsync(ct);
    [HttpGet("proposal-commitments")] public Task<List<ProposalCommitment>> Commitments(CancellationToken ct) => svc.GetCommitmentsAsync(ct);
    [HttpPost("export-pack"), Authorize(Policy="RfpEvidence.Export")] public Task<EvidencePackDto> Export(CancellationToken ct) => svc.ExportPackAsync(ct);
}

[ApiController, Route("api/demo"), Authorize(Policy="Demo.View")]
public sealed class DemoController(IDemoDataVerificationService svc, EProcurementDbContext db, IConfiguration config, IWebHostEnvironment env) : ControllerBase
{
    [HttpGet("status")] public Task<DemoStatusDto> Status(CancellationToken ct) => svc.GetStatusAsync(ct);
    [HttpPost("verify"), Authorize(Policy="Demo.Manage")] public Task<DemoStatusDto> Verify(CancellationToken ct) => svc.VerifyAsync(ct);
    [HttpPost("reset"), Authorize(Policy="Demo.Manage")] public async Task<ActionResult> Reset(CancellationToken ct) { var allowed = env.IsDevelopment() || config.GetValue<bool>("Operations:AllowDemoReset"); return await svc.ResetAsync(allowed, ct) ? Ok(new{reset="allowed"}) : StatusCode(403, new{reset="blocked"}); }
    [HttpPost("reseed"), Authorize(Policy="Demo.Manage")] public async Task<ActionResult> Reseed(CancellationToken ct) { await svc.ReseedAsync(ct); return Ok(); }
    [HttpGet("script")] public Task<List<DemoStep>> Script(CancellationToken ct) => db.DemoSteps.AsNoTracking().OrderBy(x=>x.StepNumber).ToListAsync(ct);
    [HttpPut("script/{id:guid}"), Authorize(Policy="Demo.Manage")] public async Task<ActionResult> Update(Guid id, DemoStep step, CancellationToken ct) { var e=await db.DemoSteps.FindAsync([id],ct); if(e is null) return NotFound(); db.Entry(e).CurrentValues.SetValues(step with { UpdatedAt=DateTimeOffset.UtcNow }); await db.SaveChangesAsync(ct); return Ok(e); }
}

[ApiController, Route("api/uat"), Authorize(Policy="Uat.View")]
public sealed class UatController(EProcurementDbContext db) : ControllerBase
{
    [HttpGet("suites")] public Task<List<UatTestSuite>> Suites(CancellationToken ct)=>db.UatTestSuites.AsNoTracking().Include(x=>x.TestCases).OrderBy(x=>x.Name).ToListAsync(ct);
    [HttpGet("suites/{id:guid}")] public async Task<ActionResult<UatTestSuite>> Suite(Guid id,CancellationToken ct)=>await db.UatTestSuites.AsNoTracking().Include(x=>x.TestCases).SingleOrDefaultAsync(x=>x.Id==id,ct) is{} s?s:NotFound();
    [HttpPost("suites"), Authorize(Policy="Uat.Manage")] public async Task<UatTestSuite> Create(CreateUatSuiteDto d,CancellationToken ct){var s=new UatTestSuite(d.Code,d.Name,d.Description,d.Module,DateTimeOffset.UtcNow,d.CreatedBy); db.UatTestSuites.Add(s); db.AuditEvents.Add(new AuditEvent("UAT test case created","UAT",s.Id,"Create","system",s.Name,DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return s;}
    [HttpGet("runs")] public Task<List<UatTestRun>> Runs(CancellationToken ct)=>db.UatTestRuns.AsNoTracking().Include(x=>x.Results).ToListAsync(ct);
    [HttpPost("runs"), Authorize(Policy="Uat.Execute")] public async Task<UatTestRun> Run(CreateUatRunDto d,CancellationToken ct){var count=await db.UatTestRuns.CountAsync(ct)+1; var r=new UatTestRun($"UAT-RUN-{count:000}",d.SuiteId,DateTimeOffset.UtcNow,null,"InProgress",d.ExecutedBy,d.Notes); db.UatTestRuns.Add(r); db.AuditEvents.Add(new AuditEvent("UAT test run started","UAT",r.Id,"Start",d.ExecutedBy,r.RunNumber,DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return r;}
    [HttpPost("runs/{id:guid}/results"), Authorize(Policy="Uat.Execute")] public async Task<ActionResult<UatTestResult>> Result(Guid id,RecordUatResultDto d,CancellationToken ct){if(!await db.UatTestRuns.AnyAsync(x=>x.Id==id,ct)) return NotFound(); var r=new UatTestResult(id,d.TestCaseId,d.Result,d.ActualResult,d.EvidenceNotes,d.DefectReference,DateTimeOffset.UtcNow,d.ExecutedBy); db.UatTestResults.Add(r); db.AuditEvents.Add(new AuditEvent("UAT test result recorded","UAT",r.Id,"Result",d.ExecutedBy,d.Result.ToString(),DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return r;}
    [HttpGet("runs/{id:guid}/export.csv")] public async Task<FileContentResult> Export(Guid id,CancellationToken ct){var rows=await db.UatTestResults.Where(x=>x.RunId==id).ToListAsync(ct); var csv="TestCaseId,Result,ActualResult,EvidenceNotes,ExecutedAt\n"+string.Join("\n",rows.Select(x=>$"{x.TestCaseId},{x.Result},\"{x.ActualResult.Replace("\"","\"\"")}\",\"{x.EvidenceNotes.Replace("\"","\"\"")}\",{x.ExecutedAt:O}")); return File(System.Text.Encoding.UTF8.GetBytes(csv),"text/csv","uat-run.csv");}
}

[ApiController, Route("api/training"), Authorize(Policy="Training.View")]
public sealed class TrainingController(EProcurementDbContext db) : ControllerBase
{
    [HttpGet("modules")] public Task<List<TrainingModule>> Modules(CancellationToken ct){ var supplier=User.IsInRole("Supplier"); var q=db.TrainingModules.AsNoTracking().Include(x=>x.Lessons).AsQueryable(); if(supplier) q=q.Where(x=>x.Audience==TrainingAudience.Supplier); return q.OrderBy(x=>x.Name).ToListAsync(ct); }
    [HttpGet("modules/{code}")] public async Task<ActionResult<TrainingModule>> Module(string code,CancellationToken ct){var m=await db.TrainingModules.AsNoTracking().Include(x=>x.Lessons).SingleOrDefaultAsync(x=>x.Code==code,ct); if(m is null) return NotFound(); db.AuditEvents.Add(new AuditEvent("Training module viewed","Training",m.Id,"View",User.Identity?.Name??"system",code,DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return m;}
    [HttpPost("modules/{code}/complete")] public async Task<ActionResult> Complete(string code,CancellationToken ct){var m=await db.TrainingModules.SingleOrDefaultAsync(x=>x.Code==code,ct); if(m is null) return NotFound(); db.TrainingCompletions.Add(new TrainingCompletion(m.Id,User.Identity?.Name??"system",DateTimeOffset.UtcNow)); db.AuditEvents.Add(new AuditEvent("Training completion recorded","Training",m.Id,"Complete",User.Identity?.Name??"system",code,DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return Ok();}
}

[ApiController, Route("api/implementation"), Authorize(Policy="Implementation.View")]
public sealed class ImplementationController(EProcurementDbContext db):ControllerBase{[HttpGet("phases")] public Task<List<ImplementationPhase>> Phases(CancellationToken ct)=>db.ImplementationPhases.AsNoTracking().Include(x=>x.Milestones).ThenInclude(x=>x.Tasks).OrderBy(x=>x.StartDate).ToListAsync(ct); [HttpPost("tasks/{id:guid}/complete"),Authorize(Policy="Implementation.Manage")] public async Task<ActionResult> Complete(Guid id,CancellationToken ct){var t=await db.ImplementationTasks.FindAsync([id],ct); if(t is null)return NotFound(); db.Entry(t).CurrentValues[nameof(ImplementationTask.Status)]="Completed"; db.Entry(t).CurrentValues[nameof(ImplementationTask.CompletedAt)]=DateTimeOffset.UtcNow; db.AuditEvents.Add(new AuditEvent("Implementation milestone completed","Implementation",id,"Complete","system",t.Title,DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return Ok();}}
[ApiController, Route("api/handover"), Authorize(Policy="Handover.View")]
public sealed class HandoverController(EProcurementDbContext db):ControllerBase{[HttpGet("checklists")] public Task<List<HandoverChecklist>> Lists(CancellationToken ct)=>db.HandoverChecklists.AsNoTracking().Include(x=>x.Items).ToListAsync(ct); [HttpPost("items/{id:guid}/complete"),Authorize(Policy="Handover.Manage")] public async Task<ActionResult> Complete(Guid id,CancellationToken ct){var t=await db.HandoverChecklistItems.FindAsync([id],ct); if(t is null)return NotFound(); db.Entry(t).CurrentValues[nameof(HandoverChecklistItem.Status)]="Completed"; db.Entry(t).CurrentValues[nameof(HandoverChecklistItem.CompletedBy)]=User.Identity?.Name??"system"; db.Entry(t).CurrentValues[nameof(HandoverChecklistItem.CompletedAt)]=DateTimeOffset.UtcNow; db.AuditEvents.Add(new AuditEvent("Handover checklist item completed","Handover",id,"Complete","system",t.Category,DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return Ok();}}
[ApiController, Route("api/readiness"), Authorize(Policy="Readiness.View")]
public sealed class ReadinessController(IReadinessAssessmentService svc):ControllerBase{[HttpGet] public Task<List<ReadinessCategoryDto>> Get(CancellationToken ct)=>svc.GetAsync(ct); [HttpGet("{category}")] public async Task<ActionResult<ReadinessCategoryDto>> GetOne(string category,CancellationToken ct)=>await svc.GetAsync(category,ct) is{} r?r:NotFound();}
[ApiController, Route("api/support-maintenance"), Authorize(Policy="SupportMaintenance.View")]
public sealed class SupportMaintenanceController(EProcurementDbContext db):ControllerBase{[HttpGet("service-levels")] public Task<List<SupportServiceLevel>> Levels(CancellationToken ct)=>db.SupportServiceLevels.AsNoTracking().OrderBy(x=>x.ResponseTimeHours).ToListAsync(ct);}
