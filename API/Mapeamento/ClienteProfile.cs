using AutoMapper;
using Dominio.Entidades;
using Dominio.ModelosDTO;

namespace API.Mapeamento {
  public class ClienteProfile : Profile {
    public ClienteProfile() {
      CreateMap<Cliente, ClienteDTO>();
      CreateMap<ClienteDTO, Cliente>();
    }
  }
}
