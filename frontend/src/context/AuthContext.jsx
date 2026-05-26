import { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { api, setToken } from '../services/Api';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser]       = useState(null);
  const [loading, setLoading] = useState(true);

  // On mount, try to restore session via stored token
useEffect(() => {
//   if (!getToken()) return; // Simply exit without calling state updates

  api.getMe()
    .then(setUser)
    .catch(() => { setToken(""); setUser(null); })
    .finally(() => setLoading(false)); // Turn off loading here
}, []);

  const login = useCallback(async (email, password) => {
    const res = await api.login({ email, password });
    setToken(res.token);
    setUser(res);
    return res;
  }, []);

  const register = useCallback(async (displayName, email, password, weeklyBudget) => {
    const res = await api.register({ displayName, email, password, weeklyBudget });
    // After register, log in automatically
    return login(email, password).then(() => res);
  }, [login]);

  const logout = useCallback(async () => {
    await api.logout().catch(() => {});
    setToken('');
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