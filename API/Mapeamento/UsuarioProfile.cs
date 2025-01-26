using AutoMapper;
using Dominio.Entidades;
using Dominio.ModelosDTO;

namespace API.Mapeamento {
  public class UsuarioProfile : Profile {
    public UsuarioProfile() {
      CreateMap<Usuario, UsuarioLoginDTO>();
      CreateMap<UsuarioLoginDTO, Usuario>();
    }
  }
}
