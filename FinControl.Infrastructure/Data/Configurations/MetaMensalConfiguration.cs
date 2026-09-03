using FinControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinControl.Infrastructure.Data.Configurations;

public class MetaMensalConfiguration : IEntityTypeConfiguration<MetaMensal>
{
    public void Configure(EntityTypeBuilder<MetaMensal> builder)
    {
        builder.ToTable("metas_mensais", t =>
        {
            t.HasCheckConstraint("ck_metas_mensais_valor_limite", "valor_limite > 0");
            t.HasCheckConstraint("ck_metas_mensais_mes", "mes BETWEEN 1 AND 12");
            t.HasCheckConstraint("ck_metas_mensais_ano", "ano >= 2000");
        });

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id");

        builder.Property(m => m.UsuarioId).HasColumnName("usuario_id");
        builder.Property(m => m.CategoriaId).HasColumnName("categoria_id");
        builder.Property(m => m.ValorLimite).HasColumnName("valor_limite").HasColumnType("decimal(12,2)");
        builder.Property(m => m.Mes).HasColumnName("mes");
        builder.Property(m => m.Ano).HasColumnName("ano");

        builder.HasIndex(m => new { m.UsuarioId, m.CategoriaId, m.Mes, m.Ano }).IsUnique();
        builder.HasIndex(m => new { m.UsuarioId, m.Ano, m.Mes });
    }
}
