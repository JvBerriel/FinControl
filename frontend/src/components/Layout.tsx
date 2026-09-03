import { NavLink, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const LINKS = [
  { to: '/', label: 'Dashboard' },
  { to: '/transacoes', label: 'Transações' },
  { to: '/categorias', label: 'Categorias' },
];

export function Layout() {
  const { usuario, sair } = useAuth();

  return (
    <div className="app-shell">
      <nav className="app-nav">
        <span className="app-nav__brand">FinControl</span>
        <div className="app-nav__links">
          {LINKS.map((link) => (
            <NavLink
              key={link.to}
              to={link.to}
              end={link.to === '/'}
              className={({ isActive }) => `app-nav__link${isActive ? ' active' : ''}`}
            >
              {link.label}
            </NavLink>
          ))}
        </div>
        <div className="app-nav__user">
          <span>{usuario?.nome}</span>
          <button type="button" className="btn-link" onClick={sair}>
            Sair
          </button>
        </div>
      </nav>
      <main className="app-main">
        <Outlet />
      </main>
    </div>
  );
}
