using FinControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinControl.Infrastructure.Data.Configurations;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("categorias");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.UsuarioId).HasColumnName("usuario_id");
        builder.Property(c => c.Nome).HasColumnName("nome").HasMaxLength(100).IsRequired();
        builder.Property(c => c.Cor).HasColumnName("cor").HasMaxLength(7).IsRequired();
        builder.Property(c => c.Icone).HasColumnName("icone").HasMaxLength(50);
        builder.Property(c => c.Ativa).HasColumnName("ativa").HasDefaultValue(true);

        builder.HasIndex(c => new { c.UsuarioId, c.Nome }).IsUnique();

        builder.HasMany(c => c.Transacoes)
            .WithOne(t => t.Categoria)
            .HasForeignKey(t => t.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.MetasMensais)
            .WithOne(m => m.Categoria)
            .HasForeignKey(m => m.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
