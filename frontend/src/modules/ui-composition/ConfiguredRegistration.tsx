import { useEffect, useMemo, useState } from "react";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { EmptyState } from "../../components/ui/EmptyState";
import { Input } from "../../components/ui/Input";
import { LoadingState } from "../../components/ui/LoadingState";
import {
  getSupplierRegistrationConfiguration,
  registerSupplier,
} from "../../services/suppliersApi";
import type {
  ApprovalMatrix,
  BusinessProcessDefinition,
  DocumentRequirementSet,
  FormDefinition,
  WorkflowDefinition,
} from "../../types/api";

type RegistrationConfiguration = {
  process?: BusinessProcessDefinition;
  form?: FormDefinition;
  documentRequirements?: DocumentRequirementSet;
  approvalMatrix?: ApprovalMatrix | null;
  workflow?: WorkflowDefinition;
};

type RegistrationComponentConfiguration = {
  configurationEndpoint?: string;
  submitEndpoint?: string;
  referencePrefix?: string;
  actor?: string;
};

function asConfiguration(value: unknown): RegistrationComponentConfiguration {
  return value && typeof value === "object"
    ? (value as RegistrationComponentConfiguration)
    : {};
}

function publishedSections(form?: FormDefinition) {
  const active = form?.versions?.find((version) => version.status === "Published");
  return [...(active?.sections ?? [])].sort(
    (a, b) => (a.displayOrder ?? 0) - (b.displayOrder ?? 0),
  );
}

function inputType(fieldType: string) {
  if (fieldType === "email") return "email";
  if (["number", "money", "percentage"].includes(fieldType)) return "number";
  if (fieldType === "date") return "date";
  return "text";
}

export function ConfiguredRegistration({
  configuration,
}: {
  configuration?: unknown;
}) {
  const options = asConfiguration(configuration);
  const [metadata, setMetadata] = useState<RegistrationConfiguration>();
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState<string>();
  const [error, setError] = useState<string>();

  useEffect(() => {
    setLoading(true);
    void getSupplierRegistrationConfiguration().then((result) => {
      setMetadata((result.data as RegistrationConfiguration | null) ?? undefined);
      setError(result.error);
      setLoading(false);
    });
  }, [options.configurationEndpoint]);

  const sections = useMemo(() => publishedSections(metadata?.form), [metadata]);
  const requirements = metadata?.documentRequirements?.requirements ?? [];
  const workflow = metadata?.workflow?.versions?.find(
    (version) => version.id === metadata.workflow?.publishedVersionId,
  ) ?? metadata?.workflow?.versions?.find((version) => version.status === "Published");
  const generatedReference = `${options.referencePrefix ?? "SUP-LCA"}-${new Date().getFullYear()}-${Date.now().toString().slice(-4)}`;

  if (loading) return <LoadingState />;
  if (!metadata?.process || !metadata.form || sections.length === 0)
    return (
      <EmptyState
        title="Registration configuration is incomplete"
        message={
          error ||
          "Publish an Application, Business Process, Page Definition, Dynamic Form, Workflow, Business Rules, Approval Matrix, Document Types, Navigation item, and UI composition metadata."
        }
      />
    );

  return (
    <div className="grid cols-2">
      <form
        className="panel form-grid"
        onSubmit={async (event) => {
          event.preventDefault();
          const fd = new FormData(event.currentTarget);
          const fields = sections.flatMap((section) => section.fields ?? []);
          const values = Object.fromEntries(
            fields.map((field) => [field.code, String(fd.get(field.code) ?? "")]),
          );
          const referenceNumber = String(
            fd.get("referenceNumber") || generatedReference,
          );
          const documents = requirements.map((requirement) => ({
            documentType: requirement.documentType,
            fileName: String(
              fd.get(`doc_${requirement.documentType}`) ||
                `${requirement.documentType}.pdf`,
            ),
          }));
          const result = await registerSupplier({
            referenceNumber,
            actor: options.actor ?? "supplier@demo.co.ls",
            values,
            documents,
          });
          const workflowInstanceId = (
            result.data as { workflowInstanceId?: string } | null
          )?.workflowInstanceId;
          setMessage(
            workflowInstanceId
              ? `Submitted by configuration. Workflow instance: ${workflowInstanceId}`
              : result.error || "Registration could not be submitted.",
          );
        }}
      >
        <label>
          Supplier reference *
          <Input name="referenceNumber" defaultValue={generatedReference} />
        </label>
        {sections.map((section) => (
          <fieldset className="registration-section" key={section.code}>
            <legend>{section.title}</legend>
            {[...(section.fields ?? [])]
              .sort((a, b) => (a.displayOrder ?? 0) - (b.displayOrder ?? 0))
              .map((field) => (
                <label key={field.code}>
                  {field.label}
                  {field.isRequired ? " *" : ""}
                  <Input
                    required={field.isRequired}
                    type={inputType(field.fieldType)}
                    name={field.code}
                    placeholder={
                      field.visibilityRules?.[0]?.expression
                        ? `Visible when ${field.visibilityRules[0].expression}`
                        : undefined
                    }
                  />
                  {field.validations?.[0]?.message && (
                    <small>{field.validations[0].message}</small>
                  )}
                </label>
              ))}
          </fieldset>
        ))}
        {requirements.map((requirement) => (
          <label key={`upload-${requirement.documentType}`}>
            {requirement.documentType} file metadata
            {requirement.required ? " *" : ""}
            <Input
              name={`doc_${requirement.documentType}`}
              defaultValue={`${requirement.documentType}.pdf`}
            />
          </label>
        ))}
        {message && <p className="success">{message}</p>}
        <Button>Submit through configured process</Button>
      </form>
      <div className="stack">
        <Card>
          <h2>{metadata.process.name}</h2>
          <p>{metadata.process.description}</p>
          <p>
            <strong>Process:</strong> {metadata.process.code} · {metadata.process.status}
          </p>
        </Card>
        <Card>
          <h2>Workflow and approval matrix</h2>
          <p>
            <strong>Workflow:</strong> {metadata.workflow?.name} (
            {workflow?.nodes?.length ?? 0} nodes, {workflow?.transitions?.length ?? 0} transitions)
          </p>
          {(metadata.approvalMatrix?.steps ?? []).map((step) => (
            <p key={`${step.sequence}-${step.role}`}>
              Step {step.sequence}: {step.role}
              {step.ruleCode ? ` · rule ${step.ruleCode}` : ""}
            </p>
          ))}
        </Card>
        <Card>
          <h2>Document types</h2>
          {requirements.map((requirement) => (
            <p key={requirement.documentType}>
              <strong>{requirement.documentType}</strong>: {requirement.minimumFiles}-
              {requirement.maximumFiles} file(s), {requirement.allowedExtensions}, max {" "}
              {Math.round(requirement.maximumFileSize / 1048576)}MB
              {requirement.ruleCode ? ` · rule ${requirement.ruleCode}` : ""}
            </p>
          ))}
        </Card>
      </div>
    </div>
  );
}
