  import { useState, useEffect, useCallback } from 'react';
  import { api } from '../services/Api';
  import { useAuth } from '../context/AuthContext';

  const DAYS_OF_WEEK = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
  const EMPTY_FORM   = { name: '', category: 'Breakfast', description: '', instructions: '', costPerServing: '' };

  export default function MealPlanner({ onAddToPlan, weeklyBudget, onBudgetChange }) {
    const { user } = useAuth();

    const [recipes, setRecipes]         = useState([]);
    const [loading, setLoading]         = useState(false);
    const [error, setError]             = useState('');
    const [activeFilter, setActiveFilter] = useState('all');
    const [selectedDays, setSelectedDays] = useState({});
    const [showModal, setShowModal]     = useState(false);
    const [form, setForm]               = useState(EMPTY_FORM);
    const [formError, setFormError]     = useState('');
    const [submitting, setSubmitting]   = useState(false);

    // ── Fetch recipes from backend ──────────────────────────────────────────────
    const fetchRecipes = useCallback(async () => {
      if (!user) return;
      setLoading(true); setError('');
      try {
        const params = {};
        if (activeFilter !== 'all') params.category = activeFilter;
        const data = await api.getRecipes(params);
        setRecipes(data);
      } catch{
        setError('Could not load recipes. Is the backend running?');
      } finally {
        setLoading(false);
      }
    }, [user, activeFilter]);

    useEffect(() => { fetchRecipes(); }, [fetchRecipes]);

    // ── Budget filter (client-side) ─────────────────────────────────────────────
    const budgetNum = weeklyBudget === '' ? Infinity : Number(weeklyBudget);
    const filteredRecipes = recipes.filter((r) => (r.costPerServing ?? 0) <= budgetNum);

    // ── Add custom recipe ───────────────────────────────────────────────────────
    const handleFormChange = (field, value) => { setForm((f) => ({ ...f, [field]: value })); setFormError(''); };

    const handleAddRecipe = async () => {
      if (!form.name.trim())         { setFormError('Recipe name is required.'); return; }
      if (!form.costPerServing || isNaN(Number(form.costPerServing)) || Number(form.costPerServing) < 0)
        { setFormError('Enter a valid cost.'); return; }

      setSubmitting(true);
      try {
        const created = await api.createRecipe({
          name: form.name.trim(),
          category: form.category,
          description: form.description.trim(),
          instructions: form.instructions.trim(),
          costPerServing: Number(form.costPerServing),
          ingredientIds: [],
        });
        setRecipes((prev) => [...prev, created]);
        setForm(EMPTY_FORM);
        setShowModal(false);
      } catch (err) {
        setFormError(err.data?.error || 'Failed to create recipe.');
      } finally {
        setSubmitting(false);
      }
    };

    // ── Delete custom recipe ────────────────────────────────────────────────────
    const handleDelete = async (id) => {
      try {
        await api.deleteRecipe(id);
        setRecipes((prev) => prev.filter((r) => r.id !== id));
      } catch {
        // If we can't delete (e.g. it's a global recipe or in a plan), show nothing
      }
    };

    // ── Unauthenticated fallback ────────────────────────────────────────────────
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

          {/* Meals Grid */}
          <div className="meals-grid">
            {loading && <div className="no-results">Loading recipes…</div>}
            {error   && <div className="no-results" style={{ color: 'var(--color-danger, #e53e3e)' }}>{error}</div>}
            {!loading && !error && filteredRecipes.length === 0 && (
              <div className="no-results">No recipes found. Try adjusting your budget or filter, or add one!</div>
            )}
            {!loading && filteredRecipes.map((recipe) => (
              <div key={recipe.id} className="meal-card">
                <div className="card-content">
                  <div className="meal-card-header">
                    <h3 className="meal-title">{recipe.name}</h3>
                    {recipe.isOwner && (
                      <button type="button" className="trash-btn" onClick={() => handleDelete(recipe.id)} aria-label={`Delete ${recipe.name}`} title="Delete recipe">🗑️</button>
                    )}
                  </div>
                  <span className="meal-badge">{recipe.category}</span>
                  {recipe.description && <p className="meal-ingredients">{recipe.description}</p>}
                  <div className="planner-card-actions">
                    <select
                      className="plan-day-dropdown"
                      value={selectedDays[recipe.id] || DAYS_OF_WEEK[0]}
                      onChange={(e) => setSelectedDays((s) => ({ ...s, [recipe.id]: e.target.value }))}
                    >
                      {DAYS_OF_WEEK.map((day) => <option key={day} value={day}>{day}</option>)}
                    </select>
                    <button type="button" className="action-button" onClick={() => onAddToPlan(recipe, selectedDays[recipe.id] || DAYS_OF_WEEK[0])}>
                      Add to Plan
                    </button>
                  </div>
                  <div className="card-details">
                    <span className="meal-price">~₱{(recipe.costPerServing ?? 0).toLocaleString()}</span>
                  </div>
                </div>
              </div>
            ))}
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
                  <input type="text" placeholder="e.g. Chicken Adobo" value={form.name} onChange={(e) => handleFormChange('name', e.target.value)} className="budget-input" />
                </div>
                <div className="modal-field">
                  <label>Category *</label>
                  <select className="filter-dropdown" value={form.category} onChange={(e) => handleFormChange('category', e.target.value)}>
                    <option value="Breakfast">Breakfast</option>
                    <option value="Lunch">Lunch</option>
                    <option value="Dinner">Dinner</option>
                    <option value="Ulam">Ulam</option>
                    <option value="Snack">Snack</option>
                  </select>
                </div>
                <div className="modal-field">
                  <label>Cost Per Serving (₱) *</label>
                  <input type="number" placeholder="0" min="0" value={form.costPerServing} onChange={(e) => handleFormChange('costPerServing', e.target.value)} className="budget-input" />
                </div>
                <div className="modal-field">
                  <label>Description</label>
                  <textarea placeholder="Short description…" value={form.description} onChange={(e) => handleFormChange('description', e.target.value)} className="budget-input" />
                </div>
                <div className="modal-field">
                  <label>Instructions</label>
                  <textarea placeholder="Steps to prepare…" value={form.instructions} onChange={(e) => handleFormChange('instructions', e.target.value)} className="budget-input" />
                </div>
                {formError && <p className="modal-error">{formError}</p>}
              </div>
              <div className="modal-footer">
                <button type="button" className="modal-cancel-btn" onClick={() => setShowModal(false)}>Cancel</button>
                <button type="button" className="action-button" onClick={handleAddRecipe} disabled={submitting}>
                  {submitting ? 'Saving…' : 'Add Recipe'}
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    );
  }