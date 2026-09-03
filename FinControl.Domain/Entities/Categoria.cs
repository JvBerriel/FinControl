namespace FinControl.Domain.Entities;

public class Categoria
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    public string? Icone { get; set; }
    public bool Ativa { get; set; } = true;

    public Usuario? Usuario { get; set; }
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    public ICollection<MetaMensal> MetasMensais { get; set; } = new List<MetaMensal>();
}
