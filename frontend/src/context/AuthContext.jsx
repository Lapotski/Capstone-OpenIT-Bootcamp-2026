import { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { api } from '../services/Api'; // Cleaned: setToken is removed

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  // ── Session Restoration ───────────────────────────────────────────────────
  // On mount, the browser automatically sends the HttpOnly auth cookie to /getMe
  useEffect(() => {
    api.getMe()
      .then((userData) => {
        setUser(userData);
      })
      .catch(() => { 
        // If the cookie is expired, missing, or invalid, reset user state quietly
        setUser(null); 
      })
      .finally(() => {
        setLoading(false);
      });
  }, []);

  // ── Actions ───────────────────────────────────────────────────────────────
  const login = useCallback(async (email, password) => {
    // The backend intercepts this payload and establishes the session cookie on response headers
    const res = await api.login({ email, password });
    setUser(res); // Expected response format is now your raw profile data object (no token string needed)
    return res;
  }, []);

  const register = useCallback(async (displayName, email, password, weeklyBudget) => {
    const res = await api.register({ displayName, email, password, weeklyBudget });
    // After register, perform sequential cookie login
    return login(email, password).then(() => res);
  }, [login]);

  const logout = useCallback(async () => {
    // The backend clears or expires the HttpOnly auth cookie on this call
    await api.logout().catch(() => {});
    setUser(null);
  }, []);

  const updateBudget = useCallback(async (weeklyBudget) => {
    const updated = await api.patchMe({ weeklyBudget });
    setUser((u) => ({ ...u, weeklyBudget: updated.weeklyBudget }));
    return updated;
  }, []);

  return (
    <AuthContext.Provider value={{ user, loading, login, register, logout, updateBudget }}>
      {children}
    </AuthContext.Provider>
  );        
}

export function useAuth() {
  return useContext(AuthContext);
}