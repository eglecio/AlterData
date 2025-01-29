using AutoMapper;
using Dominio.Entidades;
using Dominio.Helper;
using Dominio.ModelosDTO;

namespace API.Mapeamento {
  public class UsuarioProfile : Profile {
    public UsuarioProfile() {
      CreateMap<Usuario, UsuarioLoginDTO>();
      CreateMap<UsuarioLoginDTO, Usuario>();

      CreateMap<Usuario, UsuarioVisualizacaoDTO>()
        .ForMember(d => d.DataInativacao, o => o.MapFrom(p => p.DataInativacao.ToString() ?? ""))
        .ForMember(d => d.DataCadastro, o => o.MapFrom(p => p.DataCadastro.ToString("dd/MM/yyyy")))
        .ForMember(d => d.StatusDescricao, o => o.MapFrom(x => x.Status.ToString()))
        .ForMember(d => d.PerfilDescricao, o => o.MapFrom(x => HelperUsuario.DescricaoPerfil(x.Perfil)));

      CreateMap<Usuario, UsuarioListagemDTO>()
        .ForMember(d => d.Perfil, o => o.MapFrom(x => HelperUsuario.DescricaoPerfil(x.Perfil)));

      CreateMap<UsuarioDTO, Usuario>()
        .ForMember(d => d.DataInativacao, opt => opt.Ignore())
        .ForMember(d => d.DataCadastro, opt => opt.Ignore())
        .ForMember(d => d.Id, opt => opt.Ignore());
    }
  }
}
