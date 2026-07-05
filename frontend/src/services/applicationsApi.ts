import { apiDelete, apiGet, apiPost, apiPut } from "./apiClient";
import type { ApplicationDesigner } from "../types/api";

export const getApplications = () => apiGet<ApplicationDesigner[]>("/api/applications", []);
export const createApplication = (body: ApplicationDesigner) => apiPost<ApplicationDesigner>("/api/applications", body, {} as ApplicationDesigner);
export const updateApplication = (id: string, body: ApplicationDesigner) => apiPut<ApplicationDesigner>(`/api/applications/${id}`, body, {} as ApplicationDesigner);
export const publishApplication = (id: string) => apiPost<ApplicationDesigner>(`/api/applications/${id}/publish`, {}, {} as ApplicationDesigner);
export const archiveApplication = (id: string) => apiPost<ApplicationDesigner>(`/api/applications/${id}/archive`, {}, {} as ApplicationDesigner);
export const deleteApplication = (id: string) => apiDelete(`/api/applications/${id}`);
