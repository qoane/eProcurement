import { PageHeader } from "../../components/ui/PageHeader";
import { Card } from "../../components/ui/Card";
export function ConfigurationPage() {
  return (
    <>
      <PageHeader
        title="Configuration"
        description="Central platform configuration dashboard."
      />
      <div className="grid cols-3">
        {[
          "Workflow mappings",
          "Transition effects",
          "Document types",
          "Lookups",
          "Supplier categories",
        ].map((x) => (
          <Card key={x}>
            <h2>{x}</h2>
            <p className="muted">
              Manage configured {x.toLowerCase()} through platform APIs.
            </p>
          </Card>
        ))}
      </div>
    </>
  );
}
