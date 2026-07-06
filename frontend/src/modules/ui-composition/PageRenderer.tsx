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
import { getActiveForm } from "../../services/formsApi";
import { apiPost, apiPut } from "../../services/apiClient";
import type {
  ComponentDefinition,
  PageAction,
  PageColumn,
  PageComponent,
  PageDesigner,
  FormDefinition,
  FormSection,
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

function configOf(component: PageComponent) {
  return (component.configuration || {}) as Record<string, string>;
}

function activeSections(definition: FormDefinition | null): FormSection[] {
  return (
    definition?.versions?.find(
      (version) =>
        version.status === "Published" || version.status === "Active",
    )?.sections ||
    definition?.versions?.[0]?.sections ||
    []
  );
}

function DynamicFormRegion({
  component,
  page,
}: {
  component: PageComponent;
  page: PageDesigner;
}) {
  const configuration = configOf(component);
  const formCode = configuration.formCode || configuration.formId || "";
  const [definition, setDefinition] = useState<FormDefinition | null>(null);
  const [values, setValues] = useState<Record<string, string>>({});
  const [message, setMessage] = useState<string>();

  useEffect(() => {
    if (!formCode) {
      setDefinition(null);
      return;
    }
    void getActiveForm(formCode).then((result) => {
      if (result.error) setMessage(result.error);
      setDefinition(result.data as FormDefinition | null);
    });
  }, [formCode]);

  async function submit() {
    const endpoint =
      configuration.createEndpoint ||
      page.datasource.createEndpoint ||
      page.datasource.endpoint;
    if (!endpoint) {
      setMessage("No create endpoint configured for this form region.");
      return;
    }
    const result = endpoint.includes("{id}")
      ? await apiPut(endpoint.replace("/{id}", ""), values, values)
      : await apiPost(endpoint, values, values);
    setMessage(result.error || "Form submitted successfully.");
  }

  if (!formCode)
    return (
      <EmptyState
        title={component.name}
        message="Select a Dynamic Form definition in Page Designer to render data-entry fields here."
      />
    );

  const sections = activeSections(definition);
  return (
    <section className="panel">
      <h3>{configuration.formName || definition?.name || component.name}</h3>
      {message && (
        <p className={message.includes("success") ? "success" : "muted"}>
          {message}
        </p>
      )}
      {sections.length === 0 ? (
        <EmptyState
          title="Form definition unavailable"
          message={`The configured form ${formCode} has no active fields yet.`}
        />
      ) : (
        <>
          {sections.map((section) => (
            <fieldset key={section.code} className="panel">
              <legend>{section.title}</legend>
              <div className="form-grid">
                {(section.fields || [])
                  .sort((a, b) => (a.displayOrder || 0) - (b.displayOrder || 0))
                  .map((field) => (
                    <FieldRenderer
                      key={field.code}
                      label={field.label}
                      required={field.isRequired}
                      value={values[field.code] || ""}
                      onChange={(value) =>
                        setValues((current) => ({
                          ...current,
                          [field.code]: value,
                        }))
                      }
                    />
                  ))}
              </div>
            </fieldset>
          ))}
          <Button onClick={() => void submit()}>Submit</Button>
        </>
      )}
    </section>
  );
}
function rowActions(actions: PageAction[]) {
  return actions.filter(
    (action) => action.kind === "Row" || action.target?.includes("{"),
  );
}

function renderComponent(
  component: PageComponent,
  rows: Record<string, unknown>[],
  page: PageDesigner,
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
    return <DynamicFormRegion component={component} page={page} />;
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
    setError(undefined);
    void loadDataSource(page.datasource)
      .then((result) => {
        setRows(result.rows);
        setError(result.error);
      })
      .catch((exception: unknown) => {
        setRows([]);
        setError(
          exception instanceof Error
            ? exception.message
            : "Unexpected datasource loading error.",
        );
      })
      .finally(() => setLoading(false));
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
  const rowActionItems = rowActions(page.actions);

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
              void executePageAction(action, undefined, {
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
      {error && (
        <EmptyState
          title="Datasource could not be loaded"
          message={`${error} Check the Page Designer datasource mode, endpoint, and key field before publishing.`}
        />
      )}
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
                ...(rowActionItems.length
                  ? [
                      {
                        header: "Actions",
                        cell: (row: Record<string, unknown>) => (
                          <div className="actions">
                            {rowActionItems.map((action) => (
                              <Button
                                key={action.code}
                                variant={
                                  action.code.toLowerCase() === "delete"
                                    ? "secondary"
                                    : "primary"
                                }
                                onClick={() =>
                                  void executePageAction(action, row, {
                                    datasource: page.datasource,
                                    onRefresh: refreshDatasource,
                                    onSuccess: setActionMessage,
                                  })
                                }
                              >
                                {action.label}
                              </Button>
                            ))}
                          </div>
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
            <div key={component.code}>
              {renderComponent(component, rows, page)}
            </div>
          ))}
        </div>
      )}
      {page.pageType === "Form" &&
        activeComponents.map((component) => (
          <div key={component.code}>
            {renderComponent(component, rows, page)}
          </div>
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
