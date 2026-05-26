import { useState } from 'react';
import './App.css';
import Header from './common/Header';
import Footer from './common/Footer';
import MealPlanner from './pages/MealPlanner';
import Plans from './pages/Plans';

function App() {
  const [activePage, setActivePage] = useState('recipe');
  const [plans, setPlans] = useState([]);
  const [darkMode, setDarkMode] = useState(false);
  const [weeklyBudget, setWeeklyBudget] = useState('');

  const handleNavigate = (pageName) => setActivePage(pageName);

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
    <div className={`app-container${darkMode ? ' dark' : ''}`}>
      <Header
        activePage={activePage}
        onNavigate={handleNavigate}
        darkMode={darkMode}
        onToggleDark={() => setDarkMode((d) => !d)}
      />
      <main className="main-content">
        {activePage === 'plans' ? (
          <Plans plans={plans} onRemove={handleRemoveFromPlan} weeklyBudget={weeklyBudget} />
        ) : (
          <MealPlanner
            onAddToPlan={handleAddToPlan}
            weeklyBudget={weeklyBudget}
            onBudgetChange={setWeeklyBudget}
          />
        )}
      </main>
      <Footer />
    </div>
  );
}

export default App;
