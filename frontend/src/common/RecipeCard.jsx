
export default function RecipeCard({ recipe, currentDay, onAddToPlan, onDayChange }) {
  const displayPrice = recipe.costPerServing ?? 0;

  return (
    <>
      {/* Category Badge and Description */}
      <span className="meal-badge">{recipe.category}</span>
      {recipe.description && <p className="meal-ingredients">{recipe.description}</p>}
      
      {/* Actions Dropdown and Button */}
      <div className="planner-card-actions">
        <select 
          className="plan-day-dropdown" 
          value={currentDay} 
          onChange={(e) => onDayChange(e.target.value)}
        >
          <option value="Monday">Monday</option>
          <option value="Tuesday">Tuesday</option>
          <option value="Wednesday">Wednesday</option>
          <option value="Thursday">Thursday</option>
          <option value="Friday">Friday</option>
          <option value="Saturday">Saturday</option>
          <option value="Sunday">Sunday</option>
        </select>

        <button type="button" className="action-button" onClick={() => onAddToPlan(recipe, currentDay)}>
          Add to Plan
        </button>
      </div>

      {/* Price Tag */}
      <div className="card-details">
        <span className="meal-price">~₱{displayPrice.toLocaleString()}</span>
      </div>
    </>
  );
}