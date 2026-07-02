import React from "react";
import { createRoot } from "react-dom/client";
import { CheckCircle, Clock, FileText, ShieldCheck } from "lucide-react";
import "./styles.css";

const suppliers = [
  {
    ref: "SUP-LCA-2026-0001",
    name: "Maseru ICT Supplies Pty Ltd",
    status: "UnderVerification",
    categories: "ICT Equipment",
  },
  {
    ref: "SUP-LCA-2026-0002",
    name: "Leribe Office Partners",
    status: "Approved",
    categories: "Office Supplies",
  },
];
const audit = [
  "Supplier created",
  "Document metadata uploaded",
  "Supplier submitted for verification",
  "Rule evaluated",
  "Workflow moved step",
];
function Badge({ status }: { status: string }) {
  return <span className={`badge ${status.toLowerCase()}`}>{status}</span>;
}
function App() {
  return (
    <div className="shell">
      <aside>
        <h2>LCA eProcurement</h2>
        <nav>
          {[
            "Dashboard",
            "Supplier list",
            "Supplier registration",
            "Supplier verification",
            "Supplier detail",
            "Audit timeline",
          ].map((x) => (
            <a>{x}</a>
          ))}
        </nav>
      </aside>
      <main>
        <header>
          <div>
            <p>Configurable procurement automation platform</p>
            <h1>Supplier Onboarding Command Centre</h1>
          </div>
          <button>Register supplier</button>
        </header>
        <section className="cards">
          <article>
            <Clock />
            <b>12</b>
            <span>Pending verification</span>
          </article>
          <article>
            <CheckCircle />
            <b>48</b>
            <span>Approved suppliers</span>
          </article>
          <article>
            <ShieldCheck />
            <b>3</b>
            <span>Workflow tasks</span>
          </article>
          <article>
            <FileText />
            <b>2</b>
            <span>Rejected suppliers</span>
          </article>
        </section>
        <section className="grid">
          <div className="panel wide">
            <h2>Supplier verification queue</h2>
            <table>
              <thead>
                <tr>
                  <th>Reference</th>
                  <th>Supplier</th>
                  <th>Categories</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                {suppliers.map((s) => (
                  <tr>
                    <td>{s.ref}</td>
                    <td>{s.name}</td>
                    <td>{s.categories}</td>
                    <td>
                      <Badge status={s.status} />
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <div className="panel">
            <h2>Audit timeline</h2>
            <ol className="timeline">
              {audit.map((a, i) => (
                <li>
                  <span>{i + 1}</span>
                  {a}
                </li>
              ))}
            </ol>
          </div>
          <div className="panel">
            <h2>Registration workspace</h2>
            <label>
              Legal name
              <input defaultValue="Maseru ICT Supplies Pty Ltd" />
            </label>
            <label>
              Category
              <select>
                <option>ICT Equipment</option>
              </select>
            </label>
            <label>
              Documents<div className="doc">Company registration.pdf</div>
              <div className="doc">Tax clearance.pdf</div>
            </label>
          </div>
          <div className="panel">
            <h2>Verification rules</h2>
            <p>✓ Company registration document</p>
            <p>✓ Tax clearance document</p>
            <p>✓ At least one category</p>
            <button>Approve supplier</button>
          </div>
        </section>
      </main>
    </div>
  );
}
createRoot(document.getElementById("root")!).render(<App />);
