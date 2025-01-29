using Dominio.Enumeradores;

namespace Dominio.ModelosDTO {
  public class UsuarioDTO {
    public int Id { get; set; }
    public string Email { get; set; }
    public string? Senha { get; set; }
    public StatusUsuario Status { get; set; } = StatusUsuario.Ativo;
    public PerfilUsuario? Perfil { get; set; } = PerfilUsuario.Padrao;
    
  }
}
