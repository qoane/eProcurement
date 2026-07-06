import { apiGet, apiPost } from "./apiClient";
import type { FormDefinition } from "../types/api";
export const getForms = () =>
  apiGet<FormDefinition[]>("/api/form-definitions", []);
export const getActiveForm = (c: string) =>
  apiGet(`/api/form-definitions/${c}/active`, null);
export const createForm = (b: unknown) =>
  apiPost("/api/form-definitions", b, null);

export const publishForm = (code: string, actor: string) =>
  apiPost(`/api/form-definitions/${code}/publish`, { actor }, null);
