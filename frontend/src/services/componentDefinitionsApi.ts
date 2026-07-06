import { apiDelete, apiGet, apiPost, apiPut } from "./apiClient";
import type { ComponentDefinition } from "../types/api";

export const getComponentDefinitions = () =>
  apiGet<ComponentDefinition[]>("/api/component-definitions", []);
export const createComponentDefinition = (body: ComponentDefinition) =>
  apiPost<ComponentDefinition>(
    "/api/component-definitions",
    body,
    {} as ComponentDefinition,
  );
export const updateComponentDefinition = (id: string, body: ComponentDefinition) =>
  apiPut<ComponentDefinition>(
    `/api/component-definitions/${id}`,
    body,
    {} as ComponentDefinition,
  );
export const deleteComponentDefinition = (id: string) =>
  apiDelete(`/api/component-definitions/${id}`);
