export default function Plans({ plans, onRemove }) {
  const daysOfWeek = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

  const groupedPlans = daysOfWeek.reduce((result, day) => {
    result[day] = plans.filter((planItem) => planItem.day === day);
    return result;
  }, {});

  const maxItems = Math.max(0, ...Object.values(groupedPlans).map((arr) => arr.length));

  if (plans.length === 0) {
    return (
      <div className="plans-page">
        <header className="plans-header">
          <h2>Weekly Meal Plans</h2>
          <p>You haven't added any recipes to the plan yet.</p>
        </header>
      </div>
    );
  }

  return (
    <div className="plans-page">
      <header className="plans-header">
        <h2>Weekly Meal Plans</h2>
        <p>Recipes assigned to each day of the week.</p>
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
                            <p className="plan-type">{item.type} • ₱{item.price}</p>
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
          </tbody>
        </table>
      </div>
    </div>
  );
}
