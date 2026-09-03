using Microsoft.AspNetCore.Identity;

namespace FinControl.Domain.Entities;

public class Usuario : IdentityUser<int>
{
    public string Nome { get; set; } = string.Empty;
    public decimal RendaMensal { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    public ICollection<MetaMensal> MetasMensais { get; set; } = new List<MetaMensal>();
}
