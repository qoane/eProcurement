import { getSuppliers } from "../../services/suppliersApi";

export type DataSourceResult = {
  rows: Record<string, unknown>[];
  error?: string;
};

export async function loadDataSource(
  entity?: string,
): Promise<DataSourceResult> {
  switch ((entity || "").toLowerCase()) {
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
          ? `No data source registered for ${entity}.`
          : "No data source configured.",
      };
  }
}
