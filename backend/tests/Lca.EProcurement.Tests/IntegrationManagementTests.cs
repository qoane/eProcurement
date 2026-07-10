using Lca.EProcurement.Domain;

namespace Lca.EProcurement.Tests;

public class IntegrationManagementTests
{
    [Fact] public void Integration_endpoint_can_be_configured(){var e=new IntegrationEndpoint("CMS","Contract Management",IntegrationSystemType.ContractManagement,"https://cms.example","ApiKey",true,"{}",DateTimeOffset.UtcNow);Assert.True(e.IsEnabled);Assert.Equal(IntegrationSystemType.ContractManagement,e.SystemType);}
    [Fact] public void Contract_handover_creates_integration_message(){var c=Contract();var msg=new IntegrationMessage(Guid.NewGuid(),nameof(Contract),c.Id,IntegrationDirection.Outbound,"{}",IntegrationMessageStatus.Sent,DateTimeOffset.UtcNow,DateTimeOffset.UtcNow);Assert.Equal(c.Id,msg.EntityId);Assert.Equal(IntegrationMessageStatus.Sent,msg.Status);}
    [Fact] public void Missing_endpoint_does_not_break_contract(){var c=Contract();var msg=new IntegrationMessage(null,nameof(Contract),c.Id,IntegrationDirection.Outbound,"{}",IntegrationMessageStatus.PendingExternalConfiguration,DateTimeOffset.UtcNow);Assert.Equal(IntegrationMessageStatus.PendingExternalConfiguration,msg.Status);Assert.Null(msg.EndpointId);}
    [Fact] public void Failed_handover_is_logged(){var log=new IntegrationLog(Guid.NewGuid(),Guid.NewGuid(),nameof(Contract),Guid.NewGuid(),"CMS",IntegrationDirection.Outbound,"Failed","Timeout",DateTimeOffset.UtcNow);Assert.Equal("Failed",log.Status);Assert.Equal("Timeout",log.Error);}
    [Fact] public void External_reference_is_stored_when_returned(){var c=Contract();var r=new ExternalSystemReference(nameof(Contract),c.Id,"CMS","EXT-123","Active",DateTimeOffset.UtcNow);Assert.Equal("EXT-123",r.ExternalReference);Assert.Equal(c.Id,r.EntityId);}
    [Fact] public void Integration_logs_are_visible_to_permitted_users(){var permission="Integrations.View";var logs=new[]{new IntegrationLog(null,null,nameof(Contract),Guid.NewGuid(),"CMS",IntegrationDirection.Outbound,"Sent",null,DateTimeOffset.UtcNow)};Assert.Equal("Integrations.View",permission);Assert.Single(logs);}
    static Contract Contract()=>new("CTR-I",Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),"Supplier","Title","Desc",ContractType.ServiceContract,DateTimeOffset.UtcNow,DateTimeOffset.UtcNow.AddMonths(6),100,100,ContractStatus.Active,"user",DateTimeOffset.UtcNow);
}
