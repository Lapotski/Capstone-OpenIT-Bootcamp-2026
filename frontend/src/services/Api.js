const BASE = 'http://localhost:5000/api';

// ─── Auth token helpers ───────────────────────────────────────────────────────
let _token = localStorage.getItem('token') || '';

export function setToken(t) {
  _token = t;
  if (t) localStorage.setItem('token', t);
  else localStorage.removeItem('token');
}

export function getToken() { return _token; }

function authHeaders(extra = {}) {
  return {
    'Content-Type': 'application/json',
    ...(_token ? { Authorization: `Bearer ${_token}` } : {}),
    ...extra,
  };
}

async function req(method, path, body) {
  const res = await fetch(`${BASE}${path}`, {
    method,
    headers: authHeaders(),
    ...(body !== undefined ? { body: JSON.stringify(body) } : {}),
  });
  if (res.status === 204) return null;
  const data = await res.json().catch(() => ({}));
  if (!res.ok) throw Object.assign(new Error(data.error || res.statusText), { status: res.status, data });
  return data;
}

const get  = (path)         => req('GET',    path);
const post = (path, body)   => req('POST',   path, body);
const patch = (path, body)  => req('PATCH',  path, body);
const del  = (path)         => req('DELETE', path);

// ─── Users ────────────────────────────────────────────────────────────────────
export const api = {
  register: (dto) => post('/users/register', dto),
  login:    (dto) => post('/users/login?useCookies=true', dto),
  logout:   ()    => post('/users/logout'),
  getMe:    ()    => get('/users/me'),
  patchMe:  (dto) => patch('/users/me', dto),

  // ─── Recipes ─────────────────────────────────────────────────────────────
  getRecipes:    (params = {}) => {
    const q = new URLSearchParams();
    if (params.search)        q.set('search', params.search);
    if (params.category)      q.set('category', params.category);
    if (params.sortByCostAsc) q.set('sortByCostAsc', 'true');
    return get(`/recipes${q.toString() ? '?' + q : ''}`);
  },
  getRecipe:     (id)   => get(`/recipes/${id}`),
  createRecipe:  (dto)  => post('/recipes', dto),
  patchRecipe:   (id, dto) => patch(`/recipes/${id}`, dto),
  deleteRecipe:  (id)   => del(`/recipes/${id}`),
  saveRecipe:    (id)   => post(`/recipes/${id}/save`),
  unsaveRecipe:  (id)   => del(`/recipes/${id}/save`),

  // ─── Weekly Plans ─────────────────────────────────────────────────────────
  getPlans:      ()     => get('/weekly-plans'),
  getPlan:       (id)   => get(`/weekly-plans/${id}`),
  createPlan:    (dto)  => post('/weekly-plans', dto),
  patchPlan:     (id, dto) => patch(`/weekly-plans/${id}`, dto),
  deletePlan:    (id)   => del(`/weekly-plans/${id}`),
  addPlanItem:   (planId, dto) => post(`/weekly-plans/${planId}/items`, dto),
  patchPlanItem: (planId, itemId, dto) => patch(`/weekly-plans/${planId}/items/${itemId}`, dto),
  removePlanItem:(planId, itemId) => del(`/weekly-plans/${planId}/items/${itemId}`),

  // ─── Ingredients ──────────────────────────────────────────────────────────
  getIngredients: (search) => get(`/ingredients${search ? '?search=' + encodeURIComponent(search) : ''}`),
  createIngredient: (dto)  => post('/ingredients', dto),
  patchIngredient:  (id, dto) => patch(`/ingredients/${id}`, dto),
  deleteIngredient: (id)   => del(`/ingredients/${id}`),
};