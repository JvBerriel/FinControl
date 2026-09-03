import { apiClient } from './client';
import type { MediaCategoria, ResumoMensal, SugestaoInvestimento } from '../types';

export async function obterResumoMensal(mes: number, ano: number): Promise<ResumoMensal> {
  const { data } = await apiClient.get<ResumoMensal>('/dashboard/resumo-mensal', {
    params: { mes, ano },
  });
  return data;
}

export async function obterMediasPorCategoria(quantidadeMeses = 6): Promise<MediaCategoria[]> {
  const { data } = await apiClient.get<MediaCategoria[]>('/dashboard/medias-por-categoria', {
    params: { quantidadeMeses },
  });
  return data;
}

export async function obterSugestaoInvestimento(
  mes: number,
  ano: number,
  percentualReserva = 0.2,
): Promise<SugestaoInvestimento> {
  const { data } = await apiClient.get<SugestaoInvestimento>('/dashboard/sugestao-investimento', {
    params: { mes, ano, percentualReserva },
  });
  return data;
}
