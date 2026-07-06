import { useEffect, useMemo, useState } from "react";
import { EmptyState } from "../../components/ui/EmptyState";
import { LoadingState } from "../../components/ui/LoadingState";
import { getComponentDefinitions } from "../../services/componentDefinitionsApi";
import { getPageDefinitions } from "../../services/pageDefinitionsApi";
import type { ComponentDefinition, PageDesigner } from "../../types/api";
import { PageRenderer } from "./PageRenderer";

export function UiCompositionPage({ route }: { route: string }) {
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

  const page = useMemo(
    () =>
      pages.find(
        (item) =>
          item.navigation?.route === route && item.status !== "Archived",
      ),
    [pages, route],
  );

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
