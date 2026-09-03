import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip, type TooltipContentProps } from 'recharts';
import type { GastoPorCategoria } from '../types';
import { formatarMoeda } from '../lib/format';

interface GastosPorCategoriaChartProps {
  gastos: GastoPorCategoria[];
  cores: Record<number, string>;
}

function TooltipPersonalizado({ active, payload }: Partial<TooltipContentProps<number, string>>) {
  if (!active || !payload?.length) return null;
  const item = payload[0];
  const gasto = item.payload as GastoPorCategoria;

  return (
    <div className="chart-tooltip">
      <div className="chart-tooltip__title">{gasto.categoriaNome}</div>
      <div className="chart-tooltip__row">
        <span className="chart-tooltip__key">Total gasto</span>
        <span className="chart-tooltip__value">{formatarMoeda(gasto.totalGasto)}</span>
      </div>
      {gasto.limiteMeta !== null && (
        <div className="chart-tooltip__row">
          <span className="chart-tooltip__key">Meta mensal</span>
          <span className="chart-tooltip__value">{formatarMoeda(gasto.limiteMeta)}</span>
        </div>
      )}
    </div>
  );
}

export function GastosPorCategoriaChart({ gastos, cores }: GastosPorCategoriaChartProps) {
  const total = gastos.reduce((soma, g) => soma + g.totalGasto, 0);

  return (
    <div>
      <ResponsiveContainer width="100%" height={260}>
        <PieChart>
          <Pie
            data={gastos}
            dataKey="totalGasto"
            nameKey="categoriaNome"
            innerRadius={60}
            outerRadius={100}
            paddingAngle={gastos.length > 1 ? 2 : 0}
            stroke="var(--surface-3)"
            strokeWidth={2}
            label={({ percent }: { percent?: number }) =>
              percent && percent >= 0.08 ? `${Math.round(percent * 100)}%` : ''
            }
            labelLine={false}
          >
            {gastos.map((gasto) => (
              <Cell key={gasto.categoriaId} fill={cores[gasto.categoriaId] ?? 'var(--series-1)'} />
            ))}
          </Pie>
          <Tooltip content={<TooltipPersonalizado />} />
        </PieChart>
      </ResponsiveContainer>
      <div className="chart-legend">
        {gastos.map((gasto) => (
          <span className="chart-legend__item" key={gasto.categoriaId}>
            <span
              className="badge-dot"
              style={{ background: cores[gasto.categoriaId] ?? 'var(--series-1)' }}
            />
            {gasto.categoriaNome} · {formatarMoeda(gasto.totalGasto)}
            {total > 0 ? ` (${Math.round((gasto.totalGasto / total) * 100)}%)` : ''}
          </span>
        ))}
      </div>
    </div>
  );
}
