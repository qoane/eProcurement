import { PageHeader } from "../../components/ui/PageHeader";
import { Panel } from "../../components/ui/Panel";
import { Input } from "../../components/ui/Input";
import { Button } from "../../components/ui/Button";
export function RuleDesignerPage() {
  return (
    <>
      <PageHeader title="Rule designer" />
      <Panel>
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
            Applies to
            <Input />
          </label>
          <label>
            Expression
            <Input />
          </label>
        </div>
        <Button>Create rule</Button>
      </Panel>
    </>
  );
}
