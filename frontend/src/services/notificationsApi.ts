import { apiDelete, apiGet, apiPost, apiPut } from "./apiClient";

export type NotificationMessage = { id: string; eventCode: string; channel: string; subject: string; body: string; priority: string; status: string; createdAt: string; relatedUrl?: string | null; recipients?: { status: string; readAt?: string | null }[] };
export type NotificationTemplate = { id?: string; code: string; name: string; description: string; eventCode: string; channel: string; subjectTemplate: string; bodyTemplate: string; isActive: boolean };
export type NotificationPreference = { eventCode: string; inAppEnabled: boolean; emailEnabled: boolean; smsEnabled: boolean };
export type Setting = { key: string; value: string; isSecret: boolean; category: string; isOverridden: boolean };
export type DeliveryLog = { id: string; notificationMessageId: string; channel: string; status: string; sentAt: string; error?: string | null; requestPayload: string; responsePayload: string };

export const getMyNotifications = () => apiGet<NotificationMessage[]>("/api/notifications/my", []);
export const getUnreadCount = () => apiGet<{ count: number }>("/api/notifications/my/unread-count", { count: 0 });
export const markNotificationRead = (id: string) => apiPost(`/api/notifications/${id}/mark-read`, {}, { success: true });
export const markAllNotificationsRead = () => apiPost("/api/notifications/mark-all-read", {}, { success: true });
export const getTemplates = () => apiGet<NotificationTemplate[]>("/api/notification-templates", []);
export const saveTemplate = (t: NotificationTemplate) => t.id ? apiPut(`/api/notification-templates/${t.id}`, t, t) : apiPost("/api/notification-templates", t, t);
export const testTemplate = (id: string, model: Record<string, string>) => apiPost<string>(`/api/notification-templates/${id}/test`, model, "");
export const getDeliveryLogs = () => apiGet<DeliveryLog[]>("/api/notification-delivery-logs", []);
export const getPreferences = () => apiGet<NotificationPreference[]>("/api/notification-preferences/my", []);
export const savePreferences = (p: NotificationPreference[]) => apiPut("/api/notification-preferences/my", p, p);
export const getSettings = () => apiGet<Setting[]>("/api/settings", []);
export const saveSetting = (s: Setting) => apiPut(`/api/settings/${encodeURIComponent(s.key)}`, s, s);
export const deleteSetting = (key: string) => apiDelete(`/api/settings/${encodeURIComponent(key)}`);
export const testEmailSettings = (to: string) => apiPost("/api/settings/test-email", { to }, { success: false, response: "" });
export const testSmsSettings = (phoneNumber: string) => apiPost("/api/settings/test-sms", { phoneNumber }, { success: false, response: "" });
