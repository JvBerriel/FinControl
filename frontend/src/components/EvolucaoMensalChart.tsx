import {
  CartesianGrid,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
  type TooltipContentProps,
} from 'recharts';
import type { PontoEvolucaoMensal } from '../hooks/useDashboard';
import { formatarMoeda } from '../lib/format';

interface EvolucaoMensalChartProps {
  dados: PontoEvolucaoMensal[];
}

const SERIES = [
  { key: 'receitas', nome: 'Receitas', cor: 'var(--series-1)' },
  { key: 'despesas', nome: 'Despesas', cor: 'var(--series-2)' },
] as const;

function TooltipPersonalizado({
  active,
  payload,
  label,
}: Partial<TooltipContentProps<number, string>>) {
  if (!active || !payload?.length) return null;

  return (
    <div className="chart-tooltip">
      <div className="chart-tooltip__title">{label}</div>
      {SERIES.map((serie) => {
        const item = payload.find((p) => p.dataKey === serie.key);
        if (!item) return null;
        return (
          <div className="chart-tooltip__row" key={serie.key}>
            <span className="chart-tooltip__key">
              <span className="chart-tooltip__swatch" style={{ background: serie.cor }} />
              {serie.nome}
            </span>
            <span className="chart-tooltip__value">{formatarMoeda(Number(item.value))}</span>
          </div>
        );
      })}
    </div>
  );
}

export function EvolucaoMensalChart({ dados }: EvolucaoMensalChartProps) {
  return (
    <div>
      <ResponsiveContainer width="100%" height={260}>
        <LineChart data={dados} margin={{ top: 8, right: 12, left: 0, bottom: 0 }}>
          <CartesianGrid vertical={false} stroke="var(--gridline)" />
          <XAxis
            dataKey="label"
            tickLine={false}
            axisLine={{ stroke: 'var(--baseline)' }}
            tick={{ fill: 'var(--text-muted)', fontSize: 12 }}
          />
          <YAxis
            tickLine={false}
            axisLine={false}
            width={64}
            tick={{ fill: 'var(--text-muted)', fontSize: 12 }}
            tickFormatter={(valor: number) =>
              valor >= 1000 ? `${(valor / 1000).toFixed(0)}k` : String(valor)
            }
          />
          <Tooltip content={<TooltipPersonalizado />} cursor={{ stroke: 'var(--baseline)' }} />
          {SERIES.map((serie) => (
            <Line
              key={serie.key}
              type="monotone"
              dataKey={serie.key}
              name={serie.nome}
              stroke={serie.cor}
              strokeWidth={2}
              dot={{ r: 4, fill: serie.cor, strokeWidth: 2, stroke: 'var(--surface-3)' }}
              activeDot={{ r: 5, fill: serie.cor, strokeWidth: 2, stroke: 'var(--surface-3)' }}
            />
          ))}
        </LineChart>
      </ResponsiveContainer>
      <div className="chart-legend">
        {SERIES.map((serie) => (
          <span className="chart-legend__item" key={serie.key}>
            <span className="chart-tooltip__swatch" style={{ background: serie.cor }} />
            {serie.nome}
          </span>
        ))}
      </div>
    </div>
  );
}
