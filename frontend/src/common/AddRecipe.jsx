import { useState } from 'react';
import { api } from '../services/Api';

export default function AddRecipeModal({ onClose, onRecipeCreated }) {
  const [name, setName] = useState('');
  const [category, setCategory] = useState('Breakfast');
  const [cost, setCost] = useState('');
  const [desc, setDesc] = useState('');
  const [inst, setInst] = useState('');
  const [formError, setFormError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  function handleSubmit() {
    if (name.trim() === '') {
      setFormError('Recipe name is required.');
      return;
    }
    if (cost === '' || isNaN(Number(cost)) || Number(cost) < 0) {
      setFormError('Enter a valid cost.');
      return;
    }

    setSubmitting(true);

    const newRecipeData = {
      name: name.trim(),
      category: category,
      description: desc.trim(),
      instructions: inst.trim(),
      costPerServing: Number(cost),
      ingredientIds: []
    };

    api.createRecipe(newRecipeData)
      .then((createdRecipe) => {
        onRecipeCreated(createdRecipe);
        setSubmitting(false);
        onClose(); // Closes the modal window
      })
      .catch((err) => {
        setSubmitting(false);
        setFormError(err.data?.error || 'Failed to create recipe.');
      });
  }

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-box" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h3>Add Your Own Recipe</h3>
          <button type="button" className="modal-close" onClick={onClose}>✕</button>
        </div>
        
        <div className="modal-body">
          <div className="modal-field">
            <label>Recipe Name *</label>
            <input type="text" placeholder="e.g. Chicken Adobo" value={name} onChange={(e) => setName(e.target.value)} className="budget-input" />
          </div>

          <div className="modal-field">
            <label>Category *</label>
            <select className="filter-dropdown" value={category} onChange={(e) => setCategory(e.target.value)}>
              <option value="Breakfast">Breakfast</option>
              <option value="Lunch">Lunch</option>
              <option value="Dinner">Dinner</option>
              <option value="Ulam">Ulam</option>
              <option value="Snack">Snack</option>
            </select>
          </div>

          <div className="modal-field">
            <label>Description</label>
            <textarea placeholder="Short description…" value={desc} onChange={(e) => setDesc(e.target.value)} className="budget-input" />
          </div>

          <div className="modal-field">
            <label>Instructions</label>
            <textarea placeholder="Steps to prepare…" value={inst} onChange={(e) => setInst(e.target.value)} className="budget-input" />
          </div>

          {formError && <p className="modal-error">{formError}</p>}
        </div>

        <div className="modal-footer">
          <button type="button" className="modal-cancel-btn" onClick={onClose}>Cancel</button>
          <button type="button" className="action-button" onClick={handleSubmit} disabled={submitting}>
            {submitting ? 'Saving…' : 'Add Recipe'}
          </button>
        </div>
      </div>
    </div>
  );
}