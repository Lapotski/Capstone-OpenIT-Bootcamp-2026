import { useState } from 'react';
import './App.css';
import Header from './common/Header';
import Footer from './common/Footer';
import MealPlanner from './pages/MealPlanner';
import Plans from './pages/Plans';

function App() {
  const [activePage, setActivePage] = useState('recipe');
  const [plans, setPlans] = useState([]);

  const handleNavigate = (pageName) => {
    setActivePage(pageName);
  };

  const handleAddToPlan = (meal, day) => {
    setPlans((currentPlans) => {
      const alreadyPlanned = currentPlans.some(
        (item) => item.id === meal.id && item.day === day
      );
      if (alreadyPlanned) return currentPlans;
      return [...currentPlans, { ...meal, day }];
    });
  };

  const handleRemoveFromPlan = (id, day) => {
    setPlans((currentPlans) => currentPlans.filter((item) => item.id !== id || item.day !== day));
  };

  return (
    <div className="app-container">
      <Header activePage={activePage} onNavigate={handleNavigate} />
      <main className="main-content">
        {activePage === 'plans' ? (
          <Plans plans={plans} onRemove={handleRemoveFromPlan} />
        ) : (
          <MealPlanner onAddToPlan={handleAddToPlan} />
        )}
      </main>
      <Footer />
    </div>
  );
}

export default App;