using Dominio.Enumeradores;

namespace Dominio.ModelosDTO {
  public class UsuarioVisualizacaoDTO {
    public int Id { get; set; }
    public string Email { get; set; }
    public string DataInativacao { get; set; }
    public string DataCadastro { get; set; }
    public string? Senha { get; set; }
    public string PerfilDescricao { get; set; } = PerfilUsuario.Padrao.ToString();
    public string StatusDescricao { get; set; } = StatusUsuario.Ativo.ToString();
    public StatusUsuario Status { get; set; } = StatusUsuario.Ativo;
    public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Padrao;
  }
}
