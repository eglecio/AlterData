namespace Dominio.ModelosDTO {
  public class ProdutoDTO {
    public int Id { get; set; }
    public string Nome { get; set; }
    public double QuantidadeEstoque { get; set; }
    public double ValorVenda { get; set; }
    public double ValorCusto { get; set; }
    public string? Observacao { get; set; }
  }
}
