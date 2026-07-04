import { navigate } from "../../app/routes";
import { Button } from "../../components/ui/Button";
import { Logo } from "../../components/ui/Logo";

const capabilities = [
  {
    title: "Workflow automation",
    description:
      "Orchestrate procurement approvals, exceptions and handoffs with configurable stages.",
  },
  {
    title: "Supplier onboarding",
    description:
      "Guide suppliers through registration, verification and readiness reviews.",
  },
  {
    title: "Configurable forms",
    description:
      "Adapt intake, evaluation and compliance forms without rebuilding core workflows.",
  },
  {
    title: "Audit-ready reporting",
    description:
      "Keep transparent decision trails and procurement evidence available for review.",
  },
];

const dashboardStats = [
  "24 active workflows",
  "98% audit coverage",
  "12 supplier reviews",
];

export function LandingPage() {
  return (
    <main className="landing-page">
      <section className="landing-card" aria-labelledby="landing-title">
        <Logo />
        <p className="landing-kicker">ProcuraFlow Command Center</p>
        <h1 id="landing-title">Procurement, configured with confidence.</h1>
        <p className="landing-statement">
          Configure workflows, suppliers, approvals, forms, and audit trails in
          one place. Built for transparent public procurement.
        </p>
        <div className="landing-actions">
          <Button onClick={() => navigate("/login")}>Sign in</Button>
          <Button variant="secondary" onClick={() => navigate("/login")}>
            Learn more
          </Button>
        </div>
      </section>

      <section
        className="landing-showcase"
        aria-label="ProcuraFlow platform overview"
      >
        <div
          className="landing-dashboard"
          aria-label="Procurement dashboard highlights"
        >
          <div>
            <p className="landing-dashboard-label">Live public procurement</p>
            <h2>Transparent operations at a glance</h2>
          </div>
          <div className="landing-stat-grid">
            {dashboardStats.map((stat) => (
              <div className="landing-stat" key={stat}>
                <span>{stat.split(" ")[0]}</span>
                <p>{stat.split(" ").slice(1).join(" ")}</p>
              </div>
            ))}
          </div>
        </div>

        <div
          className="landing-capabilities"
          aria-label="Platform capabilities"
        >
          {capabilities.map((capability) => (
            <article className="landing-feature-card" key={capability.title}>
              <span aria-hidden="true" />
              <h3>{capability.title}</h3>
              <p>{capability.description}</p>
            </article>
          ))}
        </div>
      </section>

      <footer className="landing-footer">
        Configured for Lesotho Communications Authority
      </footer>
    </main>
  );
}
