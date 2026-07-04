export function navigate(path: string) {
  history.pushState(null, "", path);
  window.dispatchEvent(new Event("popstate"));
}
export const path = () => location.pathname;
