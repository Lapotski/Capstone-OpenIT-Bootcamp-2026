import { useState, useRef, useEffect } from 'react';
import { HiUser } from 'react-icons/hi';
import { useAuth } from '../context/AuthContext';

export default function Header({ activePage, onNavigate, darkMode, onToggleDark }) {
  const { user, login, register, logout } = useAuth();
  const [menuOpen, setMenuOpen] = useState(false);
  const [modal, setModal] = useState(null);
  const [form, setForm] = useState({ displayName: '', email: '', password: '', weeklyBudget: '' });
  const [error, setError] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const menuRef = useRef(null);

  useEffect(() => {
    const handler = (e) => {
      if (menuRef.current && !menuRef.current.contains(e.target)) setMenuOpen(false);
    };
    document.addEventListener('mousedown', handler);
    return () => document.removeEventListener('mousedown', handler);
  }, []);

  const openModal = (type) => { setModal(type); setError(''); setForm({ displayName: '', email: '', password: '', weeklyBudget: '' }); setMenuOpen(false); };
  const closeModal = () => setModal(null);
  const field = (key) => (e) => setForm((f) => ({ ...f, [key]: e.target.value }));

  const handleLogin = async (e) => {
    e.preventDefault();
    setSubmitting(true); setError('');
    try { await login(form.email, form.password); closeModal(); }
    catch (err) { setError(err.data?.error || 'Login failed. Check your credentials.'); }
    finally { setSubmitting(false); }
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    setSubmitting(true); setError('');
    try {
      await register(form.displayName, form.email, form.password);
      if (logout) {
          await logout(); 
        }
        
        openModal('login'); 
      }
      catch (err) { setError(err.data?.error || 'Registration failed.'); }
      finally { setSubmitting(false); }
    };

  return (
    <>
      <header className="site-header">
        <div className="header-container">
          <div className="header-logo">
            <span role="img" aria-label="logo" style={{ marginRight: 8 }}>🍽️</span>
            <strong>SaloSalo</strong>
          </div>
          <div className="header-actions">
            <nav className="header-nav" aria-label="Main navigation">
              <button type="button" className={`header-nav-link${activePage === 'recipe' ? ' active' : ''}`} onClick={() => onNavigate('recipe')}>Recipe</button>
              <button type="button" className={`header-nav-link${activePage === 'plans'  ? ' active' : ''}`} onClick={() => onNavigate('plans')}>Plans</button>
            </nav>

            {!user ? (
              <div className="header-auth-btns">
                <button type="button" className="auth-btn auth-btn--ghost" onClick={() => openModal('login')}>Log in</button>
                <button type="button" className="auth-btn auth-btn--solid" onClick={() => openModal('register')}>Sign up</button>
              </div>
            ) : (
              <div className="header-profile" ref={menuRef}>
                <button type="button" className="header-avatar" onClick={() => setMenuOpen((o) => !o)} aria-label="Profile menu" aria-haspopup="true" aria-expanded={menuOpen}>
                  <HiUser />
                </button>
                {menuOpen && (
                  <div className="avatar-dropdown">
                    <div className="avatar-dropdown-user">
                      <span className="avatar-name">{user.displayName || user.email}</span>
                    </div>
                    <div className="avatar-dropdown-divider" />
                    <button type="button" className="avatar-dropdown-item" onClick={() => { onToggleDark(); setMenuOpen(false); }}>
                      <span className="dropdown-icon">{darkMode ? '☀️' : '🌙'}</span>
                      {darkMode ? 'Light Mode' : 'Dark Mode'}
                    </button>
                    <div className="avatar-dropdown-divider" />
                    <button type="button" className="avatar-dropdown-item avatar-dropdown-logout" onClick={() => { logout(); setMenuOpen(false); }}>
                      <span className="dropdown-icon">🚪</span> Logout
                    </button>
                  </div>
                )}
              </div>
            )}
          </div>
        </div>
      </header>

      {/* ── Auth Modals ── */}
      {modal && (
        <div className="modal-overlay" onClick={closeModal}>
          <div className="modal-box" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h3>{modal === 'login' ? 'Log In' : 'Create Account'}</h3>
              <button type="button" className="modal-close" onClick={closeModal}>✕</button>
            </div>

            <form className="modal-body" onSubmit={modal === 'login' ? handleLogin : handleRegister}>
              {modal === 'register' && (
                <div className="modal-field">
                  <label>Display Name</label>
                  <input type="text" className="budget-input" value={form.displayName} onChange={field('displayName')} placeholder="Your name" required />
                </div>
              )}
              <div className="modal-field">
                <label>Email</label>
                <input type="email" className="budget-input" value={form.email} onChange={field('email')} placeholder="you@email.com" required />
              </div>
              <div className="modal-field">
                <label>Password</label>
                <input type="password" className="budget-input" value={form.password} onChange={field('password')} placeholder="••••••••" required minLength={6} />
              </div>
              {modal === 'register' && (
                <div className="modal-field">
                  <label>Weekly Budget (₱) <span style={{ opacity: 0.6, fontWeight: 400 }}>optional</span></label>
                  <input type="number" className="budget-input" value={form.weeklyBudget} onChange={field('weeklyBudget')} placeholder="0" min="0" />
                </div>
              )}
              {error && <p className="modal-error">{error}</p>}
              <div className="modal-footer">
                <button type="button" className="modal-cancel-btn" onClick={closeModal}>Cancel</button>
                <button type="submit" className="action-button" disabled={submitting}>{submitting ? 'Please wait…' : modal === 'login' ? 'Log In' : 'Create Account'}</button>
              </div>
              {modal === 'login' && (
                <p className="modal-switch">Don't have an account? <button type="button" className="link-btn" onClick={() => openModal('register')}>Sign up</button></p>
              )}
              {modal === 'register' && (
                <p className="modal-switch">Already have an account? <button type="button" className="link-btn" onClick={() => openModal('login')}>Log in</button></p>
              )}
            </form>
          </div>
        </div>
      )}
    </>
  );
}