const bases = [
  import.meta.env.VITE_API_BASE_URL,
  "https://localhost:44313",
  "http://localhost:5000",
].filter(Boolean) as string[];
export type ApiResult<T> = { data: T; error?: string; loading: false };
export async function apiGet<T>(
  path: string,
  fallback: T,
): Promise<ApiResult<T>> {
  let last = "";
  for (const b of bases) {
    try {
      const r = await fetch(`${b}${path}`);
      if (r.ok) return { data: (await r.json()) as T, loading: false };
      last = `${r.status} ${r.statusText}`;
    } catch (e) {
      last = e instanceof Error ? e.message : String(e);
    }
  }
  return { data: fallback, error: last || "API unavailable", loading: false };
}
export async function apiPost<T>(
  path: string,
  body: unknown,
  fallback: T,
): Promise<ApiResult<T>> {
  let last = "";
  for (const b of bases) {
    try {
      const r = await fetch(`${b}${path}`, {
        method: "POST",
        headers: { "content-type": "application/json" },
        body: JSON.stringify(body),
      });
      if (r.ok) return { data: (await r.json()) as T, loading: false };
      last = `${r.status} ${r.statusText}`;
    } catch (e) {
      last = e instanceof Error ? e.message : String(e);
    }
  }
  return { data: fallback, error: last || "API unavailable", loading: false };
}
