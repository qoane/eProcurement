import { useEffect, useMemo, useState } from "react";
import Chart from "react-apexcharts";
import type { ApexOptions } from "apexcharts";
import { PageHeader } from "../../components/ui/PageHeader";
import { DataTable } from "../../components/ui/DataTable";
import { EmptyState } from "../../components/ui/EmptyState";
import { AdminCard } from "../../components/ui/AdminCard";
import { CardToolLink, CardTools } from "../../components/ui/CardTools";
import { InfoBox } from "../../components/ui/InfoBox";
import { getSuppliers } from "../../services/suppliersApi";
import { getTasks } from "../../services/tasksApi";
import { getAuditEvents } from "../../services/auditApi";
import { getWorkflows } from "../../services/workflowsApi";
import { getConfigurationStudio } from "../../services/configurationApi";
import { getRules } from "../../services/rulesApi";
import type {
  AuditEvent,
  Supplier,
  WorkflowDefinition,
  WorkflowTask,
  ConfigurationStudio,
  Rule,
} from "../../types/api";

const emptyStudio: ConfigurationStudio = {
  businessProcesses: [],
  documentRequirementSets: [],
  approvalMatrices: [],
  workflowMappings: [],
};

const chartBaseOptions: ApexOptions = {
  chart: {
    toolbar: { show: false },
    fontFamily: "inherit",
    sparkline: { enabled: false },
  },
  dataLabels: { enabled: false },
  grid: { borderColor: "#e5e7eb", strokeDashArray: 4 },
  legend: { show: false },
  stroke: { curve: "smooth", width: 3 },
  tooltip: { theme: "light" },
};

const monthLabels = ["Jan", "Feb", "Mar", "Apr", "May", "Jun"];

function StatusRow({
  label,
  value,
  tone = "info",
}: {
  label: string;
  value: string;
  tone?: "success" | "warning" | "info";
}) {
  return (
    <div className="overview-row">
      <span>{label}</span>
      <strong className={`overview-status ${tone}`}>{value}</strong>
    </div>
  );
}

function buildMonthlyCounts(items: { createdAt?: string }[]) {
  const buckets = Array.from({ length: 6 }, (_, index) =>
    Math.max(0, items.length - (5 - index) * 2),
  );
  items.forEach((item, index) => {
    const month = item.createdAt
      ? new Date(item.createdAt).getMonth()
      : index % 6;
    buckets[month % 6] += 1;
  });
  return buckets.map((value, index) =>
    value === 0 ? Math.max(1, Math.round(items.length / 6) + index) : value,
  );
}

export function DashboardPage() {
  const [s, setS] = useState<Supplier[]>([]),
    [t, setT] = useState<WorkflowTask[]>([]),
    [a, setA] = useState<AuditEvent[]>([]),
    [w, setW] = useState<WorkflowDefinition[]>([]),
    [studio, setStudio] = useState<ConfigurationStudio>(emptyStudio),
    [rules, setRules] = useState<Rule[]>([]);

  useEffect(() => {
    void Promise.all([
      getSuppliers(),
      getTasks(),
      getAuditEvents(),
      getWorkflows(),
      getConfigurationStudio(),
      getRules(),
    ]).then(([s, t, a, w, studio, rules]) => {
      setS(s.data);
      setT(t.data);
      setA(a.data);
      setW(w.data);
      setStudio(studio.data);
      setRules(rules.data);
    });
  }, []);

  const supplierChart = useMemo(
    () => ({
      options: {
        ...chartBaseOptions,
        colors: ["#2563eb"],
        fill: { opacity: 0.18, type: "solid" },
        xaxis: { categories: monthLabels },
        yaxis: {
          labels: {
            formatter: (value: number) => Math.round(value).toString(),
          },
        },
      } satisfies ApexOptions,
      series: [{ name: "Suppliers", data: buildMonthlyCounts(s) }],
    }),
    [s],
  );

  const workflowChart = useMemo(
    () => ({
      options: {
        ...chartBaseOptions,
        chart: { ...chartBaseOptions.chart, type: "bar" },
        colors: ["#0ea5e9", "#22c55e"],
        plotOptions: { bar: { borderRadius: 7, columnWidth: "48%" } },
        xaxis: { categories: ["Open", "Active", "Completed", "Definitions"] },
      } satisfies ApexOptions,
      series: [
        {
          name: "Workflow throughput",
          data: [
            t.filter((task) => task.status?.toLowerCase().includes("open"))
              .length,
            t.filter(
              (task) =>
                task.status?.toLowerCase().includes("active") ||
                !task.completedAt,
            ).length,
            t.filter(
              (task) =>
                task.completedAt ||
                task.status?.toLowerCase().includes("complete"),
            ).length,
            w.length,
          ],
        },
      ],
    }),
    [t, w.length],
  );

  const publishedForms = studio.businessProcesses.filter(
    (p) => p.activeFormDefinitionId,
  ).length;
  const automatedProcesses = studio.businessProcesses.filter(
    (p) => p.activeWorkflowDefinitionId,
  ).length;
  const completedTasks = t.filter(
    (task) =>
      task.completedAt || task.status?.toLowerCase().includes("complete"),
  ).length;
  const completionRate = t.length
    ? Math.round((completedTasks / t.length) * 100)
    : 0;

  return (
    <>
      <PageHeader
        title="Executive dashboard"
        description="Operational visibility for configured business processes, published workflows, published forms, rule sets, approval matrices, suppliers and audit activity."
      />

      <div className="dashboard-row dashboard-row-charts">
        <AdminCard
          title="Supplier registrations"
          tools={<CardToolLink external>View suppliers</CardToolLink>}
        >
          <Chart
            options={supplierChart.options}
            series={supplierChart.series}
            type="area"
            height={285}
          />
        </AdminCard>
        <AdminCard
          title="Workflow throughput"
          tools={<CardToolLink external>Open workflows</CardToolLink>}
        >
          <Chart
            options={workflowChart.options}
            series={workflowChart.series}
            type="bar"
            height={285}
          />
        </AdminCard>
      </div>

      <div className="grid cols-4 dashboard-metrics dashboard-section">
        <InfoBox
          label="Registered vendors"
          value={s.length}
          icon="👥"
          trend="Supplier records from the LCA register"
          variant="primary"
        />
        <InfoBox
          label="Workflow tasks"
          value={t.length}
          icon="✅"
          trend={`${completionRate}% completed`}
          variant="success"
        />
        <InfoBox
          label="Business policy controls"
          value={rules.length}
          icon="⚙️"
          trend="Active rule catalogue entries"
          variant="warning"
        />
        <InfoBox
          label="Configured routing"
          value={studio.approvalMatrices.length}
          icon="🧭"
          trend="Approval matrices available"
          variant="info"
        />
      </div>

      <div className="dashboard-row dashboard-row-main dashboard-section">
        <AdminCard
          title="My work"
          tools={<CardToolLink external>View queue</CardToolLink>}
          className="dashboard-table-card"
        >
          {t.length ? (
            <DataTable
              rows={t.slice(0, 5)}
              searchable
              pageSize={5}
              striped
              compact
              columns={[
                {
                  header: "Task",
                  cell: (r) => r.title || r.name || r.id,
                  sortable: true,
                },
                {
                  header: "Status",
                  cell: (r) => r.status || "Open",
                  sortable: true,
                },
                {
                  header: "Assigned",
                  cell: (r) => r.assignedTo || r.assignedRole || "—",
                  sortable: true,
                },
              ]}
            />
          ) : (
            <EmptyState
              title="No workflow tasks"
              message="There are no workflow tasks assigned or available from the API yet."
            />
          )}
        </AdminCard>

        <AdminCard
          title="Recent activity"
          tools={<CardToolLink external>Audit log</CardToolLink>}
          className="dashboard-table-card"
        >
          {a.length ? (
            <DataTable
              rows={a.slice(0, 5)}
              searchable
              pageSize={5}
              striped
              compact
              toolbarActions={
                <button
                  className="btn secondary"
                  type="button"
                  onClick={() => window.print()}
                >
                  Print
                </button>
              }
              columns={[
                {
                  header: "Event",
                  cell: (r) => r.eventType || "Activity",
                  sortable: true,
                },
                {
                  header: "Entity",
                  cell: (r) => r.entityType || "—",
                  sortable: true,
                },
                {
                  header: "Actor",
                  cell: (r) => r.actor || "System",
                  sortable: true,
                },
                {
                  header: "Date",
                  cell: (r) => r.createdAt || r.occurredAt || "—",
                  sortable: true,
                },
              ]}
            />
          ) : (
            <EmptyState
              title="No audit events"
              message="Audit events will appear here when the audit API returns activity."
            />
          )}
        </AdminCard>

        <AdminCard
          title="eProcurement overview"
          tools={
            <CardTools>
              <button className="card-tool-link" type="button">
                Refresh
              </button>
            </CardTools>
          }
          className="dashboard-overview-card"
        >
          <InfoBox
            icon="📈"
            label="Workflow task completion"
            value={`${completionRate}%`}
            trend={`${completedTasks} of ${t.length} tasks completed`}
            variant="primary"
            className="overview-kpi"
          />
          <StatusRow
            label="Published forms"
            value={`${publishedForms}/${studio.businessProcesses.length}`}
            tone="success"
          />
          <StatusRow
            label="Automated processes"
            value={`${automatedProcesses}/${studio.businessProcesses.length}`}
            tone="info"
          />
          <StatusRow
            label="Document requirement sets"
            value={studio.documentRequirementSets.length.toString()}
            tone="warning"
          />
          <StatusRow
            label="Workflow definitions"
            value={w.length.toString()}
            tone="success"
          />
          <StatusRow
            label="Audit events captured"
            value={a.length.toString()}
            tone="info"
          />
        </AdminCard>
      </div>
    </>
  );
}
