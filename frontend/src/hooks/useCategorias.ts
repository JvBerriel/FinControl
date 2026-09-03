import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  atualizarCategoria,
  criarCategoria,
  listarCategorias,
  removerCategoria,
} from '../api/categorias';
import type { AtualizarCategoria, CriarCategoria } from '../types';

const CATEGORIAS_QUERY_KEY = ['categorias'];

export function useCategorias() {
  return useQuery({
    queryKey: CATEGORIAS_QUERY_KEY,
    queryFn: listarCategorias,
  });
}

export function useCriarCategoria() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CriarCategoria) => criarCategoria(payload),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: CATEGORIAS_QUERY_KEY }),
  });
}

export function useAtualizarCategoria() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: AtualizarCategoria }) =>
      atualizarCategoria(id, payload),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: CATEGORIAS_QUERY_KEY }),
  });
}

export function useRemoverCategoria() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => removerCategoria(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: CATEGORIAS_QUERY_KEY }),
  });
}
