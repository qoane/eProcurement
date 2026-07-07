import { useState } from "react";
import { navigate } from "../../app/routes";
import { useAuth } from "../../auth/AuthContext";
import { Button } from "../../components/ui/Button";
import { Input } from "../../components/ui/Input";
import { Logo } from "../../components/ui/Logo";

export function LoginPage() {
  const auth = useAuth();
  const [email, setEmail] = useState("admin@lca.org.ls");
  const [password, setPassword] = useState("demo");
  const [error, setError] = useState("");
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
        onSubmit={async (e) => {
          e.preventDefault();
          setError("");
          if (await auth.login(email, password)) navigate("/app/dashboard");
          else setError("Invalid email or password.");
        }}
      >
        <div>
          <h2>Welcome back</h2>
          <p className="muted">Use your organisation account to continue.</p>
        </div>
        <label>
          Email
          <Input type="email" value={email} onChange={(e) => setEmail(e.currentTarget.value)} required />
        </label>
        <label>
          Password
          <Input type="password" value={password} onChange={(e) => setPassword(e.currentTarget.value)} required />
        </label>
        <div className="login-options">
          <label className="check-row">
            <input type="checkbox" /> Remember me
          </label>
          <a href="#">Forgot password</a>
        </div>
        {error && <p className="text-danger">{error}</p>}
        <Button>Sign in</Button>
        <p className="demo-credentials">
          Demo credentials: <strong>admin@lca.org.ls</strong> /{" "}
          <strong>demo</strong>
        </p>
      </form>
    </main>
  );
}
