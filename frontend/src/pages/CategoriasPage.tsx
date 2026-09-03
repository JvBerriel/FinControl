import { zodResolver } from '@hookform/resolvers/zod';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { extrairMensagemErro } from '../api/client';
import {
  useAtualizarCategoria,
  useCategorias,
  useCriarCategoria,
  useRemoverCategoria,
} from '../hooks/useCategorias';
import type { Categoria } from '../types';

const schema = z.object({
  nome: z.string().min(1, "'Nome' deve ser informado.").max(100),
  cor: z.string().regex(/^#[0-9A-Fa-f]{6}$/, 'Escolha uma cor válida.'),
  icone: z.string().max(50).optional(),
});

type FormValues = z.infer<typeof schema>;

const CORES_SUGERIDAS = [
  '#2a78d6', '#eb6834', '#1baf7a', '#eda100', '#e87ba4', '#4a3aa7', '#e34948', '#008300',
];

export function CategoriasPage() {
  const { data: categorias, isLoading } = useCategorias();
  const criar = useCriarCategoria();
  const atualizar = useAtualizarCategoria();
  const remover = useRemoverCategoria();

  const [categoriaEmEdicao, setCategoriaEmEdicao] = useState<Categoria | null>(null);
  const [erroApi, setErroApi] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    watch,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { nome: '', cor: CORES_SUGERIDAS[0], icone: '' },
  });

  const corAtual = watch('cor');

  function iniciarEdicao(categoria: Categoria) {
    setCategoriaEmEdicao(categoria);
    reset({ nome: categoria.nome, cor: categoria.cor, icone: categoria.icone ?? '' });
  }

  function cancelarEdicao() {
    setCategoriaEmEdicao(null);
    reset({ nome: '', cor: CORES_SUGERIDAS[0], icone: '' });
  }

  async function aoSubmeter(valores: FormValues) {
    setErroApi(null);
    try {
      if (categoriaEmEdicao) {
        await atualizar.mutateAsync({
          id: categoriaEmEdicao.id,
          payload: { ...valores, ativa: categoriaEmEdicao.ativa },
        });
      } else {
        await criar.mutateAsync(valores);
      }
      cancelarEdicao();
    } catch (erro) {
      setErroApi(extrairMensagemErro(erro));
    }
  }

  async function alternarAtiva(categoria: Categoria) {
    setErroApi(null);
    try {
      await atualizar.mutateAsync({
        id: categoria.id,
        payload: {
          nome: categoria.nome,
          cor: categoria.cor,
          icone: categoria.icone,
          ativa: !categoria.ativa,
        },
      });
    } catch (erro) {
      setErroApi(extrairMensagemErro(erro));
    }
  }

  async function excluir(categoria: Categoria) {
    if (!window.confirm(`Excluir a categoria "${categoria.nome}"?`)) return;
    setErroApi(null);
    try {
      await remover.mutateAsync(categoria.id);
    } catch (erro) {
      setErroApi(extrairMensagemErro(erro));
    }
  }

  return (
    <>
      <div className="page-header">
        <h1>Categorias</h1>
      </div>

      <div className="card">
        <div className="card__title">{categoriaEmEdicao ? 'Editar categoria' : 'Nova categoria'}</div>
        <form className="form" onSubmit={handleSubmit(aoSubmeter)} noValidate>
          <div className="form-row--inline">
            <div className="form-row">
              <label htmlFor="nome">Nome</label>
              <input id="nome" {...register('nome')} />
              {errors.nome && <span className="form-error">{errors.nome.message}</span>}
            </div>

            <div className="form-row">
              <label htmlFor="cor">Cor</label>
              <input id="cor" type="color" {...register('cor')} style={{ height: 40, padding: 4 }} />
              {errors.cor && <span className="form-error">{errors.cor.message}</span>}
            </div>

            <div className="form-row">
              <label htmlFor="icone">Ícone (opcional)</label>
              <input id="icone" placeholder="ex: carro, casa..." {...register('icone')} />
            </div>
          </div>

          <div className="form-row" style={{ flexDirection: 'row', gap: 8, flexWrap: 'wrap' }}>
            {CORES_SUGERIDAS.map((cor) => (
              <button
                type="button"
                key={cor}
                onClick={() =>
                  reset({ nome: watch('nome'), cor, icone: watch('icone') }, { keepDefaultValues: false })
                }
                style={{
                  width: 22,
                  height: 22,
                  borderRadius: '50%',
                  background: cor,
                  border: corAtual === cor ? '2px solid var(--text-primary)' : '1px solid var(--border)',
                  cursor: 'pointer',
                }}
                aria-label={`Usar cor ${cor}`}
              />
            ))}
          </div>

          {erroApi && <span className="form-error">{erroApi}</span>}

          <div className="form-actions">
            <button type="submit" className="btn btn-primary" disabled={isSubmitting}>
              {categoriaEmEdicao ? 'Salvar alterações' : 'Criar categoria'}
            </button>
            {categoriaEmEdicao && (
              <button type="button" className="btn btn-secondary" onClick={cancelarEdicao}>
                Cancelar
              </button>
            )}
          </div>
        </form>
      </div>

      <div className="card">
        <div className="card__title">Suas categorias</div>
        {isLoading && <div className="loading-state">Carregando...</div>}
        {!isLoading && categorias?.length === 0 && (
          <div className="empty-state">Nenhuma categoria criada ainda.</div>
        )}
        {!isLoading && categorias && categorias.length > 0 && (
          <table className="table">
            <thead>
              <tr>
                <th>Categoria</th>
                <th>Ícone</th>
                <th>Status</th>
                <th />
              </tr>
            </thead>
            <tbody>
              {categorias.map((categoria) => (
                <tr key={categoria.id} style={{ opacity: categoria.ativa ? 1 : 0.5 }}>
                  <td>
                    <span className="badge">
                      <span className="badge-dot" style={{ background: categoria.cor }} />
                      {categoria.nome}
                    </span>
                  </td>
                  <td>{categoria.icone || '—'}</td>
                  <td>{categoria.ativa ? 'Ativa' : 'Inativa'}</td>
                  <td>
                    <div className="table-actions">
                      <button type="button" className="btn-link" onClick={() => iniciarEdicao(categoria)}>
                        Editar
                      </button>
                      <button type="button" className="btn-link" onClick={() => alternarAtiva(categoria)}>
                        {categoria.ativa ? 'Desativar' : 'Ativar'}
                      </button>
                      <button type="button" className="btn-danger" onClick={() => excluir(categoria)}>
                        Excluir
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </>
  );
}
