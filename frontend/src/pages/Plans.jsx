export default function Plans({ plans, onRemove, weeklyBudget }) {
  const daysOfWeek = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
  const budget = weeklyBudget === '' ? null : Number(weeklyBudget);

  const groupedPlans = daysOfWeek.reduce((result, day) => {
    result[day] = plans.filter((planItem) => planItem.day === day);
    return result;
  }, {});

  const dayTotals = daysOfWeek.reduce((result, day) => {
    result[day] = groupedPlans[day].reduce((sum, item) => sum + item.price, 0);
    return result;
  }, {});

  const weekTotal = Object.values(dayTotals).reduce((sum, t) => sum + t, 0);
  const overBudget = budget !== null && weekTotal > budget;
  const maxItems = Math.max(0, ...Object.values(groupedPlans).map((arr) => arr.length));

  if (plans.length === 0) {
    return (
      <div className="plans-page">
        <header className="plans-header">
          <h2>Weekly Meal Plans</h2>
          <p>You haven&apos;t added any recipes to the plan yet.</p>
        </header>
      </div>
    );
  }

  return (
    <div className="plans-page">
      <header className="plans-header">
        <h2>Weekly Meal Plans</h2>
        <p>Recipes assigned to each day of the week.</p>

        {/* Week summary bar */}
        <div className={`week-summary${overBudget ? ' over-budget' : ''}`}>
          <span className="week-total-label">Weekly Total:</span>
          <span className="week-total-amount">₱{weekTotal.toLocaleString()}</span>
          {budget !== null && (
            <span className="week-budget-label">
              {overBudget
                ? `⚠️ Over budget by ₱${(weekTotal - budget).toLocaleString()}`
                : `✅ Within the budget with ₱${(budget - weekTotal).toLocaleString()} remaining`}
            </span>
          )}
        </div>
      </header>

      <div className="plans-table-wrap">
        <table className="plans-table">
          <thead>
            <tr>
              {daysOfWeek.map((day) => (
                <th key={day}>{day}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {Array.from({ length: Math.max(1, maxItems) }).map((_, rowIndex) => (
              <tr key={rowIndex}>
                {daysOfWeek.map((day) => {
                  const item = groupedPlans[day][rowIndex];
                  return (
                    <td key={day + rowIndex} className="plans-cell">
                      {item ? (
                        <div className="plan-card">
                          <div>
                            <h4>{item.name}</h4>
                            <p className="plan-type">{item.type} • ₱{item.price.toLocaleString()}</p>
                          </div>
                          <button
                            type="button"
                            className="action-button remove-button"
                            onClick={() => onRemove(item.id, item.day)}
                          >
                            Remove
                          </button>
                        </div>
                      ) : (
                        <div className="empty-cell">—</div>
                      )}
                    </td>
                  );
                })}
              </tr>
            ))}

            {/* Day totals row */}
            <tr className="day-totals-row">
              {daysOfWeek.map((day) => {
                const total = dayTotals[day];
                const dayOver = budget !== null && total > budget;
                return (
                  <td key={day + '-total'} className="day-total-cell">
                    {groupedPlans[day].length > 0 ? (
                      <div className={`day-total-badge${dayOver ? ' day-total-over' : ''}`}>
                        <span className="day-total-icon">Total:</span>
                        <span>₱{total.toLocaleString()}</span>
                        <span className="day-meal-count">{groupedPlans[day].length} meal{groupedPlans[day].length !== 1 ? 's' : ''}</span>
                      </div>
                    ) : (
                      <div className="day-total-empty">—</div>
                    )}
                  </td>
                );
              })}
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  );
}
