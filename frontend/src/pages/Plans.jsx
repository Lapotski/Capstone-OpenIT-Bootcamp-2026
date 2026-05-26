import { useState, useEffect, useCallback } from 'react';
import { api } from '../services/Api';
import { useAuth } from '../context/AuthContext';

const DAYS_OF_WEEK = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

/**
 * Plans page — shows the user's current weekly plan fetched from the backend,
 * plus a local optimistic view from the in-memory `plans` prop (for items just added).
 *
 * Strategy:
 *  - On mount (when user is logged in) fetch the latest weekly plan.
 *  - Merge with locally-added items (from the `plans` prop) so UX stays responsive.
 *  - Remove button calls the backend then also removes from local state.
 */
export default function Plans({ plans: localPlans, onRemove, weeklyBudget }) {
  const { user } = useAuth();
  const [apiPlan,  setApiPlan]  = useState(null);  // WeeklyPlanDto from backend
  const [loading,  setLoading]  = useState(false);
  const [error,    setError]    = useState('');

  const budget = weeklyBudget === '' ? null : Number(weeklyBudget);

  // ── Fetch or create a plan for the current week ──────────────────────────────
  const fetchPlan = useCallback(async () => {
    if (!user) return;
    setLoading(true); setError('');
    try {
      const all = await api.getPlans();
      if (all.length > 0) {
        // Use the most recent plan
        setApiPlan(all[all.length - 1]);
      } else {
        // Create a default plan for this week
        const now = new Date();
        const monday = new Date(now);
        monday.setDate(now.getDate() - ((now.getDay() + 6) % 7));
        const sunday = new Date(monday);
        sunday.setDate(monday.getDate() + 6);
        const created = await api.createPlan({
          name: `Week of ${monday.toLocaleDateString()}`,
          weekStart: monday.toISOString().slice(0, 10),
        });
        setApiPlan(created);
      }
    } catch {
      setError('Could not load your weekly plan.');
    } finally {
      setLoading(false);
    }
  }, [user]);

  useEffect(() => { fetchPlan(); }, [fetchPlan]);

  // ── Remove an item from the backend plan ─────────────────────────────────────
  const handleRemoveApiItem = async (planId, itemId, recipeId, day) => {
    try {
      await api.removePlanItem(planId, itemId);
      setApiPlan((prev) => ({
        ...prev,
        mealPlanItems: prev.mealPlanItems.filter((i) => i.id !== itemId),
      }));
    } catch { /* ignore */ }
    // Also remove from local state by recipeId + day
    onRemove(recipeId, day);
  };

  // ── Merge local + api items into per-day groups ──────────────────────────────
  const groupedPlans = DAYS_OF_WEEK.reduce((acc, day) => {
    const apiItems = (apiPlan?.mealPlanItems ?? [])
      .filter((i) => i.dayOfWeek === day)
      .map((i) => ({ ...i, _source: 'api' }));

    const localItems = localPlans
      .filter((p) => p.day === day)
      .filter((p) => !apiItems.some((a) => a.recipeId === p.id))
      .map((p) => ({ ...p, recipeId: p.id, recipeName: p.name, estimatedCost: p.costPerServing ?? p.price ?? 0, _source: 'local' }));

    acc[day] = [...apiItems, ...localItems];
    return acc;
  }, {});

  const dayTotals = DAYS_OF_WEEK.reduce((acc, day) => {
    acc[day] = groupedPlans[day].reduce((s, i) => s + (i.totalCost ?? i.estimatedCost ?? 0), 0);
    return acc;
  }, {});
  const weekTotal  = Object.values(dayTotals).reduce((s, t) => s + t, 0);
  const overBudget = budget !== null && weekTotal > budget;
  const maxItems   = Math.max(1, ...Object.values(groupedPlans).map((a) => a.length));
  const isEmpty    = Object.values(groupedPlans).every((a) => a.length === 0);

  if (!user) {
    return (
      <div className="plans-page">
        <div className="no-results" style={{ margin: '4rem auto', textAlign: 'center' }}>
          <p style={{ fontSize: '1.25rem' }}>Please <strong>log in</strong> to see your weekly plan.</p>
        </div>
      </div>
    );
  }

  if (loading) return <div className="plans-page"><div className="no-results">Loading your plan…</div></div>;
  if (error)   return <div className="plans-page"><div className="no-results" style={{ color: '#e53e3e' }}>{error}</div></div>;

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
            <tr>{DAYS_OF_WEEK.map((day) => <th key={day}>{day}</th>)}</tr>
          </thead>
          <tbody>
            {Array.from({ length: maxItems }).map((_, rowIdx) => (
              <tr key={rowIdx}>
                {DAYS_OF_WEEK.map((day) => {
                  const item = groupedPlans[day][rowIdx];
                  return (
                    <td key={day + rowIdx} className="plans-cell">
                      {item ? (
                        <div className="plan-card">
                          <div>
                            <h4>{item.recipeName ?? item.name}</h4>
                            <p className="plan-type">{item.category ?? item.type} • ₱{(item.totalCost ?? item.estimatedCost ?? 0).toLocaleString()}</p>
                          </div>
                          <button
                            type="button"
                            className="action-button remove-button"
                            onClick={() => {
                              if (item._source === 'api') {
                                handleRemoveApiItem(apiPlan.id, item.id, item.recipeId, day);
                              } else {
                                onRemove(item.id, day);
                              }
                            }}
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
            <tr className="day-totals-row">
              {DAYS_OF_WEEK.map((day) => {
                const total  = dayTotals[day];
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