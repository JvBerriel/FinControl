export type TipoTransacao = 1 | 2;

export const TIPO_RECEITA: TipoTransacao = 1;
export const TIPO_DESPESA: TipoTransacao = 2;

export interface Usuario {
  usuarioId: number;
  nome: string;
  email: string;
  token: string;
  expiraEm: string;
}

export interface Categoria {
  id: number;
  usuarioId: number;
  nome: string;
  cor: string;
  icone: string | null;
  ativa: boolean;
}

export interface CriarCategoria {
  nome: string;
  cor: string;
  icone?: string | null;
}

export interface AtualizarCategoria {
  nome: string;
  cor: string;
  icone?: string | null;
  ativa: boolean;
}

export interface Transacao {
  id: number;
  usuarioId: number;
  categoriaId: number;
  categoriaNome: string;
  descricao: string;
  valor: number;
  tipo: TipoTransacao;
  dataTransacao: string;
}

export interface CriarTransacao {
  categoriaId: number;
  descricao: string;
  valor: number;
  tipo: TipoTransacao;
  dataTransacao: string;
}

export interface AtualizarTransacao {
  categoriaId: number;
  descricao: string;
  valor: number;
  tipo: TipoTransacao;
  dataTransacao: string;
}

export interface GastoPorCategoria {
  categoriaId: number;
  categoriaNome: string;
  totalGasto: number;
  limiteMeta: number | null;
  estourouMeta: boolean;
}

export interface ResumoMensal {
  mes: number;
  ano: number;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
  gastosPorCategoria: GastoPorCategoria[];
}

export interface MediaCategoria {
  categoriaId: number;
  categoriaNome: string;
  mediaMensal: number;
  mesesConsiderados: number;
}

export interface SugestaoInvestimento {
  rendaMensal: number;
  totalDespesasMes: number;
  percentualReserva: number;
  valorReservaSeguranca: number;
  valorSugeridoInvestimento: number;
}

export interface ErroApi {
  mensagem: string;
  erros?: { campo: string; erro: string }[];
}
