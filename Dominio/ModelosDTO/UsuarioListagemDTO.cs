using Dominio.Enumeradores;

namespace Dominio.ModelosDTO {
  public class UsuarioListagemDTO {
    public int Id { get; set; }
    public string Email { get; set; }
    public StatusUsuario Status { get; set; } = StatusUsuario.Ativo;
    public string Perfil { get; set; } = PerfilUsuario.Padrao.ToString();
  }
}
