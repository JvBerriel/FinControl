import { useQueries, useQuery } from '@tanstack/react-query';
import { obterMediasPorCategoria, obterResumoMensal, obterSugestaoInvestimento } from '../api/dashboard';

export function useResumoMensal(mes: number, ano: number) {
  return useQuery({
    queryKey: ['dashboard', 'resumo-mensal', mes, ano],
    queryFn: () => obterResumoMensal(mes, ano),
  });
}

export function useMediasPorCategoria(quantidadeMeses = 6) {
  return useQuery({
    queryKey: ['dashboard', 'medias-por-categoria', quantidadeMeses],
    queryFn: () => obterMediasPorCategoria(quantidadeMeses),
  });
}

export function useSugestaoInvestimento(mes: number, ano: number, percentualReserva = 0.2) {
  return useQuery({
    queryKey: ['dashboard', 'sugestao-investimento', mes, ano, percentualReserva],
    queryFn: () => obterSugestaoInvestimento(mes, ano, percentualReserva),
  });
}

const NOMES_MESES = [
  'Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun',
  'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez',
];

function ultimosMeses(quantidade: number): { mes: number; ano: number }[] {
  const hoje = new Date();
  const meses: { mes: number; ano: number }[] = [];

  for (let i = quantidade - 1; i >= 0; i--) {
    const data = new Date(hoje.getFullYear(), hoje.getMonth() - i, 1);
    meses.push({ mes: data.getMonth() + 1, ano: data.getFullYear() });
  }

  return meses;
}

export interface PontoEvolucaoMensal {
  label: string;
  mes: number;
  ano: number;
  receitas: number;
  despesas: number;
  saldo: number;
}

export function useEvolucaoMensal(quantidadeMeses = 6) {
  const periodos = ultimosMeses(quantidadeMeses);

  const resultados = useQueries({
    queries: periodos.map(({ mes, ano }) => ({
      queryKey: ['dashboard', 'resumo-mensal', mes, ano],
      queryFn: () => obterResumoMensal(mes, ano),
    })),
  });

  const carregando = resultados.some((r) => r.isLoading);
  const comErro = resultados.some((r) => r.isError);

  const dados: PontoEvolucaoMensal[] = resultados.map((resultado, indice) => {
    const { mes, ano } = periodos[indice];
    const resumo = resultado.data;
    return {
      label: `${NOMES_MESES[mes - 1]}/${String(ano).slice(2)}`,
      mes,
      ano,
      receitas: resumo?.totalReceitas ?? 0,
      despesas: resumo?.totalDespesas ?? 0,
      saldo: resumo?.saldo ?? 0,
    };
  });

  return { dados, carregando, comErro };
}
