import { zodResolver } from '@hookform/resolvers/zod';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import { z } from 'zod';
import { login } from '../api/auth';
import { extrairMensagemErro } from '../api/client';
import { useAuth } from '../context/AuthContext';

const schema = z.object({
  email: z.string().min(1, "'Email' deve ser informado.").email('Informe um e-mail válido.'),
  senha: z.string().min(1, "'Senha' deve ser informada."),
});

type FormValues = z.infer<typeof schema>;

export function LoginPage() {
  const { entrar } = useAuth();
  const navigate = useNavigate();
  const [erroApi, setErroApi] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({ resolver: zodResolver(schema) });

  async function aoSubmeter(valores: FormValues) {
    setErroApi(null);
    try {
      const usuario = await login(valores);
      entrar(usuario);
      navigate('/', { replace: true });
    } catch (erro) {
      setErroApi(extrairMensagemErro(erro));
    }
  }

  return (
    <div className="auth-shell">
      <div className="auth-card">
        <div>
          <h1 className="auth-card__title">Entrar no FinControl</h1>
          <p className="auth-card__subtitle">Controle suas finanças pessoais em um só lugar.</p>
        </div>

        <form className="form" onSubmit={handleSubmit(aoSubmeter)} noValidate>
          <div className="form-row">
            <label htmlFor="email">E-mail</label>
            <input id="email" type="email" autoComplete="email" {...register('email')} />
            {errors.email && <span className="form-error">{errors.email.message}</span>}
          </div>

          <div className="form-row">
            <label htmlFor="senha">Senha</label>
            <input id="senha" type="password" autoComplete="current-password" {...register('senha')} />
            {errors.senha && <span className="form-error">{errors.senha.message}</span>}
          </div>

          {erroApi && <span className="form-error">{erroApi}</span>}

          <div className="form-actions">
            <button type="submit" className="btn btn-primary" disabled={isSubmitting}>
              {isSubmitting ? 'Entrando...' : 'Entrar'}
            </button>
          </div>
        </form>

        <div className="auth-card__footer">
          Ainda não tem conta? <Link to="/registrar">Cadastre-se</Link>
        </div>
      </div>
    </div>
  );
}
