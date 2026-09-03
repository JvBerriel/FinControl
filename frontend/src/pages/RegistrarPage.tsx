import { zodResolver } from '@hookform/resolvers/zod';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import { z } from 'zod';
import { registrar } from '../api/auth';
import { extrairMensagemErro } from '../api/client';
import { useAuth } from '../context/AuthContext';

const schema = z.object({
  nome: z.string().min(1, "'Nome' deve ser informado."),
  email: z.string().min(1, "'Email' deve ser informado.").email('Informe um e-mail válido.'),
  senha: z.string().min(8, 'A senha deve ter pelo menos 8 caracteres.'),
  rendaMensal: z.number().min(0, 'A renda mensal não pode ser negativa.'),
});

type FormValues = z.infer<typeof schema>;

export function RegistrarPage() {
  const { entrar } = useAuth();
  const navigate = useNavigate();
  const [erroApi, setErroApi] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({ resolver: zodResolver(schema), defaultValues: { rendaMensal: 0 } });

  async function aoSubmeter(valores: FormValues) {
    setErroApi(null);
    try {
      const usuario = await registrar(valores);
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
          <h1 className="auth-card__title">Criar conta</h1>
          <p className="auth-card__subtitle">Leva menos de um minuto.</p>
        </div>

        <form className="form" onSubmit={handleSubmit(aoSubmeter)} noValidate>
          <div className="form-row">
            <label htmlFor="nome">Nome</label>
            <input id="nome" autoComplete="name" {...register('nome')} />
            {errors.nome && <span className="form-error">{errors.nome.message}</span>}
          </div>

          <div className="form-row">
            <label htmlFor="email">E-mail</label>
            <input id="email" type="email" autoComplete="email" {...register('email')} />
            {errors.email && <span className="form-error">{errors.email.message}</span>}
          </div>

          <div className="form-row">
            <label htmlFor="senha">Senha</label>
            <input id="senha" type="password" autoComplete="new-password" {...register('senha')} />
            {errors.senha && <span className="form-error">{errors.senha.message}</span>}
          </div>

          <div className="form-row">
            <label htmlFor="rendaMensal">Renda mensal (R$)</label>
            <input
              id="rendaMensal"
              type="number"
              step="0.01"
              min="0"
              {...register('rendaMensal', { valueAsNumber: true })}
            />
            {errors.rendaMensal && <span className="form-error">{errors.rendaMensal.message}</span>}
          </div>

          {erroApi && <span className="form-error">{erroApi}</span>}

          <div className="form-actions">
            <button type="submit" className="btn btn-primary" disabled={isSubmitting}>
              {isSubmitting ? 'Criando conta...' : 'Criar conta'}
            </button>
          </div>
        </form>

        <div className="auth-card__footer">
          Já tem conta? <Link to="/login">Entrar</Link>
        </div>
      </div>
    </div>
  );
}
