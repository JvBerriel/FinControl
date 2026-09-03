import { apiClient } from './client';
import type { AtualizarCategoria, Categoria, CriarCategoria } from '../types';

export async function listarCategorias(): Promise<Categoria[]> {
  const { data } = await apiClient.get<Categoria[]>('/categorias');
  return data;
}

export async function criarCategoria(payload: CriarCategoria): Promise<Categoria> {
  const { data } = await apiClient.post<Categoria>('/categorias', payload);
  return data;
}

export async function atualizarCategoria(id: number, payload: AtualizarCategoria): Promise<Categoria> {
  const { data } = await apiClient.put<Categoria>(`/categorias/${id}`, payload);
  return data;
}

export async function removerCategoria(id: number): Promise<void> {
  await apiClient.delete(`/categorias/${id}`);
}
