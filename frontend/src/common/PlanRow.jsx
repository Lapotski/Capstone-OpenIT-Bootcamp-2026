import React from 'react';

export default function PlanRowItem({ item, day, apiPlanId, onRemove, onRemoveLocal }) {
  // If there's no item assigned to this row slot, show a placeholder dash
  if (!item) {
    return <div className="empty-cell">—</div>;
  }

  const titleText = item.recipeName ?? item.name;
  const categoryText = item.category ?? item.type;
  const itemCost = item.totalCost ?? item.estimatedCost ?? 0;

  function handleCancelClick() {
    if (item._source === 'api') {
      onRemove(apiPlanId, item.id, item.recipeId, day);
    } else {
      onRemoveLocal(item.id, day);
    }
  }

  return (
    <div className="plan-card">
      <div>
        <h4>{titleText}</h4>
        <p className="plan-type">
          {categoryText} • ₱{itemCost.toLocaleString()}
        </p>
      </div>
      <button
        type="button"
        className="action-button remove-button"
        onClick={handleCancelClick}
      >
        Remove
      </button>
    </div>
  );
}