import { useEffect, useState } from "react";
import { Bell, Building2, Mail, Menu, Search, UserCircle } from "lucide-react";
import { Input } from "../components/ui/Input";
import { useAuth } from "../auth/AuthContext";
import { navigate } from "../app/routes";
import { getMyNotifications, getUnreadCount, markAllNotificationsRead, markNotificationRead, type NotificationMessage } from "../services/notificationsApi";

function toggleSidebar() { window.dispatchEvent(new Event("procuraflow:toggle-sidebar")); }

export function TopBar() {
  const { currentUser, roles, logout } = useAuth();
  const [notificationsOpen, setNotificationsOpen] = useState(false);
  const [profileOpen, setProfileOpen] = useState(false);
  const [unread, setUnread] = useState(0);
  const [notifications, setNotifications] = useState<NotificationMessage[]>([]);
  const loadNotifications = () => { getUnreadCount().then((r) => setUnread(r.data.count)); getMyNotifications().then((r) => setNotifications(r.data.slice(0, 8))); };
  useEffect(() => { loadNotifications(); const id = setInterval(loadNotifications, 60000); return () => clearInterval(id); }, []);
  const openNotification = async (n: NotificationMessage) => { await markNotificationRead(n.id); loadNotifications(); if (n.relatedUrl) navigate(n.relatedUrl); };
  return (
    <header className="topbar app-topbar">
      <button className="topbar-icon-button" type="button" aria-label="Toggle sidebar" onClick={toggleSidebar}><Menu size={21} /></button>
      <div className="search topbar-search"><Search size={16} /><Input aria-label="Global search" placeholder="Search suppliers, workflows, tasks, audit events…" /></div>
      <div className="topbar-actions position-relative" aria-label="Workspace actions">
        <button className="topbar-icon-button has-badge" type="button" aria-label="Messages"><Mail size={19} /><span className="topbar-badge">4</span></button>
        <button className="topbar-icon-button has-badge" type="button" aria-label="Notifications" onClick={() => { setNotificationsOpen((x) => !x); setProfileOpen(false); loadNotifications(); }}><Bell size={19} />{unread > 0 && <span className="topbar-badge warning">{unread}</span>}</button>
        {notificationsOpen && <div className="dropdown-menu show p-3 shadow" style={{ right: 260, top: 44, width: 380, maxHeight: 520, overflowY: "auto" }}><div className="d-flex justify-content-between align-items-center mb-2"><strong>Notifications</strong><button className="btn btn-sm btn-link" onClick={() => markAllNotificationsRead().then(loadNotifications)}>Mark all as read</button></div>{notifications.length === 0 && <p className="text-muted">No notifications.</p>}{notifications.map((n) => <button key={n.id} className={`dropdown-item text-start border-bottom py-2 ${n.status === "Unread" ? "fw-bold" : ""}`} onClick={() => openNotification(n)}><div className="d-flex justify-content-between"><span>{n.subject}</span><span className="badge text-bg-warning">{n.priority}</span></div><small className="text-muted d-block text-truncate">{n.body}</small><small>{new Date(n.createdAt).toLocaleString()} · {n.status}</small></button>)}<button className="btn btn-sm btn-primary w-100 mt-2" onClick={() => { setNotificationsOpen(false); navigate("/app/notifications"); }}>View all notifications</button></div>}
        <button className="topbar-user-menu border-0 bg-transparent" type="button" aria-label="Organization and user menu" onClick={() => { setProfileOpen((x) => !x); setNotificationsOpen(false); }}><Building2 size={18} /><span className="topbar-org">{currentUser ? `${currentUser.fullName} · ${roles[0] || currentUser.userType}` : "Lesotho Communications Authority"}</span><UserCircle size={24} /></button>
        {profileOpen && <div className="dropdown-menu show p-3 shadow" style={{ right: 0, top: 44, width: 300 }}><strong>{currentUser?.fullName || "Guest"}</strong><p className="mb-1 text-muted">{currentUser?.email}</p><p className="mb-1">User type: {currentUser?.userType}</p><p className="small text-muted">Roles: {roles.join(", ") || "None"}</p><button className="dropdown-item" onClick={() => navigate("/app/profile")}>My profile</button><button className="dropdown-item" onClick={() => navigate("/app/notification-preferences")}>Notification preferences</button><button className="dropdown-item" onClick={() => navigate("/app/settings")}>Settings</button><button className="dropdown-item text-danger" onClick={() => { logout(); navigate("/login"); }}>Logout</button></div>}
      </div>
    </header>
  );
}
