import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { Badge } from "../../components/ui/Badge";
import { WorkflowStageBar } from "../../components/workflow/WorkflowStageBar";
import { navigate } from "../../app/routes";
export function LandingPage() {
  return (
    <div>
      <section className="container hero">
        <div>
          <span className="eyebrow">
            Configured for Lesotho Communications Authority
          </span>
          <h1>BeeOnline Enterprise Platform</h1>
          <p className="muted">
            Configurable enterprise software for procurement, workflows, forms,
            rules and reporting.
          </p>
          <div className="toolbar">
            <Button onClick={() => navigate("/login")}>Sign in</Button>
            <Button variant="secondary">View platform modules</Button>
          </div>
        </div>
        <div className="mockdash">
          <Badge tone="info">Executive workspace</Badge>
          <div className="grid cols-3" style={{ marginTop: 16 }}>
            <Card>
              <strong>Suppliers</strong>
              <h2>—</h2>
            </Card>
            <Card>
              <strong>Tasks</strong>
              <h2>—</h2>
            </Card>
            <Card>
              <strong>Audit</strong>
              <h2>—</h2>
            </Card>
          </div>
          <div className="mockrow" />
          <div className="mockrow" />
          <div className="mockrow" />
        </div>
      </section>
      <section className="container grid cols-3">
        {[
          "Workflow Engine",
          "Business Rules Engine",
          "Dynamic Forms",
          "Supplier Management",
          "Audit Trails",
          "Reporting",
        ].map((x) => (
          <Card key={x}>
            <h3>{x}</h3>
            <p className="muted">
              Enterprise capability configured through the platform studio.
            </p>
          </Card>
        ))}
      </section>
      <section className="container panel" style={{ marginTop: 28 }}>
        <h2>Procurement lifecycle</h2>
        <WorkflowStageBar
          stages={[
            "Plan",
            "Source",
            "Submit",
            "Evaluate",
            "Award",
            "Manage",
            "Report",
          ]}
        />
      </section>
      <section className="container grid cols-3" style={{ marginTop: 28 }}>
        {[
          "Configuration first",
          "Enterprise security",
          "Rich audit history",
          "Modular architecture",
          "Reusable across government processes",
        ].map((x) => (
          <Card key={x}>
            <h3>{x}</h3>
          </Card>
        ))}
      </section>
      <footer className="container" style={{ padding: "36px 0" }}>
        BeeOnline Enterprise Platform · Configured for LCA
      </footer>
    </div>
  );
}
