﻿using AutoMapper;
using Dominio.Entidades;
using Dominio.ModelosDTO;

namespace API.Mapeamento {
  public class ClienteProfile : Profile {
    public ClienteProfile() {
      CreateMap<Cliente, ClienteDTO>();
      CreateMap<ClienteDTO, Cliente>()
            .ForMember(d => d.Endereco, o => o.MapFrom(p => p.Endereco ?? ""))
            .ForMember(d => d.Telefone, o => o.MapFrom(p => p.Telefone ?? ""))
            .ForMember(d => d.Observacao, o => o.MapFrom(p => p.Observacao ?? ""))
            .ForMember(d => d.DataCadastro, opt => opt.Ignore())
            .ForMember(d => d.Id, opt => opt.Ignore());

      CreateMap<Cliente, ClienteListagemDTO>();

      CreateMap<Cliente, ClienteVisualizacaoDTO>()
        .ForMember(d => d.DataCadastro, o => o.MapFrom(p => p.DataCadastro.ToString("dd/MM/yyyy")));

    }
  }
}
