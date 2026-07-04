import { PageHeader } from "../../components/ui/PageHeader";
import { Panel } from "../../components/ui/Panel";
export function WorkflowTaskDetailPage() {
  return (
    <>
      <PageHeader title="Workflow task detail" />
      <Panel>
        <p className="muted">
          Task details are available from the inbox dataset and workflow
          instance APIs.
        </p>
      </Panel>
    </>
  );
}
