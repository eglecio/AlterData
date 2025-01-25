using Dominio.Enumeradores;

namespace Dominio.Entidades {
  public class Produto : EntidadeBase {
    public string Nome { get; set; }
    public string Observacao { get; set; }
    public double QuantidadeEstoque { get; set; }
    public double ValorCusto { get; set; }
    public double ValorVenda { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.Today;
  }
}
