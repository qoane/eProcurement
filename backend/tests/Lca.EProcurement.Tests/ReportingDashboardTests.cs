using Lca.EProcurement.Application;
using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed class ReportingDashboardTests
{
    [Fact] public async Task Executive_dashboard_returns_real_counts()
    {
        await using var db = Db(); var fy = Guid.NewGuid(); db.ProcurementCases.Add(new ProcurementCase("CASE-1", "Network", "", fy, "ICT", ProcurementCaseStatus.Active, DateTimeOffset.UtcNow, "u")); db.Tenders.Add(new Tender("T-1", "Tender", "", TenderType.RFQ, "Open", TenderStatus.Published, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(10), "u", DateTimeOffset.UtcNow)); await db.SaveChangesAsync();
        var report = await new ReportingApplicationService(db).GetExecutiveDashboardAsync(new ReportingFilters());
        Assert.Equal(1, report.Metrics.Single(x => x.Code == "activeProcurementCases").Value); Assert.Equal(1, report.Metrics.Single(x => x.Code == "publishedTenders").Value);
    }
    [Fact] public async Task Requisition_dashboard_calculates_statuses()
    {
        await using var db = Db(); var fy = Guid.NewGuid(); var cc = Guid.NewGuid(); db.Requisitions.AddRange(new Requisition("R1","A","","ICT",cc,fy,"u",DateTimeOffset.UtcNow,"Normal",10,RequisitionStatus.Draft,DateTimeOffset.UtcNow), new Requisition("R2","B","","ICT",cc,fy,"u",DateTimeOffset.UtcNow,"Normal",20,RequisitionStatus.Approved,DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(-2), DateTimeOffset.UtcNow)); await db.SaveChangesAsync();
        var report = await new ReportingApplicationService(db).GetRequisitionsAsync(new ReportingFilters());
        Assert.Equal(2, report.Metrics.Single(x => x.Code == "totalRequisitions").Value); Assert.Equal(1, report.Metrics.Single(x => x.Code == "approved").Value);
    }
    [Fact] public async Task Tender_dashboard_shows_published_and_closing_soon()
    {
        await using var db = Db(); db.Tenders.Add(new Tender("T-2", "Tender", "", TenderType.RFQ, "Open", TenderStatus.Published, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(7), "u", DateTimeOffset.UtcNow)); await db.SaveChangesAsync();
        var report = await new ReportingApplicationService(db).GetTendersAsync(new ReportingFilters());
        Assert.Equal(1, report.Metrics.Single(x => x.Code == "publishedTenders").Value); Assert.Equal(1, report.Metrics.Single(x => x.Code == "closingSoon").Value);
    }
    [Fact] public async Task Supplier_dashboard_counts_approved_and_suspended_suppliers()
    {
        await using var db = Db(); db.Suppliers.AddRange(new Supplier("S1","A",SupplierStatus.Approved), new Supplier("S2","B",SupplierStatus.Suspended)); await db.SaveChangesAsync();
        var report = await new ReportingApplicationService(db).GetSupplierPerformanceAsync(new ReportingFilters());
        Assert.Equal(1, report.Metrics.Single(x => x.Code == "approvedSuppliers").Value); Assert.Equal(1, report.Metrics.Single(x => x.Code == "suspendedSuppliers").Value);
    }
    [Fact] public async Task PO_dashboard_calculates_issued_and_closed_pos()
    {
        await using var db = Db(); var s = Guid.NewGuid(); var a = Guid.NewGuid(); db.PurchaseOrders.AddRange(new PurchaseOrder("PO1",a,s,"Supplier",DateTimeOffset.UtcNow,DateTimeOffset.UtcNow,"LSL",100,PurchaseOrderStatus.Issued,"u",DateTimeOffset.UtcNow), new PurchaseOrder("PO2",a,s,"Supplier",DateTimeOffset.UtcNow,DateTimeOffset.UtcNow,"LSL",200,PurchaseOrderStatus.Closed,"u",DateTimeOffset.UtcNow)); await db.SaveChangesAsync();
        var report = await new ReportingApplicationService(db).GetPurchaseOrdersAsync(new ReportingFilters());
        Assert.Equal(1, report.Metrics.Single(x => x.Code == "issuedPOs").Value); Assert.Equal(1, report.Metrics.Single(x => x.Code == "closedPOs").Value);
    }
    [Fact] public async Task Compliance_dashboard_includes_audit_exceptions()
    {
        await using var db = Db(); db.AuditEvents.Add(new AuditEvent("Compliance Exception", "Tender", Guid.NewGuid(), "T", "u", "Missing document", DateTimeOffset.UtcNow)); await db.SaveChangesAsync();
        var report = await new ReportingApplicationService(db).GetComplianceAsync(new ReportingFilters());
        Assert.Equal(1, report.Metrics.Single(x => x.Code == "auditExceptions").Value);
    }
    [Fact] public async Task CSV_export_works()
    {
        await using var db = Db(); var csv = await new ReportingApplicationService(db).ExportCsvAsync("requisitions", new ReportingFilters());
        Assert.StartsWith("Area,Metric,Value,Source", csv); Assert.Contains("Total requisitions", csv);
    }
    static EProcurementDbContext Db() => new(new DbContextOptionsBuilder<EProcurementDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
