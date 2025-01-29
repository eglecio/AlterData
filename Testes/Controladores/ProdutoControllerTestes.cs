using API.Controllers;
using AutoFixture;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Interfaces;
using Dominio.ModelosDTO;
using Dominio.Servicos;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace Testes.Controladores {
  public class ProdutoControllerTestes {
    private readonly Mock<ILogger<Produto>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositorio<Produto>> _repositorioMock;
    private readonly Mock<IValidator<Produto>> _validadorProdutoMock;
    private readonly Fixture _fixture;
    private readonly ProdutoController _controller;

    public ProdutoControllerTestes() {
      _loggerMock = new Mock<ILogger<Produto>>();
      _mapperMock = new Mock<IMapper>();
      _repositorioMock = new Mock<IRepositorio<Produto>>();
      _validadorProdutoMock = new Mock<IValidator<Produto>>();
      _fixture = new Fixture();
      _controller = new ProdutoController(
          _loggerMock.Object,
          _mapperMock.Object,
          _repositorioMock.Object,
          _validadorProdutoMock.Object
      );
    }

    #region Testes do método Add

    [Fact]
    public async Task Add_QuandoDadosValidos_DeveRetornarOk() {
      var produtoDto = _fixture.Create<ProdutoDTO>();
      var produto = _fixture.Create<Produto>();

      _mapperMock.Setup(m => m.Map<Produto>(produtoDto)).Returns(produto);
      _validadorProdutoMock.Setup(v => v.Validate(produto)).Returns(new ValidationResult());

      var resultado = await _controller.Add(produtoDto);

      var okResult = Assert.IsType<OkResult>(resultado.Result);
      Assert.Equal(200, okResult.StatusCode);
      _repositorioMock.Verify(r => r.AdicionarAsync(produto), Times.Once);
    }

    [Fact]
    public async Task Add_QuandoValidacaoFalha_DeveRetornarBadRequest() {
      var produtoDto = _fixture.Create<ProdutoDTO>();
      var produto = _fixture.Create<Produto>();
      var validacaoDeFalhas = new List<ValidationFailure> { new("Nome", "Nome é obrigatório") };

      _mapperMock.Setup(m => m.Map<Produto>(produtoDto)).Returns(produto);
      _validadorProdutoMock.Setup(v => v.Validate(produto)).Returns(new ValidationResult(validacaoDeFalhas));

      var resultado = await _controller.Add(produtoDto);

      var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task Add_QuandoExcecaoLancada_DeveRetornarProblem() {
      var produtoDto = _fixture.Create<ProdutoDTO>();
      _mapperMock.Setup(m => m.Map<Produto>(It.IsAny<ProdutoDTO>())).Throws(new Exception("Erro ao mapear"));

      var resultado = await _controller.Add(produtoDto);

      Assert.IsType<ObjectResult>(resultado.Result);
      _loggerMock.Verify(
          x => x.Log(
              LogLevel.Error,
              It.IsAny<EventId>(),
              It.Is<It.IsAnyType>((v, t) => true),
              It.IsAny<Exception>(),
              It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
          Times.Once);
    }
    #endregion

    #region Testes do método Update


    [Fact]
    public async Task Update_QuandoDadosValidos_DeveRetornarOk() {
      var produtoDto = _fixture.Create<ProdutoDTO>();
      var produtoExistente = _fixture.Create<Produto>();

      _repositorioMock.Setup(r => r.ObterPorIdAsync(produtoDto.Id)).ReturnsAsync(produtoExistente);
      _validadorProdutoMock.Setup(v => v.Validate(produtoExistente)).Returns(new ValidationResult());

      var resultado = await _controller.Update(produtoDto);

      var okResult = Assert.IsType<OkResult>(resultado.Result);
      Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task Update_QuandoProdutoNaoEncontrado_DeveRetornarNotFound() {
      var produtoDto = _fixture.Create<ProdutoDTO>();
      _repositorioMock.Setup(r => r.ObterPorIdAsync(produtoDto.Id)).ThrowsAsync(new RepositorioException("Falha nos testes"));

      var resultado = await _controller.Update(produtoDto);
      Assert.IsType<NotFoundResult>(resultado.Result);
    }

    #endregion

    #region Testes do método Delete

    [Fact]
    public async Task Delete_QuandoProdutoExiste_DeveRetornarOk() {
      var produto = _fixture.Create<Produto>();
      _repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(produto);

      var resultado = await _controller.Delete(1);

      Assert.IsType<OkResult>(resultado.Result);
    }

    [Fact]
    public async Task Delete_QuandoProdutoNaoExiste_DeveRetornarNotFound() {
      _repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
          .ThrowsAsync(new RepositorioException("Falha nos testes"));

      var resultado = await _controller.Delete(1);

      Assert.IsType<NotFoundResult>(resultado.Result);
    }

    #endregion

    #region Testes do método Get (por ID)

    [Fact]
    public async Task Get_QuandoProdutoExiste_DeveRetornarOkComProduto() {
      var produto = _fixture.Create<Produto>();
      var produtoDto = _fixture.Create<ProdutoVisualizacaoDTO>();

      _repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(produto);
      _mapperMock.Setup(m => m.Map<ProdutoVisualizacaoDTO>(produto)).Returns(produtoDto);

      var resultado = await _controller.Get(1);

      var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
      Assert.Equal(produtoDto, okResult.Value);
    }

    [Fact]
    public async Task Get_QuandoProdutoNaoExiste_DeveRetornarNotFound() {
      _repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
          .ThrowsAsync(new RepositorioException("Falha nos testes"));

      var resultado = await _controller.Get(1);
      Assert.IsType<NotFoundResult>(resultado.Result);
    }

    #endregion

    #region Testes do método Get (Listagem)

    [Fact]
    public async Task GetLista_QuandoExistemProdutos_DeveRetornarListaPaginada() {
      // Arrange
      var produtos = _fixture.CreateMany<Produto>(3).ToList();
      var produtosDto = _fixture.CreateMany<ProdutoListagemDTO>(3).ToList();

      _repositorioMock.Setup(r => r.BuscarPaginadoAsync(
              It.IsAny<Expression<Func<Produto, bool>>>(),
              It.IsAny<int>(),
              It.IsAny<int>(),
              It.IsAny<Expression<Func<Produto, object>>>()))
          .ReturnsAsync(produtos);

      _mapperMock.Setup(m => m.Map<List<ProdutoListagemDTO>>(It.IsAny<List<Produto>>()))
        .Returns(produtosDto);

      var resultado = await _controller.Get(1, 10);

      var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
      var listaProdutos = Assert.IsType<List<ProdutoListagemDTO>>(okResult.Value);
      Assert.Equal(produtosDto.Count, listaProdutos.Count);
    }

    [Fact]
    public async Task GetLista_QuandoUsaTermoDeBusca_DeveRetornarListaFiltrada() {
      var produtos = _fixture.CreateMany<Produto>(2).ToList();
      var produtosDto = _fixture.CreateMany<ProdutoListagemDTO>(2).ToList();

      _repositorioMock.Setup(r => r.BuscarPaginadoAsync(
              It.IsAny<Expression<Func<Produto, bool>>>(),
              It.IsAny<int>(),
              It.IsAny<int>(),
              It.IsAny<Expression<Func<Produto, object>>>()))
          .ReturnsAsync(produtos);

      _mapperMock.Setup(m => m.Map<List<ProdutoListagemDTO>>(It.IsAny<List<Produto>>()))
          .Returns(produtosDto);

      var resultado = await _controller.Get(1, 10, "termo");

      var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
      var listaProdutos = Assert.IsType<List<ProdutoListagemDTO>>(okResult.Value);
      Assert.Equal(produtosDto.Count, listaProdutos.Count);
    }

    [Fact]
    public async Task GetLista_QuandoOcorreErro_DeveRetornarNotFound() {
      _repositorioMock.Setup(r => r.BuscarPaginadoAsync(
              It.IsAny<Expression<Func<Produto, bool>>>(),
              It.IsAny<int>(),
              It.IsAny<int>(),
              It.IsAny<Expression<Func<Produto, object>>>()))
          .ThrowsAsync(new RepositorioException("falha nos testes"));

      var resultado = await _controller.Get(1, 10);
      Assert.IsType<NotFoundResult>(resultado.Result);
    }

    #endregion
  
  }
}
