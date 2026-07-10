import { navigate } from "../app/routes";
import { Button } from "../components/ui/Button";
import { Logo } from "../components/ui/Logo";

const navItems = [
  ["Home", "/public"],
  ["Opportunities", "/public/opportunities"],
  ["Award Notices", "/public/awards"],
  ["Calendar", "/public/calendar"],
  ["Supplier Registration", "/public/register"],
  ["Help", "/public/help"],
  ["Login", "/login"],
] as const;

export function PublicLayout({ children }: { children: React.ReactNode }) {
  const go = (path: string) => navigate(path);
  const search = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const form = new FormData(event.currentTarget);
    const query = String(form.get("q") || "").trim();
    navigate(query ? `/public/opportunities?search=${encodeURIComponent(query)}` : "/public/opportunities");
  };
  return (
    <div className="public public-site">
      <a className="skip-link" href="#public-content">Skip to content</a>
      <header className="public-site-header">
        <button className="public-brand" onClick={() => go("/public")} aria-label="Public Procurement Portal home">
          <span className="public-brand-mark">LCA</span>
          <span><strong>LCA Public Procurement</strong><small>Lesotho Communications Authority</small></span>
        </button>
        <Logo />
        <form className="public-site-search" onSubmit={search} role="search">
          <label className="sr-only" htmlFor="public-search">Search public opportunities</label>
          <input id="public-search" name="q" placeholder="Search tenders" />
        </form>
        <div className="public-site-actions">
          <Button variant="secondary" onClick={() => go("/login")}>Login</Button>
          <Button onClick={() => go("/public/register")}>Supplier Registration</Button>
        </div>
        <nav className="public-site-nav" aria-label="Public portal">
          {navItems.map(([label, path]) => <button key={path} onClick={() => go(path)}>{label}</button>)}
        </nav>
      </header>
      <main id="public-content">{children}</main>
      <footer className="public-site-footer">
        <strong>ProcuraFlow for LCA Procurement</strong>
        <span>Transparent public procurement notices, calendars, award notices, and supplier guidance.</span>
        <span>procurement@lca.org.ls · +266 2222 4300</span>
      </footer>
    </div>
  );
}
