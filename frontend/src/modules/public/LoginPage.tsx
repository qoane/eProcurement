import { navigate } from "../../app/routes";
import { Button } from "../../components/ui/Button";
import { Input } from "../../components/ui/Input";
import { Logo } from "../../components/ui/Logo";

export function LoginPage() {
  return (
    <main className="login-page">
      <section className="login-intro">
        <Logo />
        <h1>Sign in to ProcuraFlow</h1>
        <p className="muted">
          Configurable eProcurement Platform for Lesotho Communications
          Authority.
        </p>
      </section>
      <form
        className="login-panel"
        onSubmit={(e) => {
          e.preventDefault();
          navigate("/app/dashboard");
        }}
      >
        <div>
          <h2>Welcome back</h2>
          <p className="muted">Use your organisation account to continue.</p>
        </div>
        <label>
          Email
          <Input type="email" defaultValue="admin@lca.org.ls" required />
        </label>
        <label>
          Password
          <Input type="password" defaultValue="demo" required />
        </label>
        <div className="login-options">
          <label className="check-row">
            <input type="checkbox" /> Remember me
          </label>
          <a href="#">Forgot password</a>
        </div>
        <Button>Sign in</Button>
        <p className="demo-credentials">
          Demo credentials: <strong>admin@lca.org.ls</strong> /{" "}
          <strong>demo</strong>
        </p>
      </form>
    </main>
  );
}
