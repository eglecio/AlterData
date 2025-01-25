using Dominio.Enumeradores;

namespace Dominio.Entidades {
  public class Cliente : EntidadeBase {

    /*
     * TODO: nao consta na documentacao que ocorrera cadastro de cliente PJ, fazendo mencao apenas
     * ao CPF, que leva a crer que sera apenas cadastro de PF.
     * Ao final do projeto vou validar ambos os casos para poder atingr todos os publicos. (Usando interface para control e manipulacao das entidades cliente PF e PJ)
     * 
    */

    public required string Nome { get; set; }
    public string CPF { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public string Endereco { get; set; } // Inicialmente irei deixar este campo unico, mas o correto sera implementar endereco completo (rua, numero, etc) em campos separados e em uma TB associativa onde o cliente pode ter "N" enderecos cadastrados...
    public string Observacao { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.Today;

  }
}
