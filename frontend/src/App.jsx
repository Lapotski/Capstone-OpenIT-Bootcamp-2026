import { useState } from 'react';
import './App.css';
import { AuthProvider, useAuth } from './context/AuthContext';
import { api } from './services/Api';
import Header from './common/Header';
import Footer from './common/Footer';
import MealPlanner from './pages/MealPlanner';
import Plans from './pages/Plans';

function AppContent() {
  const { user } = useAuth();
  const [activePage, setActivePage] = useState('recipe');
  const [localPlans, setLocalPlans] = useState([]);
  const [darkMode, setDarkMode] = useState(false);
  const [weeklyBudget, setWeeklyBudget] = useState('');

  const handleAddToPlan = async (recipe, day) => {
    setLocalPlans((prev) => {
      if (prev.some((p) => p.id === recipe.id && p.day === day)) return prev;
      return [...prev, { ...recipe, day }];
    });

    if (user) {
      try {
        const plans = await api.getPlans();
        if (plans.length > 0) {
          const latestPlan = plans[plans.length - 1];
          await api.addPlanItem(latestPlan.id, {
            recipeId:  recipe.id,
            dayOfWeek: day,
            mealSlot:  recipe.category ?? 'Lunch',
          });
        }
      } catch {}
    }
  };

  const handleRemoveFromPlan = (id, day) => {
    setLocalPlans((prev) => prev.filter((p) => !(p.id === id && p.day === day)));
  };

  return (
    <div className={`app-container${darkMode ? ' dark' : ''}`}>
      <Header
        activePage={activePage}
        onNavigate={setActivePage}
        darkMode={darkMode}
        onToggleDark={() => setDarkMode((d) => !d)}
      />
      <main className="main-content">
        {activePage === 'plans' ? (
          <Plans plans={localPlans} onRemove={handleRemoveFromPlan} weeklyBudget={weeklyBudget} />
        ) : (
          <MealPlanner onAddToPlan={handleAddToPlan} weeklyBudget={weeklyBudget} onBudgetChange={setWeeklyBudget} />
        )}
      </main>
      <Footer />
    </div>
  );
}

export default function App() {
  return (
    <AuthProvider>
      <AppContent />
    </AuthProvider>
  );
}