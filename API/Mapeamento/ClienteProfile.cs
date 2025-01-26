using API.Models;
using AutoMapper;
using Dominio.Entidades;

namespace API.Mapeamento {
  public class ClienteProfile : Profile {
    public ClienteProfile() {
      CreateMap<Cliente, ClienteDTO>();
      CreateMap<ClienteDTO, Cliente>();
    }
  }
}
