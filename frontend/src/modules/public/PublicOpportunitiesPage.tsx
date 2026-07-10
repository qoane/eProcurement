import { useEffect, useMemo, useState } from "react";
import { navigate } from "../../app/routes";
import { StatusBadge } from "../../components/ui/Badge";
import { Button } from "../../components/ui/Button";
import { Input } from "../../components/ui/Input";
import { Select } from "../../components/ui/Select";
import { useAuth } from "../../auth/AuthContext";
import {
  getPublicTender,
  getPublicTenderCalendar,
  getPublicTenderCategories,
  getPublicTenders,
  type PublicTenderCategory,
  type PublicTenderDetail,
  type PublicTenderDocument,
  type PublicTenderSummary,
} from "../../services/publicTendersApi";

export function referenceOf(tender: PublicTenderSummary) {
  return (
    tender.reference ||
    tender.referenceNumber ||
    tender.tenderNumber ||
    tender.id ||
    ""
  );
}
export function categoryOf(tender: PublicTenderSummary) {
  return tender.categoryName || tender.category || "Uncategorised";
}
export function formatDate(value?: string | null) {
  if (!value) return "Not specified";
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;
  return new Intl.DateTimeFormat(undefined, { dateStyle: "medium" }).format(
    date,
  );
}
export function isClosingSoon(value?: string | null) {
  if (!value) return false;
  const closing = new Date(value).getTime();
  if (Number.isNaN(closing)) return false;
  const now = Date.now();
  const sevenDays = 7 * 24 * 60 * 60 * 1000;
  return closing >= now && closing <= now + sevenDays;
}
function publicDocumentUrl(document: PublicTenderDocument) {
  return document.downloadUrl || document.publicUrl || document.url;
}

export function OpportunitiesPage() {
  const [tenders, setTenders] = useState<PublicTenderSummary[]>([]);
  const [categories, setCategories] = useState<PublicTenderCategory[]>([]);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState(() => new URLSearchParams(location.search).get("search") || "");
  const [tenderType, setTenderType] = useState("");
  const [category, setCategory] = useState("");
  const [status, setStatus] = useState("");
  const [closingSoon, setClosingSoon] = useState(false);

  useEffect(() => {
    Promise.all([
      getPublicTenders(),
      getPublicTenderCategories(),
      getPublicTenderCalendar(),
    ]).then(([tenderResult, categoryResult]) => {
      setTenders(tenderResult.data);
      setCategories(categoryResult.data);
      setError(tenderResult.error || categoryResult.error || "");
      setLoading(false);
    });
  }, []);

  const filtered = useMemo(() => {
    const q = search.trim().toLowerCase();
    return tenders.filter((tender) => {
      const haystack =
        `${tender.title} ${referenceOf(tender)} ${categoryOf(tender)} ${tender.description || ""}`.toLowerCase();
      return (
        (!q || haystack.includes(q)) &&
        (!tenderType || tender.tenderType === tenderType) &&
        (!category || categoryOf(tender) === category) &&
        (!status || tender.status === status) &&
        (!closingSoon || isClosingSoon(tender.closingDate))
      );
    });
  }, [category, closingSoon, search, status, tenderType, tenders]);

  const tenderTypes = Array.from(
    new Set(tenders.map((t) => t.tenderType).filter(Boolean)),
  );
  const statuses = Array.from(
    new Set(tenders.map((t) => t.status).filter(Boolean)),
  );
  const categoryNames = Array.from(
    new Set([
      ...categories.map((c) => c.name || c.title || c.code).filter(Boolean),
      ...tenders.map(categoryOf),
    ]),
  );

  return (
    <div className="public-portal">
      <section className="public-hero compact">
        <p className="landing-kicker">Open opportunities</p>
        <h1>Find public tenders from the Lesotho Communications Authority.</h1>
        <p>
          Search currently published opportunities. No sign-in is required to
          view notices and public documents.
        </p>
      </section>
      <section className="opportunity-search" aria-label="Opportunity filters">
        <Input
          placeholder="Search by title, reference, category, or description"
          value={search}
          onChange={(e) => setSearch(e.currentTarget.value)}
        />
        <Select
          value={tenderType}
          onChange={(e) => setTenderType(e.currentTarget.value)}
        >
          <option value="">All tender types</option>
          {tenderTypes.map((type) => (
            <option key={type} value={type}>
              {type}
            </option>
          ))}
        </Select>
        <Select
          value={category}
          onChange={(e) => setCategory(e.currentTarget.value)}
        >
          <option value="">All categories</option>
          {categoryNames.map((name) => (
            <option key={name} value={name}>
              {name}
            </option>
          ))}
        </Select>
        <Select
          value={status}
          onChange={(e) => setStatus(e.currentTarget.value)}
        >
          <option value="">All statuses</option>
          {statuses.map((s) => (
            <option key={s} value={s}>
              {s}
            </option>
          ))}
        </Select>
        <label className="check-row">
          <input
            type="checkbox"
            checked={closingSoon}
            onChange={(e) => setClosingSoon(e.currentTarget.checked)}
          />{" "}
          Closing soon
        </label>
      </section>
      {error && <p className="public-error">{error}</p>}
      {loading ? (
        <p className="public-empty">Loading public opportunities…</p>
      ) : (
        <section className="opportunity-list">
          {filtered.length === 0 ? (
            <p className="public-empty">
              No public opportunities match the selected filters.
            </p>
          ) : (
            filtered.map((tender) => (
              <article className="opportunity-card" key={referenceOf(tender)}>
                <div>
                  <p className="opportunity-reference">{referenceOf(tender)}</p>
                  <h2>{tender.title}</h2>
                  <p>
                    {categoryOf(tender)} · {tender.tenderType || "Tender"}
                  </p>
                </div>
                <div className="opportunity-meta">
                  <StatusBadge status={tender.status || "Open"} />
                  <span>Closes {formatDate(tender.closingDate)}</span>
                </div>
                <Button
                  variant="secondary"
                  onClick={() =>
                    navigate(
                      `/public/opportunities/${encodeURIComponent(referenceOf(tender))}`,
                    )
                  }
                >
                  View details
                </Button>
              </article>
            ))
          )}
        </section>
      )}
    </div>
  );
}

export function OpportunityDetailPage({ reference }: { reference: string }) {
  const auth = useAuth();
  const [tender, setTender] = useState<PublicTenderDetail | null>(null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);
  useEffect(() => {
    getPublicTender(reference).then((r) => {
      setTender(r.data);
      setError(r.error || "");
      setLoading(false);
    });
  }, [reference]);
  const documents = tender?.publicDocuments || tender?.documents || [];
  const clarifications =
    tender?.publicClarifications || tender?.clarifications || [];
  const submitBid = () => {
    const target =
      tender?.bidSubmissionUrl ||
      `/app/bids/new?tenderReference=${encodeURIComponent(reference)}`;
    const isSupplier =
      auth.currentUser &&
      (auth.currentUser.userType?.toLowerCase().includes("supplier") ||
        auth.roles.some((r) => r.toLowerCase().includes("supplier")) ||
        auth.currentUser.supplierId);
    navigate(
      isSupplier ? target : `/login?returnUrl=${encodeURIComponent(target)}`,
    );
  };
  return (
    <div className="public-portal">
      {loading ? (
        <p className="public-empty">Loading opportunity…</p>
      ) : !tender ? (
        <p className="public-empty">{error || "Opportunity not found."}</p>
      ) : (
        <>
          <section className="opportunity-detail-head">
            <p className="opportunity-reference">{referenceOf(tender)}</p>
            <h1>{tender.title}</h1>
            <div className="detail-actions">
              <StatusBadge status={tender.status || "Open"} />
              <Button onClick={submitBid}>Submit bid</Button>
              <Button
                variant="secondary"
                onClick={() => navigate("/public/register")}
              >
                Register as supplier
              </Button>
            </div>
          </section>
          <section className="detail-grid">
            <article className="detail-main">
              <h2>Description</h2>
              <p>
                {tender.description ||
                  "No public description has been provided."}
              </p>
              <h2>Public documents</h2>
              <div className="document-actions"><Button variant="secondary" onClick={() => { const first = documents.map(publicDocumentUrl).find(Boolean); if (first) location.href = first; }}>Download Documents</Button></div>
              {documents.length ? (
                documents.map((d) => {
                  const url = publicDocumentUrl(d);
                  return (
                    <p key={d.id || d.fileName || d.title}>
                      {url ? (
                        <a href={url}>
                          {d.fileName || d.name || d.title || "Document"}
                        </a>
                      ) : (
                        <span>
                          {d.fileName || d.name || d.title || "Document"}
                        </span>
                      )}{" "}
                      <small>{d.documentType || d.description}</small>
                    </p>
                  );
                })
              ) : (
                <p>No public documents are currently available.</p>
              )}
              <h2>Public clarifications</h2>
              {clarifications.length ? (
                clarifications.map((c) => (
                  <div className="clarification" key={c.id || c.question}>
                    <strong>{c.question}</strong>
                    <p>
                      {c.answer ||
                        c.response ||
                        c.responses?.[0]?.response ||
                        "Awaiting public response."}
                    </p>
                  </div>
                ))
              ) : (
                <p>No public clarifications have been published.</p>
              )}
            </article>
            <aside className="detail-panel">
              <dl>
                <dt>Reference number</dt>
                <dd>{referenceOf(tender)}</dd>
                <dt>Tender type</dt>
                <dd>{tender.tenderType || "Not specified"}</dd>
                <dt>Procurement method</dt>
                <dd>{tender.procurementMethod || "Not specified"}</dd>
                <dt>Category</dt>
                <dd>{categoryOf(tender)}</dd>
                <dt>Published date</dt>
                <dd>
                  {formatDate(
                    tender.publishedAt ||
                      tender.publishedDate ||
                      tender.publicationDate,
                  )}
                </dd>
                <dt>Closing date</dt>
                <dd>{formatDate(tender.closingDate)}</dd>
              </dl>
              <div className="supplier-cta">
                <h2>Interested in bidding?</h2>
                <p>
                  Register as a supplier or sign in before submitting a bid.
                </p>
                <Button
                  variant="secondary"
                  onClick={() => navigate("/public/register")}
                >
                  Supplier registration
                </Button>
              </div>
            </aside>
          </section>
        </>
      )}
    </div>
  );
}
