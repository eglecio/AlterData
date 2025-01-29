using AutoMapper;
using Dominio.Entidades;
using Dominio.Extensoes;
using Dominio.ModelosDTO;

namespace API.Mapeamento {
  public class ProdutoProfile : Profile {
    public ProdutoProfile() {
      CreateMap<Produto, ProdutoDTO>();
      CreateMap<ProdutoDTO, Produto>()
            .ForMember(d => d.Observacao, o => o.MapFrom(p => p.Observacao ?? ""))
            .ForMember(d => d.DataCadastro, opt => opt.Ignore())
            .ForMember(d => d.Id, opt => opt.Ignore());

      CreateMap<Produto, ProdutoListagemDTO>();

      CreateMap<Produto, ProdutoVisualizacaoDTO>()
        .ForMember(d => d.ValorVenda, o => o.MapFrom(p => p.ValorVenda.ParaRealComSimbolo()))
        .ForMember(d => d.ValorCusto, o => o.MapFrom(p => p.ValorCusto.ParaRealComSimbolo()))
        .ForMember(d => d.QuantidadeEstoque, o => o.MapFrom(p => p.QuantidadeEstoque.ParaRealSemSimbolo()))
        .ForMember(d => d.DataCadastro, o => o.MapFrom(p => p.DataCadastro.ToString("dd/MM/yyyy")));

    }
  }
}
