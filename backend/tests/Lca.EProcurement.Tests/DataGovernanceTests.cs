using Lca.EProcurement.Application;
using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class DataGovernanceTests
{
    static EProcurementDbContext Db()
    {
        var options = new DbContextOptionsBuilder<EProcurementDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new EProcurementDbContext(options);
    }

    [Fact]
    public async Task Data_ownership_policy_and_retention_rule_can_be_created()
    {
        await using var db = Db();
        var service = new DataGovernanceApplicationService(db);
        var policy = await service.CreatePolicyAsync(new("LCA-DATA-OWNERSHIP", "LCA Data Ownership Policy", "All data remains owned by LCA.", DataGovernancePolicyType.Ownership, "Platform"));
        var rule = await service.CreateRetentionRuleAsync(new("RET-SUP", "Supplier retention", "Supplier", 2555, 1825, null));
        Assert.Equal("LCA-DATA-OWNERSHIP", policy.Code);
        Assert.True(rule.RequiresApprovalForDeletion);
    }

    [Fact]
    public async Task Sensitive_supplier_export_is_masked_without_sensitive_permission()
    {
        await using var db = Db();
        db.Suppliers.Add(new Supplier("SUP-001", "Demo Supplier", SupplierStatus.Approved));
        db.DataPrivacyClassifications.Add(new DataPrivacyClassification("Supplier", "Email", DataPrivacyClassificationLevel.PersonalData, "Personal data", true, true, true, DateTimeOffset.UtcNow, "test"));
        await db.SaveChangesAsync();
        var export = new DataExportApplicationService(db);
        var request = await export.RequestExportAsync(new("Supplier", "auditor", "CSV", false, "Audit export"));
        var csv = await export.DownloadAsync(request.Id, "auditor");
        Assert.Contains("s****m", csv);
        Assert.DoesNotContain("supplier@example.com", csv);
    }

    [Fact]
    public async Task Migration_validation_blocks_missing_required_fields_and_import()
    {
        await using var db = Db();
        var migration = new MigrationApplicationService(db);
        var template = await migration.CreateTemplateAsync(new("MIG-SUP", "Suppliers", "Supplier", "Supplier import", "CSV", [new("SupplierReference", "SupplierReference", "String", true, DisplayOrder: 1), new("LegalName", "LegalName", "String", true, DisplayOrder: 2)]));
        var batch = await migration.UploadBatchAsync(new(template.Id, "legacy", "sup.csv", "SupplierReference,LegalName\nSUP-001,", "tester"));
        var validated = await migration.ValidateBatchAsync(batch.Id);
        Assert.Equal(MigrationBatchStatus.ValidationFailed, validated!.Status);
        await Assert.ThrowsAsync<InvalidOperationException>(() => migration.ImportBatchAsync(batch.Id));
    }

    [Fact]
    public async Task Data_quality_detects_supplier_missing_category()
    {
        await using var db = Db();
        db.Suppliers.Add(new Supplier("SUP-001", "Supplier Without Category", SupplierStatus.Approved));
        await db.SaveChangesAsync();
        var results = await new DataQualityApplicationService(db).RunAsync();
        Assert.Contains(results, r => r.Code == "SUP-MISSING-CATEGORY");
    }
}
