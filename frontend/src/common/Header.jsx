export default function Header() {
  return (
    <header className="site-header">
      <div className="header-container">
        <div className="header-logo">
          <span role="img" aria-label="logo" style={{ marginRight: '8px' }}>🍴</span>
          <strong>Meal Planner</strong>
        </div>
        <div className="header-profile">
          <div className="header-avatar" style={{fontSize: '24px'}}>👤</div>
        </div>
      </div>
    </header>
  );
}