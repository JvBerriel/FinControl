import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  atualizarTransacao,
  criarTransacao,
  listarTransacoes,
  removerTransacao,
} from '../api/transacoes';
import type { AtualizarTransacao, CriarTransacao } from '../types';

const TRANSACOES_QUERY_KEY = ['transacoes'];
const DASHBOARD_QUERY_PREFIX = ['dashboard'];

export function useTransacoes() {
  return useQuery({
    queryKey: TRANSACOES_QUERY_KEY,
    queryFn: listarTransacoes,
  });
}

function invalidarTransacoesEDashboard(queryClient: ReturnType<typeof useQueryClient>) {
  queryClient.invalidateQueries({ queryKey: TRANSACOES_QUERY_KEY });
  queryClient.invalidateQueries({ queryKey: DASHBOARD_QUERY_PREFIX });
}

export function useCriarTransacao() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CriarTransacao) => criarTransacao(payload),
    onSuccess: () => invalidarTransacoesEDashboard(queryClient),
  });
}

export function useAtualizarTransacao() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: AtualizarTransacao }) =>
      atualizarTransacao(id, payload),
    onSuccess: () => invalidarTransacoesEDashboard(queryClient),
  });
}

export function useRemoverTransacao() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => removerTransacao(id),
    onSuccess: () => invalidarTransacoesEDashboard(queryClient),
  });
}
