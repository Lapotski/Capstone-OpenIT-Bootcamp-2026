import { useState, useRef, useEffect } from 'react';
import { HiUser } from 'react-icons/hi';

export default function Header({ activePage, onNavigate, darkMode, onToggleDark }) {
  const [menuOpen, setMenuOpen] = useState(false);
  const menuRef = useRef(null);

  useEffect(() => {
    function handleClickOutside(e) {
      if (menuRef.current && !menuRef.current.contains(e.target)) {
        setMenuOpen(false);
      }
    }
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  return (
    <header className="site-header">
      <div className="header-container">
        {/* Logo — left */}
        <div className="header-logo">
          <span role="img" aria-label="logo" style={{ marginRight: '8px' }}>🍽️</span>
          <strong>SaloSalo</strong>
        </div>

        {/* Actions (nav + avatar) — right */}
        <div className="header-actions">
          <nav className="header-nav" aria-label="Main navigation">
            <button
              type="button"
              className={`header-nav-link${activePage === 'recipe' ? ' active' : ''}`}
              onClick={() => onNavigate('recipe')}
            >
              Recipe
            </button>
            <button
              type="button"
              className={`header-nav-link${activePage === 'plans' ? ' active' : ''}`}
              onClick={() => onNavigate('plans')}
            >
              Plans
            </button>
          </nav>

          <div className="header-profile" ref={menuRef}>
            <button
              type="button"
              className="header-avatar"
              onClick={() => setMenuOpen((o) => !o)}
              aria-label="Open profile menu"
              aria-haspopup="true"
              aria-expanded={menuOpen}
            >
              <HiUser />
            </button>

            {menuOpen && (
              <div className="avatar-dropdown">
                <button
                  type="button"
                  className="avatar-dropdown-item"
                  onClick={() => { onToggleDark(); setMenuOpen(false); }}
                >
                  <span className="dropdown-icon">{darkMode ? '☀️' : '🌙'}</span>
                  {darkMode ? 'Light Mode' : 'Dark Mode'}
                </button>
                <div className="avatar-dropdown-divider" />
                <button
                  type="button"
                  className="avatar-dropdown-item avatar-dropdown-logout"
                  onClick={() => { setMenuOpen(false); alert('Logged out!'); }}
                >
                  <span className="dropdown-icon">🚪</span>
                  Logout
                </button>
              </div>
            )}
          </div>
        </div>
      </div>
    </header>
  );
}
