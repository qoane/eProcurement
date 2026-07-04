import { navigate } from "../../app/routes";
import { Button } from "../../components/ui/Button";
import { Logo } from "../../components/ui/Logo";

export function LandingPage() {
  return (
    <main className="landing-page">
      <section className="landing-card" aria-labelledby="landing-title">
        <Logo />
        <p className="landing-kicker">Configurable eProcurement Platform</p>
        <h1 id="landing-title">Procurement, configured with confidence.</h1>
        <p className="landing-statement">
          ProcuraFlow helps organisations manage procurement through
          configurable workflows, dynamic forms, business rules, supplier
          management and audit-ready reporting.
        </p>
        <div className="landing-actions">
          <Button onClick={() => navigate("/login")}>Sign in</Button>
          <Button variant="secondary" onClick={() => navigate("/login")}>
            Learn more
          </Button>
        </div>
      </section>
      <footer className="landing-footer">
        Configured for Lesotho Communications Authority
      </footer>
    </main>
  );
}
