import { useEffect, useMemo, useState } from "react";
import { EmptyState } from "../../components/ui/EmptyState";
import { LoadingState } from "../../components/ui/LoadingState";
import { getComponentDefinitions } from "../../services/componentDefinitionsApi";
import { getPageDefinitions } from "../../services/pageDefinitionsApi";
import type { ComponentDefinition, PageDesigner } from "../../types/api";
import { PageRenderer } from "./PageRenderer";

export function UiCompositionPage({ route, pageCode }: { route: string; pageCode?: string }) {
  const [pages, setPages] = useState<PageDesigner[]>([]);
  const [components, setComponents] = useState<ComponentDefinition[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>();

  useEffect(() => {
    setLoading(true);
    void Promise.all([getPageDefinitions(), getComponentDefinitions()]).then(
      ([pageResult, componentResult]) => {
        setPages(pageResult.data);
        setComponents(componentResult.data);
        setError(pageResult.error || componentResult.error);
        setLoading(false);
      },
    );
  }, []);

  const page = useMemo(() => {
    const activePages = pages.filter((item) => item.status !== "Archived");
    const normalizedPageCode = pageCode?.toUpperCase();
    if (normalizedPageCode) {
      const byCode = activePages.find(
        (item) => item.code.toUpperCase() === normalizedPageCode,
      );
      if (byCode) return byCode;
    }
    return activePages.find((item) => {
      const configuredRoute = item.navigation?.route || item.route;
      return configuredRoute === route;
    });
  }, [pageCode, pages, route]);

  if (loading) return <LoadingState />;
  if (!page)
    return (
      <EmptyState
        title="Page metadata not found"
        message={
          error || `No active PageDefinition is configured for ${route}.`
        }
      />
    );
  return <PageRenderer page={page} componentDefinitions={components} />;
}
