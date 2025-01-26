using API.Models;
using AutoMapper;
using Dominio.Entidades;

namespace API.Mapeamento {
  public class UsuarioProfile : Profile {
    public UsuarioProfile() {
      CreateMap<Usuario, UsuarioLoginDTO>();
      CreateMap<UsuarioLoginDTO, Usuario>();
    }
  }
}
