import { PageHeader } from "../../components/ui/PageHeader";
import { Panel } from "../../components/ui/Panel";
import { Input } from "../../components/ui/Input";
import { Button } from "../../components/ui/Button";
export function FormDesignerPage() {
  return (
    <>
      <PageHeader title="Form designer" />
      <Panel title="Definition">
        <div className="form-grid">
          <label>
            Code
            <Input />
          </label>
          <label>
            Name
            <Input />
          </label>
          <label>
            Entity type
            <Input />
          </label>
          <label>
            Section
            <Input />
          </label>
        </div>
        <Button>Create form definition</Button>
      </Panel>
    </>
  );
}
