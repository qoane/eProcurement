import { apiDelete, apiGet, apiPost, apiPut } from "./apiClient";
import type { PageDesigner } from "../types/api";

export const getPageDefinitions = () =>
  apiGet<PageDesigner[]>("/api/page-definitions", []);
export const createPageDefinition = (body: PageDesigner) =>
  apiPost<PageDesigner>("/api/page-definitions", body, body);
export const updatePageDefinition = (id: string, body: PageDesigner) =>
  apiPut<PageDesigner>(`/api/page-definitions/${id}`, body, body);
export const deletePageDefinition = (id: string) =>
  apiDelete(`/api/page-definitions/${id}`);

export const publishPageDefinition = (id: string) =>
  apiPost<PageDesigner>(`/api/page-definitions/${id}/publish`, {});
export const archivePageDefinition = (id: string) =>
  apiPost<PageDesigner>(`/api/page-definitions/${id}/archive`, {});
