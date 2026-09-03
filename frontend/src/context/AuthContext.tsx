import { createContext, useContext, useMemo, useState, type ReactNode } from 'react';
import { TOKEN_STORAGE_KEY, USUARIO_STORAGE_KEY } from '../api/client';
import type { Usuario } from '../types';

interface AuthContextValue {
  usuario: Usuario | null;
  autenticado: boolean;
  entrar: (usuario: Usuario) => void;
  sair: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

function carregarUsuarioSalvo(): Usuario | null {
  const bruto = localStorage.getItem(USUARIO_STORAGE_KEY);
  if (!bruto) return null;
  try {
    return JSON.parse(bruto) as Usuario;
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [usuario, setUsuario] = useState<Usuario | null>(carregarUsuarioSalvo);

  const entrar = (novoUsuario: Usuario) => {
    localStorage.setItem(TOKEN_STORAGE_KEY, novoUsuario.token);
    localStorage.setItem(USUARIO_STORAGE_KEY, JSON.stringify(novoUsuario));
    setUsuario(novoUsuario);
  };

  const sair = () => {
    localStorage.removeItem(TOKEN_STORAGE_KEY);
    localStorage.removeItem(USUARIO_STORAGE_KEY);
    setUsuario(null);
  };

  const value = useMemo<AuthContextValue>(
    () => ({ usuario, autenticado: usuario !== null, entrar, sair }),
    [usuario],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth deve ser usado dentro de um AuthProvider.');
  }
  return context;
}
