export default function Footer() {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="site-footer">
      <div className="footer-container">
        <p className="footer-text">&copy; {currentYear} SaloSalo. All rights reserved.</p>
      </div>
    </footer>
  );
}