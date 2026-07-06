import { navigate } from "../../app/routes";
import type { PageAction } from "../../types/api";

export function executePageAction(
  action?: PageAction,
  row?: Record<string, unknown>,
) {
  if (!action?.target) return;
  const target = action.target.replace(/\{([^}]+)\}/g, (_, key: string) => {
    const value = row?.[key];
    return encodeURIComponent(value == null ? "" : String(value));
  });
  navigate(target);
}
