import { navigate } from "../../app/routes";
import { Button } from "../../components/ui/Button";
import { Input } from "../../components/ui/Input";
export function LoginPage() {
  return (
    <div className="container hero">
      <div>
        <span className="eyebrow">BeeOnline Enterprise Platform</span>
        <h1>Secure enterprise access</h1>
        <p className="muted">
          Unified procurement operations, configurable workflows, dynamic forms
          and auditable decisions for Lesotho Communications Authority.
        </p>
      </div>
      <form
        className="panel"
        onSubmit={(e) => {
          e.preventDefault();
          navigate("/app/dashboard");
        }}
      >
        <h2>Sign in</h2>
        <p className="muted">
          Demo credentials: admin@lca.org.ls · Password: demo
        </p>
        <label>
          Email
          <Input type="email" defaultValue="admin@lca.org.ls" required />
        </label>
        <label>
          Password
          <Input type="password" defaultValue="demo" required />
        </label>
        <div className="page-header">
          <label style={{ display: "flex" }}>
            <input type="checkbox" /> Remember me
          </label>
          <a>Forgot password</a>
        </div>
        <Button>Sign in</Button>
      </form>
    </div>
  );
}
