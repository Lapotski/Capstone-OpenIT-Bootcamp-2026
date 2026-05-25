import { useState } from 'react';

const INITIAL_MEALS = [
  { id: 1, name: 'Avocado Toast & Eggs', type: 'breakfast', calories: 350, price: 5 },
  { id: 2, name: 'Buttermilk Pancakes', type: 'breakfast', calories: 450, price: 8 },
  { id: 3, name: 'Grilled Chicken Salad', type: 'lunch', calories: 520, price: 12 },
  { id: 4, name: 'Quinoa Buddha Bowl', type: 'lunch', calories: 480, price: 10},
  { id: 5, name: 'Pan-Seared Salmon', type: 'dinner', calories: 650, price: 18},
  { id: 6, name: 'Ribeye Steak & Veggies', type: 'dinner', calories: 800, price: 25,},
];

export default function MealPlanner() {
  const [activeFilter, setActiveFilter] = useState('all');
  const [weeklyBudget, setWeeklyBudget] = useState(''); // Default empty

  // Filter Logic: Checked against type AND checks if the dish price fits within the weekly budget
  const filteredMeals = INITIAL_MEALS.filter((meal) => {
    const matchesType = activeFilter === 'all' || meal.type === activeFilter;
    const budgetNum = typeof weeklyBudget === 'string' || weeklyBudget === 0 ? Infinity : weeklyBudget;
    const fitsWeeklyBudget = meal.price <= budgetNum;
    return matchesType && fitsWeeklyBudget;
  });       

  return (
    <div className="planner-container">
      <header className="planner-header">
        <h2>Daily Meal Planner</h2>
        <p>Set your budget and filter your structure effortlessly</p>
      </header>

      {/* Input controls Panel */}
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
              if (val === '') {
                setWeeklyBudget('');
              } else {
                const num = Number(val);
                if (!isNaN(num)) {
                  setWeeklyBudget(num);
                }
              }
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
      </div>

      {/* Meals Grid Output */}
      <div className="meals-grid">
        {filteredMeals.length > 0 ? (
          filteredMeals.map((meal) => (
            <div key={meal.id} className="meal-card">
              <div className="card-content">
                <h3 className="meal-title">{meal.name}</h3>
                <span className="meal-badge">{meal.type}</span>
                <div className="card-details">
                  <span className="meal-price">₱    {meal.price}</span>
                </div>
              </div>
            </div>
          ))
        ) : (
          <div className="no-results">
            No meals found matching your current budget or type filter requirements. Try increasing your weekly budget limit.
          </div>
        )}
      </div>
    </div>
  );
}