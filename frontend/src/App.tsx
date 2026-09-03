import type { ReactNode } from 'react';
import { Navigate, Route, Routes } from 'react-router-dom';
import { Layout } from './components/Layout';
import { ProtectedRoute } from './components/ProtectedRoute';
import { useAuth } from './context/AuthContext';
import { CategoriasPage } from './pages/CategoriasPage';
import { DashboardPage } from './pages/DashboardPage';
import { LoginPage } from './pages/LoginPage';
import { RegistrarPage } from './pages/RegistrarPage';
import { TransacoesPage } from './pages/TransacoesPage';

function RedirecionarSeAutenticado({ children }: { children: ReactNode }) {
  const { autenticado } = useAuth();
  if (autenticado) return <Navigate to="/" replace />;
  return <>{children}</>;
}

export default function App() {
  return (
    <Routes>
      <Route
        path="/login"
        element={
          <RedirecionarSeAutenticado>
            <LoginPage />
          </RedirecionarSeAutenticado>
        }
      />
      <Route
        path="/registrar"
        element={
          <RedirecionarSeAutenticado>
            <RegistrarPage />
          </RedirecionarSeAutenticado>
        }
      />

      <Route
        element={
          <ProtectedRoute>
            <Layout />
          </ProtectedRoute>
        }
      >
        <Route path="/" element={<DashboardPage />} />
        <Route path="/transacoes" element={<TransacoesPage />} />
        <Route path="/categorias" element={<CategoriasPage />} />
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
