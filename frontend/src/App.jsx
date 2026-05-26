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
  const [activePage, setActivePage]       = useState('recipe');
  const [localPlans, setLocalPlans]       = useState([]);
  const [darkMode, setDarkMode]           = useState(false);
  const [weeklyBudget, setWeeklyBudget]   = useState('');

  // ── When the user adds a recipe to plan, attempt to persist via API ──────────
  const handleAddToPlan = async (recipe, day) => {
    setLocalPlans((prev) => {
      if (prev.some((p) => p.id === recipe.id && p.day === day)) return prev;
      return [...prev, { ...recipe, day }];
    });
  
    if (user) {
      try {
        const plans = await api.getPlans();
        let planId;
        if (plans.length > 0) {
          planId = plans[plans.length - 1].id;
        } else {
          const now = new Date();
          const monday = new Date(now);
          monday.setDate(now.getDate() - ((now.getDay() + 6) % 7));
          const created = await api.createPlan({
            name: `Week of ${monday.toLocaleDateString()}`,
            weekStart: monday.toISOString().slice(0, 10),
          });
          planId = created.id;
        }
  
        const DAYS_OF_WEEK = ['Monday','Tuesday','Wednesday','Thursday','Friday','Saturday','Sunday'];
        await api.addPlanItem(planId, {
          recipeId:  recipe.id,
          dayOfWeek: DAYS_OF_WEEK.indexOf(day),
          mealSlot:  recipe.category ?? 'Lunch',
          servings:  1,
        });
      } catch (err) {
        console.error('Add to plan failed:', err);
      }
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