import { useState, useEffect } from 'react';
import { api } from '../services/Api';
import { useAuth } from '../context/AuthContext';
import RecipeCard from '../common/RecipeCard';
import AddRecipeModal from '../common/AddRecipe';

export default function MealPlanner({ onAddToPlan, weeklyBudget, onBudgetChange }) {
  const { user } = useAuth();

  const [recipes, setRecipes] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [activeFilter, setActiveFilter] = useState('all');
  const [selectedDays, setSelectedDays] = useState({});
  const [showModal, setShowModal] = useState(false);
  
  // Fetch recipes from backend database
  useEffect(() => {
    if (!user) return;

    setLoading(true);
    setError('');

    let params = {};
    if (activeFilter !== 'all') {
      params.category = activeFilter;
    }

    api.getRecipes(params)
      .then((data) => {
        setRecipes(data);
        setLoading(false);
      })
      .catch(() => {
        setError('Could not load recipes. Is the backend running?');
        setLoading(false);
      });
  }, [user, activeFilter]);

  // Client-side Budget calculation
  let budgetNum = weeklyBudget === '' ? Infinity : Number(weeklyBudget);
  const filteredRecipes = recipes.filter((r) => (r.costPerServing ?? 0) <= budgetNum);

  // Unauthenticated fallback UI
  if (!user) {
    return (
      <div className="planner-container">
        <div className="no-results" style={{ margin: '4rem auto', textAlign: 'center' }}>
          <p style={{ fontSize: '1.25rem', marginBottom: '0.5rem' }}>👋 Please <strong>log in</strong> or <strong>sign up</strong> to use the Meal Planner.</p>
          <p style={{ opacity: 0.6 }}>Your recipes and plans are tied to your account.</p>
        </div>
      </div>
    );
  }

  return (
    <div className="planner-container">
      <header className="planner-header">
        <h2>Daily Meal Planner</h2>
        <p>Set your budget and filter your structure effortlessly</p>
      </header>

      <div className="planner-content">
        
        {/* Controls Panel */}
        <div className="controls-panel">
          <div className="control-group">
            <label htmlFor="budget">Weekly Budget (₱)</label>
            <input
              id="budget"
              type="number"
              className="budget-input"
              value={weeklyBudget}
              onChange={(e) => onBudgetChange(e.target.value === '' ? '' : Number(e.target.value))}
              placeholder="Enter budget"
              min="0"
            />
          </div>

          <div className="control-group">
            <label htmlFor="mealType">Category</label>
            <select id="mealType" className="filter-dropdown" value={activeFilter} onChange={(e) => setActiveFilter(e.target.value)}>
              <option value="all">All</option>
              <option value="Breakfast">Breakfast</option>
              <option value="Lunch">Lunch</option>
              <option value="Dinner">Dinner</option>
              <option value="Ulam">Ulam</option>
              <option value="Snack">Snack</option>
            </select>
          </div>

          <div className="control-group control-group-action">
            <button type="button" className="add-recipe-btn" onClick={() => setShowModal(true)}>
              <span>＋</span> Add Your Own Recipe
            </button>
          </div>
        </div>

        {/* Meals Grid Panel */}
        <div className="meals-grid">
          {loading && <div className="no-results">Loading recipes…</div>}
          {error && <div className="no-results" style={{ color: 'var(--color-danger, #e53e3e)' }}>{error}</div>}
          
          {!loading && !error && filteredRecipes.length === 0 && (
            <div className="no-results">No recipes found. Try adjusting your budget or filter, or add one!</div>
          )}

          {!loading && filteredRecipes.map((recipe) => {
            const currentDay = selectedDays[recipe.id] || 'Monday';
            
            return (
              <div key={recipe.id} className="meal-card">
                <div className="card-content">
                  <RecipeCard 
                    recipe={recipe}
                    currentDay={currentDay}
                    onAddToPlan={onAddToPlan}
                    onDayChange={(day) => {
                      setSelectedDays({ ...selectedDays, [recipe.id]: day });
                    }}
                  />
                </div>
              </div>
            );
          })}
        </div>
      </div>

      {/* Add Recipe Modal Overlay */}
      {showModal && (
        <AddRecipeModal 
          onClose={() => setShowModal(false)} 
          onRecipeCreated={(createdRecipe) => {
            setRecipes([...recipes, createdRecipe]);
          }}
        />
      )}
    </div>
  );
}