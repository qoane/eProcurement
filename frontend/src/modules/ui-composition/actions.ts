import { navigate } from "../../app/routes";
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

function resolveRoute(
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

export function executePageAction(
  action?: PageAction,
  row?: Record<string, unknown>,
  options: ExecutePageActionOptions = {},
) {
  if (action?.confirmation && !window.confirm(action.confirmation)) return;

  const afterAction = action?.afterAction;
  const shouldRefresh =
    afterAction?.refreshDatasource ||
    afterAction?.afterActionType === "Refresh";
  if (shouldRefresh) options.onRefresh?.();

  if (afterAction?.successMessage)
    options.onSuccess?.(afterAction.successMessage);

  const routeMode = afterAction?.routeMode;
  const target =
    routeMode === "DatasourceDefault"
      ? defaultPageRoute(options.datasource)
      : afterAction?.navigateTo || action?.target;

  if (
    target &&
    (afterAction?.afterActionType === "Navigate" ||
      afterAction?.afterActionType === "Open" ||
      !afterAction)
  ) {
    navigate(resolveRoute(target, row, options.datasource));
  }
}
