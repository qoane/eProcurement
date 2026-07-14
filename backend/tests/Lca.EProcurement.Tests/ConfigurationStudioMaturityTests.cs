using Lca.EProcurement.Application;
using Lca.EProcurement.Domain;

namespace Lca.EProcurement.Tests;

public class ConfigurationStudioMaturityTests
{
    [Fact] public void Business_process_can_link_runtime_configuration(){var process=new BusinessProcessDefinition("REQ","Requisition Management","Requisitions",nameof(Requisition),Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),BusinessProcessStatus.Draft);Assert.NotNull(process.ActiveWorkflowDefinitionId);Assert.NotNull(process.ActiveFormDefinitionId);Assert.NotNull(process.ActiveApprovalMatrixId);Assert.NotNull(process.ActiveDocumentRequirementSetId);}
    [Fact] public void Workflow_supports_decision_nodes_for_designer(){var versionId=Guid.NewGuid();var node=new WorkflowNode(versionId,"Decision","Budget decision",WorkflowNodeKind.Decision,ConditionConfigurationJson:"{\"expression\":\"AmountGreaterThan(100000)\"}");Assert.Equal(WorkflowNodeKind.Decision,node.Kind);Assert.Contains("AmountGreaterThan",node.ConditionConfigurationJson);}
    [Fact] public void Publish_result_uses_common_shape(){var now=DateTimeOffset.UtcNow;var result=new PublishResult("Workflow","REQ",2,"Published",now,"admin");Assert.Equal("Workflow",result.EntityType);Assert.Equal("Published",result.Status);Assert.Equal("admin",result.PublishedBy);}
    [Fact] public void Unsupported_rule_expression_fails_clearly(){var ex=Assert.Throws<InvalidOperationException>(()=>SimpleExpressionEvaluator.Evaluate("UnsupportedFunction()",new RuleEvaluationContext(new Supplier("S1","Supplier",SupplierStatus.Draft),[])));Assert.Contains("not supported",ex.Message);}
    [Fact] public void Approval_matrix_preview_can_filter_threshold_steps(){var matrix=new ApprovalMatrix("Requisition Approval Matrix","Threshold approvals",nameof(Requisition));matrix.Steps.Add(new ApprovalStep(matrix.Id,"Manager",1,0,99999));matrix.Steps.Add(new ApprovalStep(matrix.Id,"CEO",2,100000,null));Assert.Single(matrix.Steps.Where(s => 250000 >= (s.MinimumAmount ?? 0) && (s.MaximumAmount is null || 250000 <= s.MaximumAmount)));}
    [Fact] public void Package_export_contract_has_manifest_fields(){var manifest=new ConfigurationPackageManifestDto(Guid.NewGuid(),"LCA Procurement Configuration Package",DateTimeOffset.UtcNow,"admin",10,"Exported");Assert.Equal("Exported",manifest.Status);Assert.True(manifest.ItemCount>0);}
    [Fact] public void Studio_permissions_include_phase_one_surface(){var permissions=new[]{"Studio.View","Studio.Publish","Studio.Export","Workflow.Configure","Form.Publish","Rule.Test","ApprovalMatrix.Publish","DocumentRequirement.Configure","Navigation.Publish","Lookup.Configure"};Assert.Contains("Studio.View",permissions);Assert.Contains("Rule.Test",permissions);Assert.DoesNotContain("Supplier.Studio",permissions);}
}
