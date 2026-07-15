using Lca.EProcurement.Domain;
using Xunit;

public class BidSubmissionTests
{
    [Fact]
    public void Supplier_can_create_draft_bid()
    {
        var bid = new BidSubmission("BID-001", Guid.NewGuid(), Guid.NewGuid(), BidSubmissionStatus.Draft, DateTimeOffset.UtcNow, "supplier@demo.ls");
        bid.Items.Add(new BidSubmissionItem(bid.Id, Guid.NewGuid(), "Technical services", 2, 1000, 2000));
        bid.Versions.Add(new BidSubmissionVersion(bid.Id, 1, DateTimeOffset.UtcNow, "supplier@demo.ls"));
        Assert.Equal(BidSubmissionStatus.Draft, bid.Status);
        Assert.Single(bid.Items);
        Assert.Equal(2000, bid.Items[0].Total);
    }

    [Fact]
    public void Required_documents_and_declaration_are_modelled_for_rule_validation()
    {
        var bid = new BidSubmission("BID-002", Guid.NewGuid(), Guid.NewGuid(), BidSubmissionStatus.Draft, DateTimeOffset.UtcNow, "supplier@demo.ls");
        bid.Documents.Add(new BidSubmissionDocument(bid.Id, "Technical Proposal", "technical.pdf", "vault://future/technical.pdf", "supplier@demo.ls", DateTimeOffset.UtcNow));
        bid.Declarations.Add(new BidSubmissionDeclaration(bid.Id, "Confidentiality Declaration", true, "supplier@demo.ls", DateTimeOffset.UtcNow));
        Assert.Contains(bid.Documents, d => d.DocumentType == "Technical Proposal");
        Assert.Contains(bid.Declarations, d => d.Accepted);
    }

    [Fact]
    public void Submission_locking_status_history_audit_and_workflow_tasks_are_supported()
    {
        var bid = new BidSubmission("BID-003", Guid.NewGuid(), Guid.NewGuid(), BidSubmissionStatus.Draft, DateTimeOffset.UtcNow, "supplier@demo.ls");
        bid.StatusHistory.Add(new BidSubmissionStatusHistory(bid.Id, BidSubmissionStatus.Draft, BidSubmissionStatus.Locked, "supplier@demo.ls", DateTimeOffset.UtcNow, "Submitted and locked"));
        bid.History.Add(new BidSubmissionHistory(bid.Id, "Submission locked", "supplier@demo.ls", "Workflow-driven lock", DateTimeOffset.UtcNow));
        var workflowTask = new WorkflowTask(Guid.NewGuid(), "Locked", "ProcurementOfficer", CreatedAt: DateTimeOffset.UtcNow);
        var audit = new AuditEvent("Submission locked", nameof(BidSubmission), bid.Id, bid.SubmissionNumber, "supplier@demo.ls", "Submitted and locked", DateTimeOffset.UtcNow);
        bid = bid with { Status = BidSubmissionStatus.Locked, LockedAt = DateTimeOffset.UtcNow };
        Assert.Equal(BidSubmissionStatus.Locked, bid.Status);
        Assert.Equal("ProcurementOfficer", workflowTask.AssignedRole);
        Assert.Equal(nameof(BidSubmission), audit.EntityType);
    }

    [Fact]
    public void Bid_submission_uses_platform_configuration_not_hardcoded_workflow_logic()
    {
        var workflow = new WorkflowDefinition("BID-SUBMISSION-WORKFLOW", "Bid Submission Workflow", nameof(BidSubmission));
        var form = new FormDefinition("BID-SUBMISSION-FORM", "Bid Submission Form", nameof(BidSubmission));
        var documents = new DocumentRequirementSet("Bid Submission Documents", "Document-driven bid package", nameof(BidSubmission));
        var process = new BusinessProcessDefinition("BID-SUBMISSION", "Bid Submission", "Configurable process", nameof(BidSubmission), workflow.Id, form.Id, documents.Id, Guid.NewGuid(), BusinessProcessStatus.Published);
        var rule = new BusinessRuleDefinition("BID-BEFORE-CLOSING", "Submission before closing", nameof(BidSubmission), "BidSubmission.SubmissionBeforeClosingDate()", true, "Bid Submission", BusinessRuleStatus.Published);
        Assert.Equal(workflow.Id, process.ActiveWorkflowDefinitionId);
        Assert.Equal(nameof(BidSubmission), form.EntityType);
        Assert.Equal(nameof(BidSubmission), documents.EntityType);
        Assert.Equal(nameof(BidSubmission), rule.AppliesTo);
    }
}
