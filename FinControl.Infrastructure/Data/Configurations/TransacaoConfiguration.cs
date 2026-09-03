using FinControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinControl.Infrastructure.Data.Configurations;

public class TransacaoConfiguration : IEntityTypeConfiguration<Transacao>
{
    public void Configure(EntityTypeBuilder<Transacao> builder)
    {
        builder.ToTable("transacoes", t =>
        {
            t.HasCheckConstraint("ck_transacoes_valor", "valor > 0");
            t.HasCheckConstraint("ck_transacoes_tipo", "tipo IN (1, 2)");
        });

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id");

        builder.Property(t => t.UsuarioId).HasColumnName("usuario_id");
        builder.Property(t => t.CategoriaId).HasColumnName("categoria_id");
        builder.Property(t => t.Descricao).HasColumnName("descricao").HasMaxLength(200).IsRequired();
        builder.Property(t => t.Valor).HasColumnName("valor").HasColumnType("decimal(12,2)");
        builder.Property(t => t.Tipo).HasColumnName("tipo").HasConversion<short>();
        builder.Property(t => t.DataTransacao).HasColumnName("data_transacao");
        builder.Property(t => t.CriadoEm).HasColumnName("criado_em").HasDefaultValueSql("now()");

        builder.HasIndex(t => t.UsuarioId);
        builder.HasIndex(t => t.CategoriaId);
        builder.HasIndex(t => new { t.UsuarioId, t.DataTransacao });
    }
}
