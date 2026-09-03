import type { GastoPorCategoria } from '../types';
import { formatarMoeda } from '../lib/format';

interface AlertaEstouroProps {
  gastos: GastoPorCategoria[];
}

export function AlertaEstouro({ gastos }: AlertaEstouroProps) {
  const estourados = gastos.filter((g) => g.estourouMeta);

  if (estourados.length === 0) return null;

  return (
    <div className="form" style={{ gap: 10 }}>
      {estourados.map((gasto) => {
        const excedente = gasto.totalGasto - (gasto.limiteMeta ?? 0);
        return (
          <div className="alert alert-critical" key={gasto.categoriaId}>
            <span className="alert-icon" aria-hidden="true">
              ⚠
            </span>
            <span>
              <strong>{gasto.categoriaNome}</strong> ultrapassou a meta mensal em{' '}
              <strong>{formatarMoeda(excedente)}</strong> ({formatarMoeda(gasto.totalGasto)} de{' '}
              {formatarMoeda(gasto.limiteMeta ?? 0)}).
            </span>
          </div>
        );
      })}
    </div>
  );
}
