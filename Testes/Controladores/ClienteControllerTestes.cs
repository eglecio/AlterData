using API.Controllers;
using AutoFixture;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Interfaces;
using Dominio.ModelosDTO;
using Dominio.Servicos;
using Dominio.Validacao;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace Testes.Controladores {
  public class ClienteControllerTestes {

    private readonly Mock<IRepositorio<Cliente>> _repositorio;
    private readonly Mock<IMapper> _mapper;
    private readonly ClienteController _controller;
    private readonly Fixture _fixture = new();
    private readonly ClienteValidador _validator = new();
    private readonly Mock<ILogger<Cliente>> _logger = new();

    public ClienteControllerTestes() {
      _repositorio = new Mock<IRepositorio<Cliente>>();
      _mapper = new Mock<IMapper>();
      _controller = new ClienteController(_logger.Object, _mapper.Object, _repositorio.Object, _validator);
    }

    [Fact]
    public async Task Add_QuandoDadosValidos_DeveRetornarOkComId() {
      var clienteDto = _fixture.Create<ClienteDTO>();
      var cliente = _fixture.Create<Cliente>();
      cliente.Nome = "Teste";// Para garantir que nao vai dar erro no validador...
      cliente.CPF = "009.261.539-27";// Para garantir que nao vai dar erro no validador...
      cliente.Email = "teste@teste.com";// Para garantir que nao vai dar erro no validador...

      _mapper.Setup(m => m.Map<Cliente>(clienteDto)).Returns(cliente);
      _repositorio.Setup(r => r.AdicionarAsync(cliente)).Returns(Task.FromResult(cliente));

      var resultado = await _controller.Add(clienteDto);

      var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
      Assert.Equal(cliente.Id, okResult.Value);
      _repositorio.Verify(r => r.AdicionarAsync(cliente), Times.Once);
    }

    [Fact]
    public async Task Update_QuandoClienteExiste_DeveRetornarOk() {
      var clienteDto = _fixture.Create<ClienteDTO>();
      var clienteExistente = _fixture.Create<Cliente>();
      clienteExistente.Id = clienteDto.Id;
      clienteExistente.Nome = "Teste";// Para garantir que nao vai dar erro no validador...
      clienteExistente.CPF = "009.261.539-27";// Para garantir que nao vai dar erro no validador...
      clienteExistente.Email = "teste@teste.com";// Para garantir que nao vai dar erro no validador...

      _repositorio.Setup(r => r.ObterPorIdAsync(clienteDto.Id)).ReturnsAsync(clienteExistente);
      _mapper.Setup(m => m.Map(clienteDto, clienteExistente)).Returns(clienteExistente);

      var resultado = await _controller.Update(clienteDto);

      Assert.IsType<OkResult>(resultado.Result);
      _repositorio.Verify(r => r.AtualizarAsync(clienteExistente), Times.Once);
    }

    [Fact]
    public async Task Delete_QuandoClienteExiste_DeveRetornarOk() {
      var cliente = _fixture.Create<Cliente>();
      _repositorio.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);

      var resultado = await _controller.Delete(cliente.Id);

      Assert.IsType<OkResult>(resultado.Result);
      _repositorio.Verify(r => r.RemoverAsync(cliente), Times.Once);
    }

    [Fact]
    public async Task Get_QuandoClienteNaoExiste_DeveRetornarNotFound() {
      _repositorio.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ThrowsAsync(new RepositorioException("Cliente não encontrado"));

      var resultado = await _controller.Get(1);
      Assert.IsType<NotFoundResult>(resultado.Result);
    }

    [Theory]
    [InlineData(1, 10, "teste")]
    [InlineData(1, 10, "")]
    [InlineData(1, 12, " ")]
    public async Task Get_QuandoExistemClientes_DeveRetornarPaginado(int pagina, int totalPorPagina, string termo) {
      var clientes = _fixture.Build<Cliente>().With(a => a.Excluido, false).CreateMany(15).ToList();
      _repositorio.Setup(r => r.BuscarPaginadoAsync(It.IsAny<Expression<Func<Cliente, bool>>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Cliente, object>>?>()))
        .ReturnsAsync(clientes);

      var resultado = await _controller.Get(pagina, totalPorPagina, termo);

      var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
      var clientesRetornados = Assert.IsAssignableFrom<IEnumerable<Cliente>>(okResult.Value);
      Assert.Equal(totalPorPagina, clientesRetornados.Count());
    }

    [Fact]
    public async Task Add_QuandoDadosInvalidos_DeveRetornarBadRequest() {
      var clienteDto = new ClienteDTO();
      // Dados inválidos propositalmente para estourar erro no controlador atraves do validador...
      var cliente = new Cliente {
        Nome = " ",
        CPF = "555",
        Email = "teste.com"
      };

      _mapper.Setup(m => m.Map<Cliente>(clienteDto)).Returns(cliente);

      var resultado = await _controller.Add(clienteDto);

      var badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
      Assert.NotNull(badRequest.Value);
    }


  }
}
