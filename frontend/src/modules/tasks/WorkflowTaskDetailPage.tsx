import { useEffect, useMemo, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { Panel } from "../../components/ui/Panel";
import { Button } from "../../components/ui/Button";
import { StatusBadge } from "../../components/ui/Badge";
import { Timeline } from "../../components/ui/Timeline";
import { executeTaskAction, getTaskDetail } from "../../services/tasksApi";

type SupplierDocument = {
  id: string;
  documentType: string;
  fileName: string;
  uploadedBy?: string;
  uploadedAt?: string;
};

type VerificationState = {
  opened: boolean;
  matchesType: boolean;
  legible: boolean;
  current: boolean;
};

const emptyVerification = (): VerificationState => ({
  opened: false,
  matchesType: false,
  legible: false,
  current: false,
});

export function WorkflowTaskDetailPage({ id }: { id?: string }) {
  const [detail, setDetail] = useState<any>(null);
  const [selectedDocumentId, setSelectedDocumentId] = useState<string>();
  const [documentChecks, setDocumentChecks] = useState<
    Record<string, VerificationState>
  >({});
  const [verificationConfirmed, setVerificationConfirmed] = useState(false);
  const [reviewNote, setReviewNote] = useState("");
  const load = () => {
    if (id) void getTaskDetail(id).then((r) => setDetail(r.data));
  };
  useEffect(load, [id]);
  const supplier = detail?.supplier?.supplier;
  const documents: SupplierDocument[] = detail?.supplier?.documents ?? [];
  const selectedDocument =
    documents.find((d) => d.id === selectedDocumentId) ?? documents[0];
  const reviewedCount = useMemo(
    () =>
      documents.filter((document) => {
        const check = documentChecks[document.id];
        return (
          check?.opened && check.matchesType && check.legible && check.current
        );
      }).length,
    [documentChecks, documents],
  );
  const documentsVerified =
    documents.length > 0 && reviewedCount === documents.length;
  const canAcceptDocuments = documentsVerified && verificationConfirmed;
  const setDocumentCheck = (
    documentId: string,
    field: keyof VerificationState,
    value: boolean,
  ) => {
    setDocumentChecks((current) => ({
      ...current,
      [documentId]: {
        ...(current[documentId] ?? emptyVerification()),
        [field]: value,
      },
    }));
  };
  const openDocumentForReview = (document: SupplierDocument) => {
    setSelectedDocumentId(document.id);
    setDocumentCheck(document.id, "opened", true);
  };
  const executeAction = async (actionCode: string) => {
    if (!canAcceptDocuments) return;
    await executeTaskAction(id!, actionCode);
    setVerificationConfirmed(false);
    setReviewNote("");
    load();
  };

  return (
    <>
      <PageHeader
        title={`Workflow task ${detail?.task?.nodeCode ?? id ?? ""}`}
        description="Verification workspace assembled from supplier, document, form, history, and workflow configuration."
      />
      <div className="grid cols-2">
        <Panel title="Supplier information">
          <p>
            <strong>{supplier?.legalName}</strong>
          </p>
          <p>{supplier?.referenceNumber}</p>
          <StatusBadge status={supplier?.status} />
        </Panel>
        <Panel title="Submitted documents">
          {documents.length === 0 ? (
            <p className="muted">No documents were submitted for this task.</p>
          ) : (
            documents.map((d) => {
              const check = documentChecks[d.id];
              const verified =
                check?.opened &&
                check.matchesType &&
                check.legible &&
                check.current;
              return (
                <div className="document-review-row" key={d.id}>
                  <div>
                    <strong>{d.documentType}</strong> — {d.fileName}
                    <br />
                    <small>
                      {verified
                        ? "Verification complete"
                        : "Awaiting reviewer verification"}
                    </small>
                  </div>
                  <Button
                    variant="secondary"
                    onClick={() => openDocumentForReview(d)}
                  >
                    {selectedDocument?.id === d.id ? "Reviewing" : "Review"}
                  </Button>
                </div>
              );
            })
          )}
        </Panel>
      </div>
      <Panel title="Document verification">
        {selectedDocument ? (
          <div className="document-verification-layout">
            <div
              className="document-preview-card"
              aria-label={`Preview for ${selectedDocument.fileName}`}
            >
              <div className="document-preview-icon">PDF</div>
              <h3>{selectedDocument.fileName}</h3>
              <p>
                <strong>Document type:</strong> {selectedDocument.documentType}
              </p>
              {selectedDocument.uploadedBy && (
                <p>
                  <strong>Uploaded by:</strong> {selectedDocument.uploadedBy}
                </p>
              )}
              {selectedDocument.uploadedAt && (
                <p>
                  <strong>Uploaded:</strong>{" "}
                  {new Date(selectedDocument.uploadedAt).toLocaleString()}
                </p>
              )}
              <p className="muted">
                Secure file preview placeholder. Use the checklist to record
                that the opened document has been reviewed against the supplier
                submission.
              </p>
            </div>
            <div className="document-checklist">
              <h3>Reviewer checklist</h3>
              <label>
                <input
                  type="checkbox"
                  checked={documentChecks[selectedDocument.id]?.opened ?? false}
                  onChange={(e) =>
                    setDocumentCheck(
                      selectedDocument.id,
                      "opened",
                      e.target.checked,
                    )
                  }
                />{" "}
                Document opened and reviewed
              </label>
              <label>
                <input
                  type="checkbox"
                  checked={
                    documentChecks[selectedDocument.id]?.matchesType ?? false
                  }
                  onChange={(e) =>
                    setDocumentCheck(
                      selectedDocument.id,
                      "matchesType",
                      e.target.checked,
                    )
                  }
                />{" "}
                File matches the required document type
              </label>
              <label>
                <input
                  type="checkbox"
                  checked={
                    documentChecks[selectedDocument.id]?.legible ?? false
                  }
                  onChange={(e) =>
                    setDocumentCheck(
                      selectedDocument.id,
                      "legible",
                      e.target.checked,
                    )
                  }
                />{" "}
                Document is readable and complete
              </label>
              <label>
                <input
                  type="checkbox"
                  checked={
                    documentChecks[selectedDocument.id]?.current ?? false
                  }
                  onChange={(e) =>
                    setDocumentCheck(
                      selectedDocument.id,
                      "current",
                      e.target.checked,
                    )
                  }
                />{" "}
                Dates, names, and identifiers are acceptable
              </label>
            </div>
          </div>
        ) : (
          <p className="muted">
            Select a submitted document to begin verification.
          </p>
        )}
      </Panel>
      <Panel title="Form submission data">
        {(detail?.supplier?.formSubmissions?.[0]?.values ?? []).map(
          (v: any) => (
            <p key={v.fieldCode}>
              <strong>{v.fieldCode}</strong>: {v.value}
            </p>
          ),
        )}
      </Panel>
      <Panel title="Available workflow actions">
        <div className="verification-summary">
          <strong>
            {reviewedCount} of {documents.length} documents verified.
          </strong>
          <label>
            <input
              type="checkbox"
              checked={verificationConfirmed}
              disabled={!documentsVerified}
              onChange={(e) => setVerificationConfirmed(e.target.checked)}
            />{" "}
            I confirm the submitted documents have been reviewed and can be
            accepted.
          </label>
          <textarea
            className="input"
            value={reviewNote}
            onChange={(e) => setReviewNote(e.target.value)}
            placeholder="Optional verification note"
            rows={3}
          />
        </div>
        {(detail?.availableActions ?? []).map((a: any) => (
          <Button
            key={a.actionCode}
            disabled={!canAcceptDocuments}
            onClick={() => executeAction(a.actionCode)}
          >
            {a.actionName}
          </Button>
        ))}
      </Panel>
      <Panel title="Workflow history">
        <Timeline
          items={(detail?.history ?? []).map((h: any) => ({
            title: h.eventType,
            meta: h.actor,
            description: `${h.nodeCode} · ${h.details}`,
          }))}
        />
      </Panel>
    </>
  );
}
