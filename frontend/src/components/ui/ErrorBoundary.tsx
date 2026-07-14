import React from "react";
import { EmptyState } from "./EmptyState";
export class ErrorBoundary extends React.Component<React.PropsWithChildren, { hasError: boolean; reference: string }> {
  state = { hasError: false, reference: "" };
  static getDerivedStateFromError() { return { hasError: true, reference: crypto.randomUUID?.() ?? String(Date.now()) }; }
  componentDidCatch(error: unknown) { console.error("Frontend error", this.state.reference, error); }
  render() { return this.state.hasError ? <EmptyState title="Something went wrong." message={`Reference: ${this.state.reference}`} /> : this.props.children; }
}
