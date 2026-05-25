import './App.css';
import Header from './common/Header';
import Footer from './common/Footer';
import MealPlanner from './pages/MealPlanner';

function App() {
  return (
    <div className="app-container">
      <Header />
      <main className="main-content">
        <MealPlanner />
      </main>
      <Footer />
    </div>
  );
}

export default App;