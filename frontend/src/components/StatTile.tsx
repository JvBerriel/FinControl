interface StatTileProps {
  label: string;
  value: string;
  tone?: 'neutral' | 'positive' | 'negative';
}

export function StatTile({ label, value, tone = 'neutral' }: StatTileProps) {
  const toneClass =
    tone === 'positive'
      ? 'stat-tile__value--positive'
      : tone === 'negative'
        ? 'stat-tile__value--negative'
        : '';

  return (
    <div className="stat-tile">
      <span className="stat-tile__label">{label}</span>
      <span className={`stat-tile__value ${toneClass}`}>{value}</span>
    </div>
  );
}
