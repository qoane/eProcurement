import { createContext, useContext, useEffect, useMemo, useState } from "react";
import { apiGet, apiPost, setAccessToken } from "../services/apiClient";

type UserProfile = { id: string; email: string; fullName: string; phoneNumber?: string; userType: string; isActive: boolean; isExternalUser: boolean; supplierId?: string; createdAt: string; lastLoginAt?: string };
type AuthResponse = { accessToken: string; userProfile: UserProfile; roles: string[]; permissions: string[] };
type AuthContextValue = { currentUser?: UserProfile; roles: string[]; permissions: string[]; hasPermission: (code: string) => boolean; login: (email: string, password: string) => Promise<boolean>; logout: () => void };
const AuthContext = createContext<AuthContextValue | undefined>(undefined);
const tokenKey = "procuraflow.accessToken";
export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [state, setState] = useState<Omit<AuthResponse, "accessToken">>({ userProfile: undefined as unknown as UserProfile, roles: [], permissions: [] });
  useEffect(() => { const token = localStorage.getItem(tokenKey); if (!token) return; setAccessToken(token); apiGet<AuthResponse>("/api/auth/me", undefined as unknown as AuthResponse).then(r => { if (r.data) setState({ userProfile: r.data.userProfile, roles: r.data.roles, permissions: r.data.permissions }); }); }, []);
  const value = useMemo<AuthContextValue>(() => ({ currentUser: state.userProfile, roles: state.roles, permissions: state.permissions, hasPermission: (code) => state.permissions.includes(code), login: async (email, password) => { const r = await apiPost<AuthResponse>("/api/auth/login", { email, password }); if (!r.data || r.error) return false; localStorage.setItem(tokenKey, r.data.accessToken); setAccessToken(r.data.accessToken); setState({ userProfile: r.data.userProfile, roles: r.data.roles, permissions: r.data.permissions }); return true; }, logout: () => { localStorage.removeItem(tokenKey); setAccessToken(undefined); setState({ userProfile: undefined as unknown as UserProfile, roles: [], permissions: [] }); } }), [state]);
  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
export function useAuth() { const ctx = useContext(AuthContext); if (!ctx) throw new Error("useAuth must be used inside AuthProvider"); return ctx; }
export function parsePermissions(json?: string) { try { const value = JSON.parse(json || "[]"); return Array.isArray(value) ? value as string[] : []; } catch { return []; } }
