function toTitle(segment: string) {
  return segment
    .split("-")
    .filter(Boolean)
    .map((part) => part.charAt(0).toUpperCase() + part.slice(1))
    .join(" ");
}

export function getRouteBreadcrumbs(pathname = location.pathname) {
  const segments = pathname
    .replace(/^\/app\/?/, "")
    .split("/")
    .filter(Boolean);
  const trail = segments.length ? segments : ["dashboard"];

  return ["Home", ...trail.map(toTitle)];
}

export function Breadcrumb({
  segments,
  title,
}: {
  segments?: string[];
  title?: string;
}) {
  const crumbs = segments?.length ? segments : getRouteBreadcrumbs();
  const currentTitle = title || crumbs[crumbs.length - 1] || "Dashboard";

  return (
    <div className="breadcrumb-bar" aria-label="Breadcrumb">
      <div>
        <h1>{currentTitle}</h1>
        <ol className="breadcrumb-list">
          {crumbs.map((crumb, index) => (
            <li
              key={`${crumb}-${index}`}
              aria-current={index === crumbs.length - 1 ? "page" : undefined}
            >
              {crumb}
            </li>
          ))}
        </ol>
      </div>
    </div>
  );
}
