import { apiClient } from './client';
import type { AtualizarTransacao, CriarTransacao, Transacao } from '../types';

export async function listarTransacoes(): Promise<Transacao[]> {
  const { data } = await apiClient.get<Transacao[]>('/transacoes');
  return data;
}

export async function criarTransacao(payload: CriarTransacao): Promise<Transacao> {
  const { data } = await apiClient.post<Transacao>('/transacoes', payload);
  return data;
}

export async function atualizarTransacao(id: number, payload: AtualizarTransacao): Promise<Transacao> {
  const { data } = await apiClient.put<Transacao>(`/transacoes/${id}`, payload);
  return data;
}

export async function removerTransacao(id: number): Promise<void> {
  await apiClient.delete(`/transacoes/${id}`);
}
