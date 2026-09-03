using FinControl.Domain.Entities;
using FinControl.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinControl.Infrastructure.Data;

public class FinControlDbContext : IdentityDbContext<Usuario, IdentityRole<int>, int>, IUnitOfWork
{
    public FinControlDbContext(DbContextOptions<FinControlDbContext> options) : base(options)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();
    public DbSet<MetaMensal> MetasMensais => Set<MetaMensal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole<int>>().ToTable("roles");
        modelBuilder.Entity<IdentityUserRole<int>>().ToTable("usuario_roles");
        modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("usuario_claims");
        modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("usuario_logins");
        modelBuilder.Entity<IdentityUserToken<int>>().ToTable("usuario_tokens");
        modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("role_claims");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinControlDbContext).Assembly);
    }

    public Task<int> SaveChangesAsync() => base.SaveChangesAsync();
}
