import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { DataTable } from "../../components/ui/DataTable";
import { Button } from "../../components/ui/Button";
import { navigate } from "../../app/routes";
import { getRules, publishRule } from "../../services/rulesApi";
import type { Rule } from "../../types/api";
export function BusinessRulesPage() {
  const [r, setR] = useState<Rule[]>([]);
  useEffect(() => {
    void load();
  }, []);
  async function load() { setR((await getRules()).data); }
  async function publish(code: string) { await publishRule(code); await load(); }
  return (
    <>
      <PageHeader
        title="Business rules"
        description="Configured rules from the Business Rules Engine."
        actions={
          <Button onClick={() => navigate("/app/rules/designer")}>
            Rule designer
          </Button>
        }
      />
      <section className="panel">
        <DataTable
          rows={r}
          columns={[
            { header: "Code", cell: (x) => x.code },
            { header: "Name", cell: (x) => x.name },
            { header: "Category", cell: (x) => x.category ?? "General" },
            { header: "Applies to", cell: (x) => x.appliesTo },
            { header: "Status", cell: (x) => x.status ?? "Draft" },
            { header: "Expression", cell: (x) => x.expression },
            { header: "Publishing", cell: (x) => <Button onClick={() => publish(x.code)}>Publish</Button> },
          ]}
          empty="No business rules are configured yet."
        />
      </section>
    </>
  );
}
