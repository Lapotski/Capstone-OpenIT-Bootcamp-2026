import { useState, useEffect } from 'react';
import { api } from '../services/Api';
import { useAuth } from '../context/AuthContext';
import PlanRowItem from '../common/PlanRow';

const DAYS_OF_WEEK = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

export default function Plans({ plans: localPlans, onRemove, weeklyBudget }) {
  const { user } = useAuth();
  
  const [apiPlan, setApiPlan] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const budget = weeklyBudget === '' ? null : Number(weeklyBudget);

  // Fetch or setup the active weekly schedule
  useEffect(() => {
    if (!user) return;

    setLoading(true);
    setError('');

    api.getPlans()
      .then((all) => {
        if (all.length > 0) {
          setApiPlan(all[all.length - 1]);
          setLoading(false);
        } else {
          const now = new Date();
          const monday = new Date(now);
          monday.setDate(now.getDate() - ((now.getDay() + 6) % 7));
          
          const createdPayload = {
            name: `Week of ${monday.toLocaleDateString()}`,
            weekStart: monday.toISOString().slice(0, 10)
          };

          api.createPlan(createdPayload)
            .then((created) => {
              setApiPlan(created);
              setLoading(false);
            })
            .catch(() => {
              setError('Could not load your weekly plan.');
              setLoading(false);
            });
        }
      })
      .catch(() => {
        setError('Could not load your weekly plan.');
        setLoading(false);
      });
  }, [user]);

  // Handle removing items from database entry records
  function handleRemoveApiItem(planId, itemId, recipeId, day) {
    api.removePlanItem(planId, itemId)
      .then(() => {
        const remainingItems = apiPlan.mealPlanItems.filter((i) => i.id !== itemId);
        setApiPlan({
          ...apiPlan,
          mealPlanItems: remainingItems
        });
        onRemove(recipeId, day);
      })
      .catch(() => {
        // Fall back on failure matching
        onRemove(recipeId, day);
      });
  }

  // Group arrays and map records per calendar column
  const groupedPlans = {};
  DAYS_OF_WEEK.forEach((day) => {
    const apiItems = [];
    if (apiPlan && apiPlan.mealPlanItems) {
      apiPlan.mealPlanItems.forEach((i) => {
        if (i.dayOfWeek === day) {
          apiItems.push({ ...i, _source: 'api' });
        }
      });
    }

    const localItems = [];
    localPlans.forEach((p) => {
      if (p.day === day) {
        const alreadyInApi = apiItems.some((a) => a.recipeId === p.id);
        if (!alreadyInApi) {
          localItems.push({
            ...p,
            recipeId: p.id,
            recipeName: p.name,
            estimatedCost: p.costPerServing ?? p.price ?? 0,
            _source: 'local'
          });
        }
      }
    });

    groupedPlans[day] = [...apiItems, ...localItems];
  });

  // Calculate costs per index grid column
  const dayTotals = {};
  DAYS_OF_WEEK.forEach((day) => {
    let runningSum = 0;
    groupedPlans[day].forEach((i) => {
      runningSum += (i.totalCost ?? i.estimatedCost ?? 0);
    });
    dayTotals[day] = runningSum;
  });

  let weekTotal = 0;
  Object.keys(dayTotals).forEach((day) => {
    weekTotal += dayTotals[day];
  });

  const overBudget = budget !== null && weekTotal > budget;

  // Track max grid row height
  let maxItems = 1;
  DAYS_OF_WEEK.forEach((day) => {
    if (groupedPlans[day].length > maxItems) {
      maxItems = groupedPlans[day].length;
    }
  });

  let isEmpty = true;
  DAYS_OF_WEEK.forEach((day) => {
    if (groupedPlans[day].length > 0) {
      isEmpty = false;
    }
  });

  if (!user) {
    return (
      <div className="plans-page">
        <div className="no-results" style={{ margin: '4rem auto', textAlign: 'center' }}>
          <p style={{ fontSize: '1.25rem' }}>Please <strong>log in</strong> to see your weekly plan.</p>
        </div>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="plans-page">
        <div className="no-results">Loading your plan…</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="plans-page">
        <div className="no-results" style={{ color: '#e53e3e' }}>{error}</div>
      </div>
    );
  }

  if (isEmpty) {
    return (
      <div className="plans-page">
        <header className="plans-header">
          <h2>Weekly Meal Plans</h2>
          <p>You haven&apos;t added any recipes to the plan yet. Head to <em>Recipe</em> and tap "Add to Plan".</p>
        </header>
      </div>
    );
  }

  return (
    <div className="plans-page">
      <header className="plans-header">
        <h2>Weekly Meal Plans</h2>
        <p>Recipes assigned to each day of the week.</p>

        <div className={`week-summary${overBudget ? ' over-budget' : ''}`}>
          <span className="week-total-label">Weekly Total:</span>
          <span className="week-total-amount">₱{weekTotal.toLocaleString()}</span>
          {budget !== null && (
            <span className="week-budget-label">
              {overBudget
                ? `⚠️ Over budget by ₱${(weekTotal - budget).toLocaleString()}`
                : `✅ ₱${(budget - weekTotal).toLocaleString()} remaining`}
            </span>
          )}
        </div>
      </header>

      <div className="plans-table-wrap">
        <table className="plans-table">
          <thead>
            <tr>
              {DAYS_OF_WEEK.map((day) => (
                <th key={day}>{day}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {Array.from({ length: maxItems }).map((_, rowIdx) => (
              <tr key={rowIdx}>
                {DAYS_OF_WEEK.map((day) => {
                  const item = groupedPlans[day][rowIdx];
                  return (
                    <td key={day + rowIdx} className="plans-cell">
                      <PlanRowItem 
                        item={item}
                        day={day}
                        apiPlanId={apiPlan ? apiPlan.id : null}
                        onRemove={handleRemoveApiItem}
                        onRemoveLocal={onRemove}
                      />
                    </td>
                  );
                })}
              </tr>
            ))}
            <tr className="day-totals-row">
              {DAYS_OF_WEEK.map((day) => {
                const total = dayTotals[day];
                const dayOver = budget !== null && total > budget;
                return (
                  <td key={day + '-total'} className="day-total-cell">
                    {groupedPlans[day].length > 0 ? (
                      <div className={`day-total-badge${dayOver ? ' day-total-over' : ''}`}>
                        <span className="day-total-icon">Total:</span>
                        <span>₱{total.toLocaleString()}</span>
                        <span className="day-meal-count">
                          {groupedPlans[day].length} meal{groupedPlans[day].length !== 1 ? 's' : ''}
                        </span>
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