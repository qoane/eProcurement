import { useEffect, useMemo, useState } from "react";
import { Button } from "../../components/ui/Button";
import { DataTable } from "../../components/ui/DataTable";
import { EmptyState } from "../../components/ui/EmptyState";
import { Input } from "../../components/ui/Input";
import { LoadingState } from "../../components/ui/LoadingState";
import { MetricCard } from "../../components/ui/MetricCard";
import { PageHeader } from "../../components/ui/PageHeader";
import { StatusBadge } from "../../components/ui/Badge";
import { FieldRenderer } from "../../components/forms/FieldRenderer";
import type {
  ComponentDefinition,
  PageAction,
  PageColumn,
  PageComponent,
  PageDesigner,
} from "../../types/api";
import { executePageAction } from "./actions";
import { loadDataSource } from "./dataSources";
import { ConfiguredRegistration } from "./ConfiguredRegistration";

function read(row: Record<string, unknown>, field: string) {
  return field.split(".").reduce<unknown>((value, key) => {
    if (value && typeof value === "object")
      return (value as Record<string, unknown>)[key];
    return undefined;
  }, row);
}

function formatCell(row: Record<string, unknown>, column: PageColumn) {
  const value = read(row, column.field);
  if (column.code.toLowerCase().includes("status"))
    return <StatusBadge status={String(value || "")} />;
  if (Array.isArray(value)) return value.length;
  return value == null || value === "" ? "—" : String(value);
}

function firstRowAction(actions: PageAction[]) {
  return actions.find(
    (action) => action.kind === "Row" || action.target?.includes("{"),
  );
}

function renderComponent(
  component: PageComponent,
  rows: Record<string, unknown>[],
) {
  if (component.componentType === "Metric")
    return (
      <MetricCard
        label={component.name}
        value={rows.length}
        meta="Loaded from metadata data source"
      />
    );
  if (component.componentType === "FormField")
    return <FieldRenderer label={component.name} required />;
  if (component.componentType === "ConfiguredRegistration")
    return <ConfiguredRegistration configuration={component.configuration} />;
  return null;
}

export function PageRenderer({
  page,
  componentDefinitions,
}: {
  page: PageDesigner;
  componentDefinitions: ComponentDefinition[];
}) {
  const [rows, setRows] = useState<Record<string, unknown>[]>([]);
  const [query, setQuery] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>();
  const [actionMessage, setActionMessage] = useState<string>();

  function refreshDatasource() {
    setLoading(true);
    void loadDataSource(page.datasource).then((result) => {
      setRows(result.rows);
      setError(result.error);
      setLoading(false);
    });
  }

  useEffect(() => {
    refreshDatasource();
  }, [page.datasource]);

  const searchable = useMemo(
    () =>
      page.columns
        .filter((column) => column.searchable)
        .map((column) => column.field),
    [page.columns],
  );
  const orderedColumns = useMemo(
    () =>
      [...page.columns].sort(
        (a, b) => (a.displayOrder || 0) - (b.displayOrder || 0),
      ),
    [page.columns],
  );
  const filtered = useMemo(() => {
    const term = query.toLowerCase();
    if (!term) return rows;
    const fields = searchable.length
      ? searchable
      : page.columns.map((column) => column.field);
    return rows.filter((row) =>
      fields.some((field) =>
        String(read(row, field) || "")
          .toLowerCase()
          .includes(term),
      ),
    );
  }, [page.columns, query, rows, searchable]);

  const allowed = page.permissions.length > 0;
  const activeComponents = page.components.filter((component) =>
    componentDefinitions.some(
      (definition) =>
        definition.rendererKey === component.componentType ||
        definition.code === component.componentType.toUpperCase(),
    ),
  );
  const headerActions = page.actions.filter((action) => action.kind !== "Row");
  const rowAction = firstRowAction(page.actions);

  if (loading) return <LoadingState />;
  if (!allowed)
    return (
      <EmptyState
        title="Access not configured"
        message="This PageDefinition has no permission metadata."
      />
    );

  return (
    <>
      <PageHeader
        title={page.name}
        description={page.description}
        actions={headerActions.map((action) => (
          <Button
            key={action.code}
            variant={action.kind === "Secondary" ? "secondary" : "primary"}
            onClick={() =>
              executePageAction(action, undefined, {
                datasource: page.datasource,
                onRefresh: refreshDatasource,
                onSuccess: setActionMessage,
              })
            }
          >
            {action.label}
          </Button>
        ))}
      />
      {actionMessage && (
        <div className="designer-message">
          <strong>{actionMessage}</strong>
        </div>
      )}
      {error && <EmptyState title="Data source warning" message={error} />}
      {page.pageType === "DataGrid" && (
        <>
          <Input
            aria-label={`Search ${page.name}`}
            placeholder="Search…"
            value={query}
            onChange={(event) => setQuery(event.target.value)}
          />
          <section className="panel" style={{ marginTop: 16 }}>
            <DataTable
              rows={filtered}
              columns={[
                ...orderedColumns.map((column) => ({
                  header: column.label,
                  cell: (row: Record<string, unknown>) =>
                    formatCell(row, column),
                })),
                ...(rowAction
                  ? [
                      {
                        header: "Action",
                        cell: (row: Record<string, unknown>) => (
                          <Button
                            variant="secondary"
                            onClick={() =>
                              executePageAction(rowAction, row, {
                                datasource: page.datasource,
                                onRefresh: refreshDatasource,
                                onSuccess: setActionMessage,
                              })
                            }
                          >
                            {rowAction.label}
                          </Button>
                        ),
                      },
                    ]
                  : []),
              ]}
            />
          </section>
        </>
      )}
      {page.pageType === "Dashboard" && (
        <div className="grid">
          {activeComponents.map((component) => (
            <div key={component.code}>{renderComponent(component, rows)}</div>
          ))}
        </div>
      )}
      {page.pageType === "Form" &&
        activeComponents.map((component) => (
          <div key={component.code}>{renderComponent(component, rows)}</div>
        ))}
      {page.pageType === "DetailPage" && (
        <EmptyState
          title={page.name}
          message="Detail page composition is ready for entity-specific metadata bindings."
        />
      )}
    </>
  );
}
