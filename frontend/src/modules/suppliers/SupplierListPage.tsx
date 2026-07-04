import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { Input } from "../../components/ui/Input";
import { DataTable } from "../../components/ui/DataTable";
import { StatusBadge } from "../../components/ui/Badge";
import { Button } from "../../components/ui/Button";
import { navigate } from "../../app/routes";
import { getSuppliers } from "../../services/suppliersApi";
import type { Supplier } from "../../types/api";
export function SupplierListPage() {
  const [rows, setRows] = useState<Supplier[]>([]),
    [q, setQ] = useState("");
  useEffect(() => {
    void getSuppliers().then((r) => setRows(r.data));
  }, []);
  const f = rows.filter((s) =>
    JSON.stringify(s).toLowerCase().includes(q.toLowerCase()),
  );
  return (
    <>
      <PageHeader
        title="Suppliers"
        description="Enterprise supplier master data from the supplier API."
        actions={
          <Button onClick={() => navigate("/app/suppliers/register")}>
            Register supplier
          </Button>
        }
      />
      <Input
        aria-label="Search suppliers"
        placeholder="Search suppliers…"
        value={q}
        onChange={(e) => setQ(e.target.value)}
      />
      <section className="panel" style={{ marginTop: 16 }}>
        <DataTable
          rows={f}
          columns={[
            { header: "Reference", cell: (r) => r.referenceNumber },
            {
              header: "Supplier",
              cell: (r) => r.legalName || r.tradingName || "—",
            },
            {
              header: "Status",
              cell: (r) => <StatusBadge status={r.status} />,
            },
            { header: "Categories", cell: (r) => (r.categories || []).length },
            {
              header: "Action",
              cell: (r) => (
                <Button
                  variant="secondary"
                  onClick={() =>
                    navigate(`/app/suppliers/${r.referenceNumber}`)
                  }
                >
                  Open detail
                </Button>
              ),
            },
          ]}
        />
      </section>
    </>
  );
}
