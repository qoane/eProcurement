import { apiGet } from "../../services/apiClient";
import { getSuppliers } from "../../services/suppliersApi";
import type { PageDatasource } from "../../types/api";

export type DataSourceResult = {
  rows: Record<string, unknown>[];
  error?: string;
};

function asRows(data: unknown): Record<string, unknown>[] {
  if (Array.isArray(data)) return data as Record<string, unknown>[];
  if (data && typeof data === "object") {
    const value = data as Record<string, unknown>;
    if (Array.isArray(value.items)) return value.items as Record<string, unknown>[];
    if (Array.isArray(value.data)) return value.data as Record<string, unknown>[];
    if (Array.isArray(value.rows)) return value.rows as Record<string, unknown>[];
  }
  return [];
}

function endpointFor(datasource?: PageDatasource | string) {
  if (!datasource) return undefined;
  if (typeof datasource === "string") return undefined;
  return datasource.endpoint;
}

function entityFor(datasource?: PageDatasource | string) {
  if (!datasource) return "";
  return typeof datasource === "string" ? datasource : datasource.entity;
}

export async function loadDataSource(
  datasource?: PageDatasource | string,
): Promise<DataSourceResult> {
  const endpoint = endpointFor(datasource);
  if (endpoint) {
    const result = await apiGet<unknown>(endpoint, []);
    return { rows: asRows(result.data), error: result.error };
  }

  const entity = entityFor(datasource);
  switch (entity.toLowerCase()) {
    case "supplier":
    case "suppliers": {
      const result = await getSuppliers();
      return {
        rows: (result.data as unknown as Record<string, unknown>[]) || [],
        error: result.error,
      };
    }
    default:
      return {
        rows: [],
        error: entity
          ? `No data source registered for ${entity}. Select a generated or custom endpoint in the page designer.`
          : "No data source configured.",
      };
  }
}
