import { apiGet } from "../../services/apiClient";
import type { PageDatasource } from "../../types/api";

export type DataSourceResult = {
  rows: Record<string, unknown>[];
  error?: string;
};

function asRows(data: unknown): Record<string, unknown>[] {
  if (Array.isArray(data)) return data as Record<string, unknown>[];
  if (data && typeof data === "object") {
    const value = data as Record<string, unknown>;
    if (Array.isArray(value.items))
      return value.items as Record<string, unknown>[];
    if (Array.isArray(value.data))
      return value.data as Record<string, unknown>[];
    if (Array.isArray(value.rows))
      return value.rows as Record<string, unknown>[];
  }
  return [];
}

function normalizeMode(mode?: string) {
  return (mode || "Metadata").toLowerCase();
}

function normalizeEntitySegment(entity: string) {
  return entity
    .trim()
    .replace(/([a-z0-9])([A-Z])/g, "$1-$2")
    .replace(/[^a-zA-Z0-9]+/g, "-")
    .replace(/(^-|-$)/g, "")
    .toLowerCase();
}

function generatedListEndpoint(datasource: PageDatasource) {
  if (datasource.endpoint) return datasource.endpoint;
  const entity = normalizeEntitySegment(datasource.entity || "");
  return entity ? `/api/entities/${entity}/records` : undefined;
}

function metadataListEndpoint(datasource: PageDatasource) {
  if (datasource.endpoint) return datasource.endpoint;
  const entity = normalizeEntitySegment(datasource.entity || "");
  return entity ? `/api/metadata/${entity}` : undefined;
}

function messageForMissingEndpoint(mode: string, entity: string) {
  if (!entity)
    return "No data source configured. Choose a datasource in Page Designer.";
  if (mode === "generatedcrud") {
    return `Generated CRUD datasource for ${entity} has no list endpoint. Select a generated datasource or configure /api/entities/{entity}/records.`;
  }
  if (mode === "customapi") {
    return `Custom API datasource for ${entity} has no endpoint. Configure the list endpoint in Page Designer before publishing.`;
  }
  return `Metadata datasource for ${entity} has no endpoint. Select metadata-backed entity definitions or configure a metadata endpoint.`;
}

export async function loadDataSource(
  datasource?: PageDatasource,
): Promise<DataSourceResult> {
  if (!datasource) {
    return {
      rows: [],
      error: "No data source configured. Choose a datasource in Page Designer.",
    };
  }

  const entity = datasource.entity?.trim() || "";
  const mode = normalizeMode(datasource.mode);
  const endpoint =
    mode === "generatedcrud"
      ? generatedListEndpoint(datasource)
      : mode === "customapi"
        ? datasource.endpoint
        : metadataListEndpoint(datasource);

  if (!endpoint) {
    return { rows: [], error: messageForMissingEndpoint(mode, entity) };
  }

  const result = await apiGet<unknown>(endpoint, []);
  return {
    rows: asRows(result.data),
    error: result.error
      ? `Unable to load ${entity || "datasource"} from ${endpoint}: ${result.error}`
      : undefined,
  };
}
