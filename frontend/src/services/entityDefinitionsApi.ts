import { apiDelete, apiGet, apiPost, apiPut } from "./apiClient";
import type { EntityDesigner } from "../types/api";

export const getEntityDefinitions = () => apiGet<EntityDesigner[]>("/api/entity-definitions", []);
export const createEntityDefinition = (body: EntityDesigner) => apiPost<EntityDesigner>("/api/entity-definitions", body, {} as EntityDesigner);
export const updateEntityDefinition = (id: string, body: EntityDesigner) => apiPut<EntityDesigner>(`/api/entity-definitions/${id}`, body, {} as EntityDesigner);
export const deleteEntityDefinition = (id: string) => apiDelete(`/api/entity-definitions/${id}`);
