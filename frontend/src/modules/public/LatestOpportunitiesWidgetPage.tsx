import { useEffect, useState } from "react";
import {
  getLatestPublicTenders,
  type PublicTenderSummary,
} from "../../services/publicTendersApi";

function referenceOf(tender: PublicTenderSummary) {
  return (
    tender.tenderNumber ||
    tender.reference ||
    tender.referenceNumber ||
    tender.id ||
    "Opportunity"
  );
}

function categoryOf(tender: PublicTenderSummary) {
  return tender.categoryName || tender.category || "General";
}

function publicUrlOf(tender: PublicTenderSummary) {
  if (tender.publicUrl) return tender.publicUrl;
  const reference = referenceOf(tender);
  return `/opportunities/${encodeURIComponent(reference)}`;
}

function formatDate(value?: string | null) {
  if (!value) return "Not specified";
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;
  return new Intl.DateTimeFormat(undefined, {
    day: "2-digit",
    month: "short",
    year: "numeric",
  }).format(date);
}

export function LatestOpportunitiesWidgetPage() {
  const [tenders, setTenders] = useState<PublicTenderSummary[]>([]);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getLatestPublicTenders(5).then((result) => {
      setTenders(result.data);
      setError(result.error || "");
      setLoading(false);
    });
  }, []);

  return (
    <main className="lca-widget" aria-label="Latest LCA procurement opportunities">
      <style>{`
        .lca-widget { box-sizing: border-box; width: 100%; max-width: 520px; margin: 0; padding: 14px; color: #132238; background: #fff; border: 1px solid #d9e2ec; border-radius: 14px; font-family: Arial, Helvetica, sans-serif; }
        .lca-widget * { box-sizing: border-box; }
        .lca-widget__header { display: flex; align-items: baseline; justify-content: space-between; gap: 12px; margin-bottom: 10px; }
        .lca-widget h1 { margin: 0; font-size: 1rem; line-height: 1.25; }
        .lca-widget__brand { color: #52606d; font-size: .72rem; font-weight: 700; letter-spacing: .08em; text-transform: uppercase; }
        .lca-widget__list { display: grid; gap: 8px; margin: 0; padding: 0; list-style: none; }
        .lca-widget__item { padding: 10px; border: 1px solid #edf2f7; border-radius: 10px; background: #f8fafc; }
        .lca-widget__link { display: inline-block; color: #0f4c81; font-size: .93rem; font-weight: 700; line-height: 1.25; text-decoration: none; }
        .lca-widget__link:hover { text-decoration: underline; }
        .lca-widget__number { margin: 0 0 4px; color: #627d98; font-size: .72rem; font-weight: 700; }
        .lca-widget__meta { display: flex; flex-wrap: wrap; gap: 6px; margin-top: 8px; color: #52606d; font-size: .78rem; }
        .lca-widget__status { color: #0b6b3a; font-weight: 700; }
        .lca-widget__footer { display: flex; justify-content: flex-end; margin-top: 12px; }
        .lca-widget__all { color: #0f4c81; font-size: .85rem; font-weight: 700; text-decoration: none; }
        .lca-widget__empty { margin: 10px 0; color: #52606d; font-size: .88rem; }
      `}</style>
      <div className="lca-widget__header">
        <h1>Latest opportunities</h1>
        <span className="lca-widget__brand">LCA</span>
      </div>
      {error && <p className="lca-widget__empty">{error}</p>}
      {loading ? (
        <p className="lca-widget__empty">Loading opportunities…</p>
      ) : tenders.length === 0 ? (
        <p className="lca-widget__empty">No open opportunities are currently published.</p>
      ) : (
        <ul className="lca-widget__list">
          {tenders.map((tender) => (
            <li className="lca-widget__item" key={referenceOf(tender)}>
              <p className="lca-widget__number">{referenceOf(tender)}</p>
              <a className="lca-widget__link" href={publicUrlOf(tender)} target="_blank" rel="noreferrer">
                {tender.title}
              </a>
              <div className="lca-widget__meta">
                <span>Closes {formatDate(tender.closingDate)}</span>
                <span>•</span>
                <span>{categoryOf(tender)}</span>
                <span>•</span>
                <span className="lca-widget__status">{tender.status || "Open"}</span>
              </div>
            </li>
          ))}
        </ul>
      )}
      <div className="lca-widget__footer">
        <a className="lca-widget__all" href="/opportunities" target="_blank" rel="noreferrer">
          View all opportunities
        </a>
      </div>
    </main>
  );
}
