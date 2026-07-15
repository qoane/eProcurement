using Lca.EProcurement.Application;
using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed class RfpEvidencePhase1Tests
{
    [Fact] public async Task Rfp_compliance_requirements_and_commitments_are_seeded()
    {
        await using var db = Db(); await SeedData.SeedAsync(db);
        Assert.True(await db.ComplianceRequirements.CountAsync() >= 60);
        Assert.True(await db.ProposalCommitments.CountAsync() >= 20);
        Assert.Contains(await db.ComplianceRequirements.ToListAsync(), x => x.RequirementText == "Sealed bid submission");
    }

    [Fact] public async Task Compliance_matrix_returns_coverage_and_requirement_can_be_verified()
    {
        await using var db = Db(); await SeedData.SeedAsync(db);
        var service = new RfpEvidenceApplicationService(db, new ReadinessAssessmentService(db));
        var coverage = await service.GetCoverageAsync();
        Assert.True(coverage.Total > 0); Assert.True(coverage.CoveragePercentage > 0);
        var req = await db.ComplianceRequirements.FirstAsync();
        var verified = await service.VerifyRequirementAsync(req.Id, "uat-user");
        Assert.NotNull(verified); Assert.Contains(await db.AuditEvents.ToListAsync(), x => x.EventType == "Compliance requirement verified");
    }

    [Fact] public async Task Evidence_pack_export_excludes_secrets_and_creates_audit_event()
    {
        await using var db = Db(); await SeedData.SeedAsync(db);
        var pack = await new RfpEvidenceApplicationService(db, new ReadinessAssessmentService(db)).ExportPackAsync();
        var json = System.Text.Json.JsonSerializer.Serialize(pack);
        Assert.DoesNotContain("PasswordHash", json); Assert.DoesNotContain("SigningKey", json);
        Assert.Contains(await db.AuditEvents.ToListAsync(), x => x.EventType == "Evidence pack exported");
    }

    [Fact] public async Task Demo_verification_identifies_records_and_reset_is_blocked_without_setting()
    {
        await using var db = Db(); await SeedData.SeedAsync(db);
        var service = new DemoDataVerificationService(db);
        var status = await service.GetStatusAsync();
        Assert.Contains(status.Checks, x => x.Name == "At least three suppliers exist");
        Assert.False(await service.ResetAsync(false));
        Assert.Contains(await db.AuditEvents.ToListAsync(), x => x.EventType == "Demo reset attempted");
    }

    [Fact] public async Task Uat_run_records_result_and_pass_percentage_can_be_calculated()
    {
        await using var db = Db(); await SeedData.SeedAsync(db);
        var suite = await db.UatTestSuites.Include(x => x.TestCases).FirstAsync();
        var run = new UatTestRun("UAT-RUN-001", suite.Id, DateTimeOffset.UtcNow, null, "InProgress", "tester", "notes");
        db.UatTestRuns.Add(run); db.UatTestResults.Add(new UatTestResult(run.Id, suite.TestCases[0].Id, UatResultStatus.Pass, "ok", "evidence", null, DateTimeOffset.UtcNow, "tester")); await db.SaveChangesAsync();
        var results = await db.UatTestResults.Where(x => x.RunId == run.Id).ToListAsync();
        Assert.Equal(100, results.Count(x => x.Result == UatResultStatus.Pass) * 100 / results.Count);
    }

    [Fact] public async Task Training_implementation_handover_readiness_and_support_are_seeded()
    {
        await using var db = Db(); await SeedData.SeedAsync(db);
        Assert.True(await db.TrainingModules.CountAsync() >= 16);
        Assert.Equal(2, await db.TrainingModules.CountAsync(x => x.Audience == TrainingAudience.Supplier));
        Assert.Equal(9, await db.ImplementationPhases.CountAsync());
        Assert.True(await db.HandoverChecklistItems.CountAsync() >= 18);
        Assert.Equal(4, await db.SupportServiceLevels.CountAsync(x => x.IsActive));
        var readiness = await new ReadinessAssessmentService(db).GetAsync();
        Assert.Contains(readiness, x => x.Category == "UAT Readiness");
    }

    static EProcurementDbContext Db() => new(new DbContextOptionsBuilder<EProcurementDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
