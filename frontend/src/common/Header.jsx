import { HiUser } from "react-icons/hi";

export default function Header({ activePage, onNavigate }) {
  return (
    <header className="site-header">
      <div className="header-container">
        <div className="header-logo">
          <span role="img" aria-label="logo" style={{ marginRight: '8px' }}>🍽️</span>
          <strong>SaloSalo</strong>
        </div>
        <div className="header-actions">
          <nav className="header-nav" aria-label="Main navigation">
            <button
              type="button"
              className={`header-nav-link ${activePage === 'recipe' ? 'active' : ''}`}
              onClick={() => onNavigate('recipe')}
            >
              Recipe
            </button>
            <button
              type="button"
              className={`header-nav-link ${activePage === 'plans' ? 'active' : ''}`}
              onClick={() => onNavigate('plans')}
            >
              Plans
            </button>
          </nav>
          <div className="header-profile">
            <div className="header-avatar" style={{ fontSize: '24px' }}><HiUser /></div>
          </div>
        </div>
      </div>
    </header>
  );
}
