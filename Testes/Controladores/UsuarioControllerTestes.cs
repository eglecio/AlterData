using API.Controllers;
using AutoFixture;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Interfaces;
using Dominio.ModelosDTO;
using Dominio.Servicos;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;
using Dominio.Enumeradores;

namespace Testes.Controladores {

  public class UsuarioControllerTestes {

    private readonly Mock<ILogger<Usuario>> _loggerMock;
    private readonly Mock<IRepositorio<Usuario>> _repositorioMock;
    private readonly Mock<IValidator<UsuarioLoginDTO>> _validadorUsuarioLoginMock;
    private readonly Mock<IValidator<Usuario>> _validatorUsuarioMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Fixture _fixture;
    private readonly UsuarioController _controller;

    public UsuarioControllerTestes() {
      _fixture = new Fixture();
      _loggerMock = new Mock<ILogger<Usuario>>();
      _repositorioMock = new Mock<IRepositorio<Usuario>>();
      _validadorUsuarioLoginMock = new Mock<IValidator<UsuarioLoginDTO>>();
      _validatorUsuarioMock = new Mock<IValidator<Usuario>>();
      _mapperMock = new Mock<IMapper>();

      _controller = new UsuarioController(
          _loggerMock.Object,
          _repositorioMock.Object,
          _validadorUsuarioLoginMock.Object,
          _mapperMock.Object,
          _validatorUsuarioMock.Object
      );
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_RetornaSignIn() {
      var loginDto = _fixture.Create<UsuarioLoginDTO>();
      var usuario = _fixture.Build<Usuario>().With(a => a.Status, StatusUsuario.Ativo).Create();

      _validadorUsuarioLoginMock.Setup(x => x.Validate(It.IsAny<UsuarioLoginDTO>())).Returns(new ValidationResult());

      _repositorioMock.Setup(x => x.ObterAsync(It.IsAny<Expression<Func<Usuario, bool>>>())).ReturnsAsync(usuario);

      var resultado = await _controller.Login(loginDto);
      var signInResult = Assert.IsType<SignInResult>(resultado);
      Assert.NotNull(signInResult);
    }

    [Fact]
    public async Task Login_ComCredenciaisInvalidas_RetornaNotFound() {
      var usuarioLoginDTO = _fixture.Create<UsuarioLoginDTO>();

      _validadorUsuarioLoginMock.Setup(x => x.Validate(It.IsAny<UsuarioLoginDTO>()))
          .Returns(new ValidationResult());

      _repositorioMock.Setup(x => x.ObterAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
          .ReturnsAsync((Usuario)null);

      var resultado = await _controller.Login(usuarioLoginDTO);
      Assert.IsType<NotFoundResult>(resultado);
    }

    [Fact]
    public async Task Login_ComValidacaoInvalida_RetornaBadRequest() {
      var usuarioLoginDTO = _fixture.Create<UsuarioLoginDTO>();
      var validationFailures = new List<ValidationFailure> { new("Login", "Login é obrigatório") };

      _validadorUsuarioLoginMock.Setup(x => x.Validate(It.IsAny<UsuarioLoginDTO>())).Returns(new ValidationResult(validationFailures));

      var resultado = await _controller.Login(usuarioLoginDTO);
      Assert.IsType<BadRequestObjectResult>(resultado);
    }

    [Fact]
    public async Task Add_ComDadosValidos_RetornaOk() {
      var usuarioDto = _fixture.Create<UsuarioDTO>();
      var usuario = _fixture.Create<Usuario>();

      _mapperMock.Setup(x => x.Map<Usuario>(It.IsAny<UsuarioDTO>())).Returns(usuario);
      _validatorUsuarioMock.Setup(x => x.Validate(It.IsAny<Usuario>())).Returns(new ValidationResult());

      // como o retorno eh valor nulo o VS identifica como sendo errada a instrucao abaixo, marcando com sublinhado como advertencia, mas eh isso mesmo que quermos no teste...
      _repositorioMock.Setup(x => x.ObterAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
          .ReturnsAsync((Usuario)null);

      var resultado = await _controller.Add(usuarioDto);
      Assert.IsType<OkResult>(resultado);
    }

    [Fact]
    public async Task Add_ComEmailDuplicado_RetornaBadRequest() {
      var usuarioDto = _fixture.Create<UsuarioDTO>();
      var usuario = _fixture.Create<Usuario>();

      _mapperMock.Setup(x => x.Map<Usuario>(It.IsAny<UsuarioDTO>())).Returns(usuario);

      _validatorUsuarioMock.Setup(x => x.Validate(It.IsAny<Usuario>()))
          .Returns(new ValidationResult());

      _repositorioMock.Setup(x => x.ObterAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
          .ReturnsAsync(usuario);

      var resultado = await _controller.Add(usuarioDto);

      var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
      var errors = Assert.IsType<List<ValidationFailure>>(badRequestResult.Value);
      Assert.Contains(errors, e => e.ErrorMessage == "E-mail já cadastrado na base de dados!");
    }

    [Theory]
    [InlineData(1, 10, "")]
    [InlineData(1, 10, "test@email.com")]
    public async Task Get_ComParametrosValidos_RetornaLista(int pagina, int totalPorPagina, string termo) {
      var usuarios = _fixture.CreateMany<Usuario>(5).ToList();
      var usuariosDTO = _fixture.CreateMany<UsuarioListagemDTO>(5).ToList();

      _repositorioMock.Setup(x => x.BuscarPaginadoAsync(
          It.IsAny<Expression<Func<Usuario, bool>>>(),
          It.IsAny<int>(),
          It.IsAny<int>(),
          It.IsAny<Expression<Func<Usuario, object>>>()))
          .ReturnsAsync(usuarios);

      _mapperMock.Setup(x => x.Map<List<UsuarioListagemDTO>>(It.IsAny<List<Usuario>>()))
          .Returns(usuariosDTO);

      // Simula claims do usuário logado...
      var claims = new List<Claim> { new Claim("Id", "1") };
      var identity = new ClaimsIdentity(claims);
      var claimsPrincipal = new ClaimsPrincipal(identity);
      _controller.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext { User = claimsPrincipal }
      };

      var resultado = await _controller.Get(pagina, totalPorPagina, termo);

      var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
      var returnValue = Assert.IsType<List<UsuarioListagemDTO>>(okResult.Value);
      Assert.Equal(usuariosDTO.Count, returnValue.Count);
    }

    [Fact]
    public async Task Update_ComDadosValidos_RetornaOk() {
      var usuarioDto = _fixture.Create<UsuarioDTO>();
      var usuarioExistente = _fixture.Create<Usuario>();

      _repositorioMock.Setup(x => x.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(usuarioExistente);

      _validatorUsuarioMock.Setup(x => x.Validate(It.IsAny<Usuario>())).Returns(new ValidationResult());

      // como o retorno eh valor nulo o VS identifica como sendo errada a instrucao abaixo, marcando com sublinhado como advertencia, mas eh isso mesmo que quermos no teste...
      _repositorioMock.Setup(x => x.ObterAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
          .ReturnsAsync((Usuario)null);

      var resultado = await _controller.Update(usuarioDto);
      Assert.IsType<OkResult>(resultado);
    }

    [Fact]
    public async Task Delete_ComIdValido_RetornaOk() {
      var usuario = _fixture.Create<Usuario>();

      _repositorioMock.Setup(x => x.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(usuario);

      var resultado = await _controller.Delete(1);
      Assert.IsType<OkResult>(resultado);
    }

    [Fact]
    public async Task Delete_ComIdInvalido_RetornaNotFound() {
      _repositorioMock.Setup(x => x.ObterPorIdAsync(It.IsAny<int>())).ThrowsAsync(new RepositorioException("Falha no repositorio"));

      var resultado = await _controller.Delete(999);
      Assert.IsType<NotFoundResult>(resultado);
    }

  }
}
