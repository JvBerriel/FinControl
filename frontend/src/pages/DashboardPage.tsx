import { useMemo, useState } from 'react';
import { AlertaEstouro } from '../components/AlertaEstouro';
import { EvolucaoMensalChart } from '../components/EvolucaoMensalChart';
import { GastosPorCategoriaChart } from '../components/GastosPorCategoriaChart';
import { StatTile } from '../components/StatTile';
import { useCategorias } from '../hooks/useCategorias';
import { useEvolucaoMensal, useResumoMensal, useSugestaoInvestimento } from '../hooks/useDashboard';
import { formatarMoeda, nomeDoMes } from '../lib/format';

function periodoAtual() {
  const hoje = new Date();
  return { mes: hoje.getMonth() + 1, ano: hoje.getFullYear() };
}

export function DashboardPage() {
  const [{ mes, ano }, setPeriodo] = useState(periodoAtual);

  const { data: categorias } = useCategorias();
  const { data: resumo, isLoading: carregandoResumo } = useResumoMensal(mes, ano);
  const { data: sugestao } = useSugestaoInvestimento(mes, ano);
  const { dados: evolucao, carregando: carregandoEvolucao } = useEvolucaoMensal(6);

  const coresPorCategoria = useMemo(() => {
    const mapa: Record<number, string> = {};
    categorias?.forEach((categoria) => {
      mapa[categoria.id] = categoria.cor;
    });
    return mapa;
  }, [categorias]);

  function mudarMes(delta: number) {
    setPeriodo((atual) => {
      const data = new Date(atual.ano, atual.mes - 1 + delta, 1);
      return { mes: data.getMonth() + 1, ano: data.getFullYear() };
    });
  }

  const gastos = resumo?.gastosPorCategoria ?? [];

  return (
    <>
      <div className="page-header">
        <h1>Dashboard</h1>
        <div className="form-actions">
          <button type="button" className="btn btn-secondary" onClick={() => mudarMes(-1)}>
            ← Mês anterior
          </button>
          <strong>
            {nomeDoMes(mes)} de {ano}
          </strong>
          <button type="button" className="btn btn-secondary" onClick={() => mudarMes(1)}>
            Mês seguinte →
          </button>
        </div>
      </div>

      {!carregandoResumo && resumo && <AlertaEstouro gastos={gastos} />}

      <div className="stat-grid">
        <StatTile label="Receitas do mês" value={formatarMoeda(resumo?.totalReceitas ?? 0)} tone="positive" />
        <StatTile label="Despesas do mês" value={formatarMoeda(resumo?.totalDespesas ?? 0)} tone="negative" />
        <StatTile
          label="Saldo do mês"
          value={formatarMoeda(resumo?.saldo ?? 0)}
          tone={(resumo?.saldo ?? 0) >= 0 ? 'positive' : 'negative'}
        />
        <StatTile
          label="Sugestão de investimento"
          value={formatarMoeda(sugestao?.valorSugeridoInvestimento ?? 0)}
        />
      </div>

      <div className="card">
        <div className="card__title">Gastos por categoria</div>
        {carregandoResumo && <div className="loading-state">Carregando...</div>}
        {!carregandoResumo && gastos.length === 0 && (
          <div className="empty-state">Nenhuma despesa lançada neste mês.</div>
        )}
        {!carregandoResumo && gastos.length > 0 && (
          <GastosPorCategoriaChart gastos={gastos} cores={coresPorCategoria} />
        )}
      </div>

      <div className="card">
        <div className="card__title">Evolução mensal (últimos 6 meses)</div>
        {carregandoEvolucao && <div className="loading-state">Carregando...</div>}
        {!carregandoEvolucao && <EvolucaoMensalChart dados={evolucao} />}
      </div>
    </>
  );
}
