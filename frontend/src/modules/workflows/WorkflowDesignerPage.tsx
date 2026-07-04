import { PageHeader } from "../../components/ui/PageHeader";
import { Panel } from "../../components/ui/Panel";
import { Button } from "../../components/ui/Button";
import { Drawer } from "../../components/ui/Drawer";
export function WorkflowDesignerPage() {
  return (
    <>
      <PageHeader
        title="Workflow designer"
        description="Professional designer shell for configured workflow definitions."
      />
      <div className="toolbar">
        <Button>Create workflow</Button>
        <Button variant="secondary">Publish</Button>
      </div>
      <div className="grid cols-3">
        <Panel title="Node list">
          <p className="muted">Start, task, automatic and terminal nodes.</p>
        </Panel>
        <Panel title="Canvas">
          <div className="mockdash">
            <div className="stagebar">
              <span className="stage">Start</span>
              <span className="stage">Review</span>
              <span className="stage">Approve</span>
            </div>
          </div>
        </Panel>
        <Drawer>
          <h2>Properties</h2>
          <p className="muted">
            Select a node or transition to configure metadata.
          </p>
        </Drawer>
      </div>
      <Panel title="Transition list">
        <p className="muted">
          Transitions are loaded from workflow definition versions.
        </p>
      </Panel>
    </>
  );
}
