using Dominio.Enumeradores;

namespace Dominio.Entidades {
  public class Usuario : EntidadeBase {
    public string Email { get; set; }
    public string Senha { get; set; }
    public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Padrao;
    public DateTime DataCadastro { get; set; } = DateTime.Today;
    public DateTime DataInativacao { get; set; } = DateTime.MaxValue;
  }
}
