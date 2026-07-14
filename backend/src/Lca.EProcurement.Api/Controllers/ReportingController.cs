using System.Text;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[Route("api/reporting")]
public sealed class ReportingController(IReportingApplicationService reporting) : ControllerBase
{
    [HttpGet("executive-dashboard")] public Task<ReportingDashboardDto> Executive([FromQuery] Guid? financialYearId, [FromQuery] string? department, [FromQuery] string? category, [FromQuery] Guid? supplierId, [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, [FromQuery] string? status, CancellationToken ct) => reporting.GetExecutiveDashboardAsync(F(financialYearId, department, category, supplierId, from, to, status), ct);
    [HttpGet("procurement-planning")] public Task<ReportingDashboardDto> Planning([FromQuery] Guid? financialYearId, [FromQuery] string? department, [FromQuery] string? category, [FromQuery] Guid? supplierId, [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, [FromQuery] string? status, CancellationToken ct) => reporting.GetProcurementPlanningAsync(F(financialYearId, department, category, supplierId, from, to, status), ct);
    [HttpGet("requisitions")] public Task<ReportingDashboardDto> Requisitions([FromQuery] Guid? financialYearId, [FromQuery] string? department, [FromQuery] string? category, [FromQuery] Guid? supplierId, [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, [FromQuery] string? status, CancellationToken ct) => reporting.GetRequisitionsAsync(F(financialYearId, department, category, supplierId, from, to, status), ct);
    [HttpGet("tenders")] public Task<ReportingDashboardDto> Tenders([FromQuery] Guid? financialYearId, [FromQuery] string? department, [FromQuery] string? category, [FromQuery] Guid? supplierId, [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, [FromQuery] string? status, CancellationToken ct) => reporting.GetTendersAsync(F(financialYearId, department, category, supplierId, from, to, status), ct);
    [HttpGet("supplier-performance")] public Task<ReportingDashboardDto> Suppliers([FromQuery] Guid? financialYearId, [FromQuery] string? department, [FromQuery] string? category, [FromQuery] Guid? supplierId, [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, [FromQuery] string? status, CancellationToken ct) => reporting.GetSupplierPerformanceAsync(F(financialYearId, department, category, supplierId, from, to, status), ct);
    [HttpGet("purchase-orders")] public Task<ReportingDashboardDto> PurchaseOrders([FromQuery] Guid? financialYearId, [FromQuery] string? department, [FromQuery] string? category, [FromQuery] Guid? supplierId, [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, [FromQuery] string? status, CancellationToken ct) => reporting.GetPurchaseOrdersAsync(F(financialYearId, department, category, supplierId, from, to, status), ct);
    [HttpGet("invoices")] public Task<ReportingDashboardDto> Invoices([FromQuery] Guid? financialYearId, [FromQuery] string? department, [FromQuery] string? category, [FromQuery] Guid? supplierId, [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, [FromQuery] string? status, CancellationToken ct) => reporting.GetInvoicesAsync(F(financialYearId, department, category, supplierId, from, to, status), ct);
    [HttpGet("po-invoice-analysis")] public Task<ReportingDashboardDto> PoInvoiceAnalysis([FromQuery] Guid? financialYearId, [FromQuery] string? department, [FromQuery] string? category, [FromQuery] Guid? supplierId, [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, [FromQuery] string? status, CancellationToken ct) => reporting.GetPoInvoiceAnalysisAsync(F(financialYearId, department, category, supplierId, from, to, status), ct);
    [HttpGet("compliance")] public Task<ReportingDashboardDto> Compliance([FromQuery] Guid? financialYearId, [FromQuery] string? department, [FromQuery] string? category, [FromQuery] Guid? supplierId, [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, [FromQuery] string? status, CancellationToken ct) => reporting.GetComplianceAsync(F(financialYearId, department, category, supplierId, from, to, status), ct);
    [HttpGet("audit")] public Task<ReportingDashboardDto> Audit([FromQuery] Guid? financialYearId, [FromQuery] string? department, [FromQuery] string? category, [FromQuery] Guid? supplierId, [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, [FromQuery] string? status, CancellationToken ct) => reporting.GetAuditAsync(F(financialYearId, department, category, supplierId, from, to, status), ct);
    [HttpGet("{reportCode}/export.csv")]
    public async Task<IActionResult> Export(string reportCode, [FromQuery] Guid? financialYearId, [FromQuery] string? department, [FromQuery] string? category, [FromQuery] Guid? supplierId, [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, [FromQuery] string? status, CancellationToken ct)
    {
        var csv = await reporting.ExportCsvAsync(reportCode, F(financialYearId, department, category, supplierId, from, to, status), ct);
        return csv is null ? NotFound() : File(Encoding.UTF8.GetBytes(csv), "text/csv", $"{reportCode}.csv");
    }
    static ReportingFilters F(Guid? financialYearId, string? department, string? category, Guid? supplierId, DateTimeOffset? from, DateTimeOffset? to, string? status) => new(financialYearId, department, category, supplierId, from, to, status);
}
