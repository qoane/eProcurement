import { useEffect, useMemo, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { Panel } from "../../components/ui/Panel";
import { Input } from "../../components/ui/Input";
import { Button } from "../../components/ui/Button";
import { createRule, getRuleDesignerMetadata, getRuleHistory, simulateRule, validateRule } from "../../services/rulesApi";
import type { RuleDesignerMetadata, RuleExecutionLog, RuleSimulationResult, RuleValidationResult } from "../../types/api";

export function RuleDesignerPage() {
  const [metadata, setMetadata] = useState<RuleDesignerMetadata>({ categories: [], fields: [], functions: [], operators: [] });
  const [history, setHistory] = useState<RuleExecutionLog[]>([]);
  const [validation, setValidation] = useState<RuleValidationResult>();
  const [simulation, setSimulation] = useState<RuleSimulationResult | null>(null);
  const [form, setForm] = useState({ code: "", name: "", appliesTo: "SupplierRegistration", category: "Registration", expression: "IsNotEmpty(Field(\"legalName\")) && HasDocument(\"TaxClearance\")", failureMessage: "Supplier registration did not satisfy the published rule." });
  const suggestions = useMemo(() => [...metadata.fields, ...metadata.functions, ...metadata.operators], [metadata]);
  useEffect(() => { void getRuleDesignerMetadata().then((x) => setMetadata(x.data)); void getRuleHistory().then((x) => setHistory(x.data)); }, []);
  async function validate() { setValidation((await validateRule({ appliesTo: form.appliesTo, expression: form.expression })).data); }
  async function simulate() { setSimulation((await simulateRule({ appliesTo: form.appliesTo, expression: form.expression, values: { legalName: "Demo Supplier", email: "demo@example.org" }, documents: [{ documentType: "TaxClearance", fileName: "tax.pdf" }], categories: ["ICT"] })).data); }
  async function save() { await createRule({ ...form, isActive: true }); await getRuleHistory().then((x) => setHistory(x.data)); }
  return (
    <>
      <PageHeader title="Business Rule Designer" description="Design, validate, simulate, categorize, and publish configurable rules without hardcoding logic." />
      <div className="form-grid">
        <Panel title="Expression editor">
          <div className="form-grid">
            <label>Code<Input value={form.code} onChange={(e) => setForm({ ...form, code: e.target.value })} /></label>
            <label>Name<Input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} /></label>
            <label>Applies to<Input value={form.appliesTo} onChange={(e) => setForm({ ...form, appliesTo: e.target.value })} /></label>
            <label>Rule category<Input list="rule-categories" value={form.category} onChange={(e) => setForm({ ...form, category: e.target.value })} /></label>
            <datalist id="rule-categories">{metadata.categories.map((x) => <option key={x} value={x} />)}</datalist>
          </div>
          <label>Expression<textarea className="input" rows={8} value={form.expression} onChange={(e) => setForm({ ...form, expression: e.target.value })} /></label>
          <label>Failure message<Input value={form.failureMessage} onChange={(e) => setForm({ ...form, failureMessage: e.target.value })} /></label>
          <div className="actions"><Button onClick={validate}>Validate</Button><Button onClick={simulate}>Simulate</Button><Button onClick={save}>Save draft</Button></div>
        </Panel>
        <Panel title="Autocomplete"><div className="chip-list">{suggestions.map((x) => <button className="chip" key={x} onClick={() => setForm({ ...form, expression: `${form.expression} ${x}`.trim() })}>{x}</button>)}</div></Panel>
      </div>
      <Panel title="Validation and simulation">
        {validation && <p className={validation.isValid ? "text-success" : "text-danger"}>{validation.isValid ? "Expression is valid." : validation.errors.join(", ")}</p>}
        {simulation && <p>Simulation result: <strong>{simulation.passed ? "Passed" : "Failed"}</strong></p>}
      </Panel>
      <Panel title="Execution history"><div className="table-wrap"><table><tbody>{history.map((h) => <tr key={`${h.ruleCode}-${h.executedAt}`}><td>{h.ruleCode}</td><td>{h.outcome}</td><td>{new Date(h.executedAt).toLocaleString()}</td></tr>)}</tbody></table></div></Panel>
    </>
  );
}
