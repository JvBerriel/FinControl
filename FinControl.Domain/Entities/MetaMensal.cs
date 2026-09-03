namespace FinControl.Domain.Entities;

public class MetaMensal
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int CategoriaId { get; set; }
    public decimal ValorLimite { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }

    public Usuario? Usuario { get; set; }
    public Categoria? Categoria { get; set; }
}
