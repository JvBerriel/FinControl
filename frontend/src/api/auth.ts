import { apiClient } from './client';
import type { Usuario } from '../types';

export interface LoginPayload {
  email: string;
  senha: string;
}

export interface RegistrarPayload {
  nome: string;
  email: string;
  senha: string;
  rendaMensal: number;
}

export async function login(payload: LoginPayload): Promise<Usuario> {
  const { data } = await apiClient.post<Usuario>('/auth/login', payload);
  return data;
}

export async function registrar(payload: RegistrarPayload): Promise<Usuario> {
  const { data } = await apiClient.post<Usuario>('/auth/registrar', payload);
  return data;
}
