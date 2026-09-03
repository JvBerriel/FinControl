import axios from 'axios';
import type { ErroApi } from '../types';

export const TOKEN_STORAGE_KEY = 'fincontrol.token';
export const USUARIO_STORAGE_KEY = 'fincontrol.usuario';

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem(TOKEN_STORAGE_KEY);
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem(TOKEN_STORAGE_KEY);
      localStorage.removeItem(USUARIO_STORAGE_KEY);
      if (window.location.pathname !== '/login') {
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  },
);

export function extrairMensagemErro(error: unknown): string {
  if (axios.isAxiosError<ErroApi>(error)) {
    const dados = error.response?.data;
    if (dados?.erros?.length) {
      return dados.erros.map((e) => e.erro).join(' ');
    }
    if (dados?.mensagem) {
      return dados.mensagem;
    }
  }
  return 'Ocorreu um erro inesperado. Tente novamente.';
}
