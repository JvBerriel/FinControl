import { zodResolver } from '@hookform/resolvers/zod';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { extrairMensagemErro } from '../api/client';
import { useCategorias } from '../hooks/useCategorias';
import {
  useAtualizarTransacao,
  useCriarTransacao,
  useRemoverTransacao,
  useTransacoes,
} from '../hooks/useTransacoes';
import { TIPO_DESPESA, TIPO_RECEITA, type Transacao } from '../types';
import { formatarData, formatarMoeda } from '../lib/format';

function hoje(): string {
  return new Date().toISOString().slice(0, 10);
}

const schema = z.object({
  categoriaId: z.number().min(1, 'Selecione uma categoria.'),
  descricao: z.string().min(1, "'Descrição' deve ser informada.").max(200),
  valor: z.number().gt(0, "'Valor' deve ser maior que zero."),
  tipo: z.union([z.literal(TIPO_RECEITA), z.literal(TIPO_DESPESA)]),
  dataTransacao: z.string().min(1, 'Informe a data.'),
});

type FormValues = z.infer<typeof schema>;

export function TransacoesPage() {
  const { data: categorias } = useCategorias();
  const { data: transacoes, isLoading } = useTransacoes();
  const criar = useCriarTransacao();
  const atualizar = useAtualizarTransacao();
  const remover = useRemoverTransacao();

  const [transacaoEmEdicao, setTransacaoEmEdicao] = useState<Transacao | null>(null);
  const [erroApi, setErroApi] = useState<string | null>(null);

  const categoriasAtivas = categorias?.filter((c) => c.ativa) ?? [];

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      categoriaId: 0,
      descricao: '',
      valor: 0,
      tipo: TIPO_DESPESA,
      dataTransacao: hoje(),
    },
  });

  function iniciarEdicao(transacao: Transacao) {
    setTransacaoEmEdicao(transacao);
    reset({
      categoriaId: transacao.categoriaId,
      descricao: transacao.descricao,
      valor: transacao.valor,
      tipo: transacao.tipo,
      dataTransacao: transacao.dataTransacao,
    });
  }

  function cancelarEdicao() {
    setTransacaoEmEdicao(null);
    reset({ categoriaId: 0, descricao: '', valor: 0, tipo: TIPO_DESPESA, dataTransacao: hoje() });
  }

  async function aoSubmeter(valores: FormValues) {
    setErroApi(null);
    try {
      if (transacaoEmEdicao) {
        await atualizar.mutateAsync({ id: transacaoEmEdicao.id, payload: valores });
      } else {
        await criar.mutateAsync(valores);
      }
      cancelarEdicao();
    } catch (erro) {
      setErroApi(extrairMensagemErro(erro));
    }
  }

  async function excluir(transacao: Transacao) {
    if (!window.confirm(`Excluir a transação "${transacao.descricao}"?`)) return;
    setErroApi(null);
    try {
      await remover.mutateAsync(transacao.id);
    } catch (erro) {
      setErroApi(extrairMensagemErro(erro));
    }
  }

  return (
    <>
      <div className="page-header">
        <h1>Transações</h1>
      </div>

      <div className="card">
        <div className="card__title">{transacaoEmEdicao ? 'Editar transação' : 'Nova transação'}</div>

        {categoriasAtivas.length === 0 ? (
          <p className="empty-state">Crie uma categoria antes de lançar transações.</p>
        ) : (
          <form className="form" onSubmit={handleSubmit(aoSubmeter)} noValidate>
            <div className="form-row--inline">
              <div className="form-row">
                <label htmlFor="descricao">Descrição</label>
                <input id="descricao" {...register('descricao')} />
                {errors.descricao && <span className="form-error">{errors.descricao.message}</span>}
              </div>

              <div className="form-row">
                <label htmlFor="categoriaId">Categoria</label>
                <select id="categoriaId" {...register('categoriaId', { valueAsNumber: true })}>
                  <option value={0}>Selecione...</option>
                  {categoriasAtivas.map((categoria) => (
                    <option key={categoria.id} value={categoria.id}>
                      {categoria.nome}
                    </option>
                  ))}
                </select>
                {errors.categoriaId && <span className="form-error">{errors.categoriaId.message}</span>}
              </div>

              <div className="form-row">
                <label htmlFor="tipo">Tipo</label>
                <select id="tipo" {...register('tipo', { valueAsNumber: true })}>
                  <option value={TIPO_DESPESA}>Despesa</option>
                  <option value={TIPO_RECEITA}>Receita</option>
                </select>
              </div>
            </div>

            <div className="form-row--inline">
              <div className="form-row">
                <label htmlFor="valor">Valor (R$)</label>
                <input
                  id="valor"
                  type="number"
                  step="0.01"
                  min="0.01"
                  {...register('valor', { valueAsNumber: true })}
                />
                {errors.valor && <span className="form-error">{errors.valor.message}</span>}
              </div>

              <div className="form-row">
                <label htmlFor="dataTransacao">Data</label>
                <input id="dataTransacao" type="date" {...register('dataTransacao')} />
                {errors.dataTransacao && (
                  <span className="form-error">{errors.dataTransacao.message}</span>
                )}
              </div>
            </div>

            {erroApi && <span className="form-error">{erroApi}</span>}

            <div className="form-actions">
              <button type="submit" className="btn btn-primary" disabled={isSubmitting}>
                {transacaoEmEdicao ? 'Salvar alterações' : 'Lançar transação'}
              </button>
              {transacaoEmEdicao && (
                <button type="button" className="btn btn-secondary" onClick={cancelarEdicao}>
                  Cancelar
                </button>
              )}
            </div>
          </form>
        )}
      </div>

      <div className="card">
        <div className="card__title">Histórico</div>
        {isLoading && <div className="loading-state">Carregando...</div>}
        {!isLoading && transacoes?.length === 0 && (
          <div className="empty-state">Nenhuma transação lançada ainda.</div>
        )}
        {!isLoading && transacoes && transacoes.length > 0 && (
          <table className="table">
            <thead>
              <tr>
                <th>Data</th>
                <th>Descrição</th>
                <th>Categoria</th>
                <th>Tipo</th>
                <th>Valor</th>
                <th />
              </tr>
            </thead>
            <tbody>
              {transacoes.map((transacao) => (
                <tr key={transacao.id}>
                  <td>{formatarData(transacao.dataTransacao)}</td>
                  <td>{transacao.descricao}</td>
                  <td>{transacao.categoriaNome}</td>
                  <td className={transacao.tipo === TIPO_RECEITA ? 'tag-receita' : 'tag-despesa'}>
                    {transacao.tipo === TIPO_RECEITA ? 'Receita' : 'Despesa'}
                  </td>
                  <td>{formatarMoeda(transacao.valor)}</td>
                  <td>
                    <div className="table-actions">
                      <button type="button" className="btn-link" onClick={() => iniciarEdicao(transacao)}>
                        Editar
                      </button>
                      <button type="button" className="btn-danger" onClick={() => excluir(transacao)}>
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
