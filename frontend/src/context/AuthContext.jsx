import { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { api } from '../services/Api'; 

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    api.getMe()
      .then((userData) => {
        setUser(userData);
      })
      .catch(() => { 
        setUser(null); 
      })
      .finally(() => {
        setLoading(false);
      });
  }, []);

  // ── Actions ───────────────────────────────────────────────────────────────
  const login = useCallback(async (email, password) => {
    const res = await api.login({ email, password });
    setUser(res); 
    return res;
  }, []);

  const register = useCallback(async (displayName, email, password, weeklyBudget) => {
    const res = await api.register({ displayName, email, password, weeklyBudget });
    return login(email, password).then(() => res);
  }, [login]);

  const logout = useCallback(async () => {
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