import { navigate } from "../../app/routes";
import { apiDelete } from "../../services/apiClient";
import type { PageAction, PageDatasource } from "../../types/api";

type ExecutePageActionOptions = {
  datasource?: PageDatasource;
  onRefresh?: () => void;
  onSuccess?: (message: string) => void;
};

function read(row: Record<string, unknown> | undefined, field: string) {
  return field.split(".").reduce<unknown>((value, key) => {
    if (value && typeof value === "object")
      return (value as Record<string, unknown>)[key];
    return undefined;
  }, row);
}

function defaultPageRoute(datasource?: PageDatasource) {
  const entity = datasource?.entity?.trim();
  return entity ? `/app/${entity.toLowerCase()}` : undefined;
}

export function resolveRoute(
  route: string,
  row: Record<string, unknown> | undefined,
  datasource: PageDatasource | undefined,
) {
  const keyField = datasource?.keyField || "id";
  const keyValue = read(row, keyField) ?? read(row, "id");
  return route.replace(/\{([^}]+)\}/g, (_, token: string) => {
    const value =
      token === "entity"
        ? datasource?.entity?.toLowerCase()
        : token === "id"
          ? keyValue
          : (read(row, token) ?? (token === keyField ? keyValue : undefined));
    return encodeURIComponent(value == null ? "" : String(value));
  });
}

export async function executePageAction(
  action?: PageAction,
  row?: Record<string, unknown>,
  options: ExecutePageActionOptions = {},
) {
  if (!action) return;

  const isDelete = action.code.toLowerCase() === "delete";
  if (isDelete) {
    if (!action.confirmation?.trim()) {
      options.onSuccess?.(
        "Delete action blocked: confirmation text is required.",
      );
      return;
    }
    const confirmed = window.prompt(
      `${action.confirmation}\n\nType DELETE to confirm.`,
    );
    if (confirmed !== "DELETE") return;
  } else if (action.confirmation && !window.confirm(action.confirmation)) {
    return;
  }

  const afterAction = action.afterAction;
  const routeMode = afterAction?.routeMode;
  const target =
    routeMode === "DatasourceDefault"
      ? defaultPageRoute(options.datasource)
      : afterAction?.navigateTo || action.target;

  if (isDelete) {
    const deleteEndpoint = action.target || options.datasource?.deleteEndpoint;
    if (!deleteEndpoint) {
      options.onSuccess?.(
        "Delete action blocked: no datasource delete endpoint is configured.",
      );
      return;
    }
    const result = await apiDelete(
      resolveRoute(deleteEndpoint, row, options.datasource),
    );
    if (result.error) {
      options.onSuccess?.(`Unable to delete record: ${result.error}`);
      return;
    }
  }

  const shouldRefresh =
    afterAction?.refreshDatasource ||
    afterAction?.afterActionType === "Refresh";
  if (shouldRefresh) options.onRefresh?.();

  if (afterAction?.successMessage)
    options.onSuccess?.(afterAction.successMessage);

  if (
    target &&
    !isDelete &&
    (afterAction?.afterActionType === "Navigate" ||
      afterAction?.afterActionType === "Open" ||
      !afterAction)
  ) {
    navigate(resolveRoute(target, row, options.datasource));
  }
}
