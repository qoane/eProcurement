import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { Panel } from "../../components/ui/Panel";
import { WorkflowStageBar } from "../../components/workflow/WorkflowStageBar";
import { WorkflowTimeline } from "../../components/workflow/WorkflowTimeline";
import { getWorkflows } from "../../services/workflowsApi";
import type { WorkflowDefinition } from "../../types/api";
export function WorkflowDetailPage({ code }: { code: string }) {
  const [w, setW] = useState<WorkflowDefinition>();
  useEffect(() => {
    void getWorkflows().then((r) => setW(r.data.find((x) => x.code === code)));
  }, [code]);
  const v = w?.versions?.[0];
  return (
    <>
      <PageHeader
        title={w?.name || code}
        description="Workflow nodes and transitions."
      />
      <Panel title="Stages">
        <WorkflowStageBar
          stages={(v?.nodes || []).map((n) => n.name || n.code)}
        />
      </Panel>
      <Panel title="Transitions">
        <WorkflowTimeline transitions={v?.transitions || []} />
      </Panel>
    </>
  );
}
