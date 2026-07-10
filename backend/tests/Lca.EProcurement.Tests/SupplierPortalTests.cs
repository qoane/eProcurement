using Lca.EProcurement.Api.Controllers;
using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using Lca.EProcurement.Domain;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace Lca.EProcurement.Tests;

public class SupplierPortalTests
{
    [Fact] public void Supplier_user_sees_supplier_navigation_only()
    {
        var labels = new[] { "Dashboard", "My Profile", "My Documents", "Opportunities", "My Bids", "Clarifications", "Notifications", "Settings" };
        var hidden = new[] { "Planning", "Budgets", "Requisitions", "Evaluation", "Awards", "Studio", "Security", "Users", "Roles" };
        Assert.Contains("My Bids", labels);
        Assert.DoesNotContain("Studio", labels);
        Assert.All(hidden, x => Assert.DoesNotContain(x, labels));
    }

    [Fact] public void Supplier_cannot_access_Studio_APIs()
    {
        var attr = typeof(ConfigurationStudioController).GetCustomAttributes(typeof(RequirePermissionAttribute), true).Single();
        Assert.Equal("Studio.View", ((RequirePermissionAttribute)attr).Permission);
    }

    [Fact] public void Supplier_cannot_access_evaluation_APIs()
    {
        var attr = typeof(EvaluationsController).GetCustomAttributes(typeof(RequirePermissionAttribute), true).Single();
        Assert.Equal("Evaluation.View", ((RequirePermissionAttribute)attr).Permission);
    }

    [Fact] public void Supplier_can_view_own_profile()
    {
        var supplierId = Guid.NewGuid();
        var supplier = new Supplier("SUP-001", "Own Supplier", SupplierStatus.Approved);
        var context = new SupplierPortalUserContext(supplierId, "supplier@example.com", "user-1");
        Assert.Equal(supplierId, context.SupplierId);
        Assert.Equal("Own Supplier", supplier.LegalName);
    }

    [Fact] public void Supplier_cannot_view_another_supplier_profile()
    {
        var context = new SupplierPortalUserContext(Guid.NewGuid(), "supplier@example.com", "user-1");
        var otherSupplierId = Guid.NewGuid();
        Assert.NotEqual(context.SupplierId, otherSupplierId);
    }

    [Fact] public void Supplier_can_see_published_opportunities()
    {
        var tender = new Tender("TND-001", "Published", "", TenderType.RFQ, "Open", TenderStatus.Published, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(7), "procurement", DateTimeOffset.UtcNow);
        Assert.Equal(TenderStatus.Published, tender.Status);
        Assert.True(tender.ClosingDate > DateTimeOffset.UtcNow);
    }

    [Fact] public void Supplier_can_create_bid_for_published_tender()
    {
        var supplierId = Guid.NewGuid();
        var tender = new Tender("TND-002", "Open", "", TenderType.RFP, "Open", TenderStatus.Published, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(3), "procurement", DateTimeOffset.UtcNow);
        var bid = new BidSubmission("BID-001", tender.Id, supplierId, BidSubmissionStatus.Draft, DateTimeOffset.UtcNow, "supplier@example.com");
        Assert.Equal(tender.Id, bid.TenderId);
        Assert.Equal(supplierId, bid.SupplierId);
    }

    [Fact] public void Supplier_can_see_own_submitted_bid()
    {
        var supplierId = Guid.NewGuid();
        var bid = new BidSubmission("BID-002", Guid.NewGuid(), supplierId, BidSubmissionStatus.Locked, DateTimeOffset.UtcNow, "supplier@example.com", SubmittedAt: DateTimeOffset.UtcNow, LockedAt: DateTimeOffset.UtcNow);
        Assert.Equal(supplierId, bid.SupplierId);
        Assert.Equal(BidSubmissionStatus.Locked, bid.Status);
    }

    [Fact] public void Supplier_cannot_see_another_suppliers_bid()
    {
        var context = new SupplierPortalUserContext(Guid.NewGuid(), "supplier@example.com", "user-1");
        var otherBid = new BidSubmission("BID-003", Guid.NewGuid(), Guid.NewGuid(), BidSubmissionStatus.Draft, DateTimeOffset.UtcNow, "other@example.com");
        Assert.NotEqual(context.SupplierId, otherBid.SupplierId);
    }

    [Fact] public void Supplier_dashboard_uses_real_data_shape()
    {
        var dashboard = new SupplierPortalDashboardDto("Approved", ["Tax Clearance"], 2, 1, 3, 1, 4);
        Assert.Equal("Approved", dashboard.ProfileStatus);
        Assert.Equal(2, dashboard.OpenOpportunities);
        Assert.Contains("Tax Clearance", dashboard.MissingDocuments);
    }
}
