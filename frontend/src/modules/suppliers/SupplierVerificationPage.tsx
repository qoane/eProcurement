import { PageHeader } from "../../components/ui/PageHeader";
import { Panel } from "../../components/ui/Panel";
import { Button } from "../../components/ui/Button";
export function SupplierVerificationPage() {
  return (
    <>
      <PageHeader
        title="Supplier verification"
        description="Workflow task style verification screen."
      />
      <Panel>
        <p className="muted">
          Select a supplier workflow task from the inbox to complete
          verification actions.
        </p>
        <Button>Complete verification</Button>
      </Panel>
    </>
  );
}
