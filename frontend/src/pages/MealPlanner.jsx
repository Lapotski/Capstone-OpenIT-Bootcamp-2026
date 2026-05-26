import { useState } from 'react';

const DEFAULT_MEALS = [
  { id: 1, name: 'Avocado Toast & Eggs',   type: 'breakfast', calories: 350, price: 5,      custom: false },
  { id: 2, name: 'Paluwagan',              type: 'breakfast', calories: 450, price: 50, custom: false },
  { id: 3, name: 'Paldo',                  type: 'lunch',     calories: 520, price: 120,    custom: false },
  { id: 4, name: 'Quinoa Buddha Bowl',     type: 'lunch',     calories: 480, price: 10,     custom: false },
  { id: 5, name: 'Pan-Seared Salmon',      type: 'dinner',    calories: 650, price: 8,     custom: false },
  { id: 6, name: 'Ribeye Steak & Veggies', type: 'dinner',    calories: 800, price: 25,     custom: false },
];

const DAYS_OF_WEEK = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

const EMPTY_FORM = { name: '', type: 'breakfast', ingredients: '', price: '' };

export default function MealPlanner({ onAddToPlan, weeklyBudget, onBudgetChange }) {
  const [meals, setMeals] = useState(DEFAULT_MEALS);
  const [activeFilter, setActiveFilter] = useState('all');
  const [selectedDays, setSelectedDays] = useState({});
  const [showModal, setShowModal] = useState(false);
  const [form, setForm] = useState(EMPTY_FORM);
  const [formError, setFormError] = useState('');

  const budgetNum = weeklyBudget === '' ? Infinity : Number(weeklyBudget);

  const filteredMeals = meals.filter((meal) => {
    const matchesType = activeFilter === 'all' || meal.type === activeFilter;
    const fitsWeeklyBudget = meal.price <= budgetNum;
    return matchesType && fitsWeeklyBudget;
  });

  const handleDeleteMeal = (id) => {
    setMeals((prev) => prev.filter((m) => m.id !== id));
  };

  const handleFormChange = (field, value) => {
    setForm((prev) => ({ ...prev, [field]: value }));
    setFormError('');
  };

  const handleAddRecipe = () => {
    if (!form.name.trim()) { setFormError('Recipe name is required.'); return; }
    if (!form.price || isNaN(Number(form.price)) || Number(form.price) < 0) {
      setFormError('Enter a valid price.'); return;
    }
    if (!form.ingredients.trim()) {
      setFormError('Recipe ingredients are required.'); return;
    }
    const newMeal = {
      id: Date.now(),
      name: form.name.trim(),
      type: form.type,
      ingredients: form.ingredients.trim(),
      price: Number(form.price),
      custom: true,
    };
    setMeals((prev) => [...prev, newMeal]);
    setForm(EMPTY_FORM);
    setFormError('');
    setShowModal(false);
  };

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
              onChange={(e) => {
                const val = e.target.value;
                onBudgetChange(val === '' ? '' : Number(val));
              }}
              placeholder="Enter budget"
              min="0"
            />
          </div>

          <div className="control-group">
            <label htmlFor="mealType">Meal Type</label>
            <select
              id="mealType"
              className="filter-dropdown"
              value={activeFilter}
              onChange={(e) => setActiveFilter(e.target.value)}
            >
              <option value="all">All Meals</option>
              <option value="breakfast">Breakfast</option>
              <option value="lunch">Lunch</option>
              <option value="dinner">Dinner</option>
            </select>
          </div>

          <div className="control-group control-group-action">
            <button
              type="button"
              className="add-recipe-btn"
              onClick={() => setShowModal(true)}
            >
              <span>＋</span> Add Your Own Recipe
            </button>
          </div>
        </div>

        {/* Meals Grid */}
        <div className="meals-grid">
          {filteredMeals.length > 0 ? (
            filteredMeals.map((meal) => (
              <div key={meal.id} className="meal-card">
                <div className="card-content">
                  <div className="meal-card-header">
                    <h3 className="meal-title">{meal.name}</h3>
                    {meal.custom && (
                      <button
                        type="button"
                        className="trash-btn"
                        onClick={() => handleDeleteMeal(meal.id)}
                        aria-label={`Delete ${meal.name}`}
                        title="Delete recipe"
                      >
                        🗑️
                      </button>
                    )}
                  </div>
                  <span className="meal-badge">{meal.type}</span>
                  {meal.ingredients && <p className="meal-ingredients">{meal.ingredients}</p>}
                  <div className="planner-card-actions">
                    <select
                      className="plan-day-dropdown"
                      value={selectedDays[meal.id] || DAYS_OF_WEEK[0]}
                      onChange={(e) =>
                        setSelectedDays((current) => ({
                          ...current,
                          [meal.id]: e.target.value,
                        }))
                      }
                    >
                      {DAYS_OF_WEEK.map((day) => (
                        <option key={day} value={day}>{day}</option>
                      ))}
                    </select>
                    <button
                      type="button"
                      className="action-button"
                      onClick={() => onAddToPlan(meal, selectedDays[meal.id] || DAYS_OF_WEEK[0])}
                    >
                      Add to Plan
                    </button>
                  </div>
                  <div className="card-details">
                    <span className="meal-price">~₱{meal.price.toLocaleString()}</span>
                  </div>
                </div>
              </div>
            ))
          ) : (
            <div className="no-results">
              No meals found matching your current budget or type filter. Try increasing your weekly budget or changing the filter.
            </div>
          )}
        </div>
      </div>

      {/* Add Recipe Modal */}
      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal-box" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h3>Add Your Own Recipe</h3>
              <button type="button" className="modal-close" onClick={() => setShowModal(false)}>✕</button>
            </div>

            <div className="modal-body">
              <div className="modal-field">
                <label>Recipe Name *</label>
                <input
                  type="text"
                  placeholder="e.g. Chicken Adobo"
                  value={form.name}
                  onChange={(e) => handleFormChange('name', e.target.value)}
                  className="budget-input"
                />
              </div>
              <div className="modal-field">
                <label>Meal Type *</label>
                <select
                  className="filter-dropdown"
                  value={form.type}
                  onChange={(e) => handleFormChange('type', e.target.value)}
                >
                  <option value="breakfast">Breakfast</option>
                  <option value="lunch">Lunch</option>
                  <option value="dinner">Dinner</option>
                </select>
              </div>
              <div className="modal-field">
                <label>Price (₱) *</label>
                <input
                  type="number"
                  placeholder="0"
                  min="0"
                  value={form.price}
                  onChange={(e) => handleFormChange('price', e.target.value)}
                  className="budget-input"
                />
              </div>
              <div className="modal-field">
                <label>Ingredients *</label>
                <textarea
                  placeholder="e.g. chicken, garlic, soy sauce"
                  value={form.ingredients}
                  onChange={(e) => handleFormChange('ingredients', e.target.value)}
                  className="budget-input"
                />
              </div>
              {formError && <p className="modal-error">{formError}</p>}
            </div>

            <div className="modal-footer">
              <button type="button" className="modal-cancel-btn" onClick={() => setShowModal(false)}>Cancel</button>
              <button type="button" className="action-button" onClick={handleAddRecipe}>Add Recipe</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
