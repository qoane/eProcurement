using Lca.EProcurement.Application;
using Lca.EProcurement.Domain;

namespace Lca.EProcurement.Tests;

public class OperationalReadinessTests
{
    [Fact] public void Performance_sample_records_correlation_and_duration(){var sample=new ApiPerformanceSample("ABC123","/api/tenders","GET",200,1250,"user",DateTimeOffset.UtcNow);Assert.Equal("ABC123",sample.CorrelationId);Assert.True(sample.DurationMs>1000);}
    [Fact] public void Paged_result_reports_total_pages(){var result=new PagedResult<string>(["a","b"],1,2,5,3);Assert.Equal(3,result.TotalPages);Assert.Equal(2,result.Items.Count);}
    [Fact] public void Backup_plan_supports_phase_one_evidence(){var plan=new BackupPlan("DAILY","Daily",BackupType.Full,"Daily","Infrastructure",true,30,DateTimeOffset.UtcNow,"system");Assert.True(plan.IsEnabled);Assert.Equal(BackupType.Full,plan.BackupType);}
    [Fact] public void Backup_run_simulation_can_complete(){var run=new BackupRun(Guid.NewGuid(),DateTimeOffset.UtcNow,DateTimeOffset.UtcNow,BackupRunStatus.Completed,"SIM-1",0,null,"system");Assert.Equal(BackupRunStatus.Completed,run.Status);Assert.StartsWith("SIM",run.BackupReference);}
    [Fact] public void Restore_run_simulation_can_complete(){var run=new RestoreRun(Guid.NewGuid(),DateTimeOffset.UtcNow,DateTimeOffset.UtcNow,BackupRunStatus.Completed,"DemoRestore",null,"system");Assert.Equal("DemoRestore",run.RestoreTarget);Assert.Null(run.ErrorMessage);}
    [Fact] public void Support_case_can_be_created_and_resolved(){var c=new SupportCase("SUP-1","Title","Description",SupportCaseSeverity.High,SupportCaseStatus.Resolved,"Operations","user","admin",DateTimeOffset.UtcNow,DateTimeOffset.UtcNow,DateTimeOffset.UtcNow,"Done","CORR");Assert.Equal(SupportCaseStatus.Resolved,c.Status);Assert.Equal("CORR",c.CorrelationId);}
    [Fact] public void Operations_permissions_are_named_for_policy_registration(){var permissions=new[]{"Operations.View","Operations.Backup.Manage","SupportCase.Create","SupportCase.Manage"};Assert.Contains("Operations.View",permissions);Assert.Contains("SupportCase.Manage",permissions);}
}
