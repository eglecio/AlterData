using API.Models;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Interfaces;
using Dominio.Servicos;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace API.Controllers {

  [ApiController]
  [Route("[controller]")]
  public class UsuarioController : ControllerBase {

    private readonly ILogger<Usuario> _logger;
    private readonly IRepositorio<Usuario> _repositorio;
    private readonly IValidator<UsuarioLoginDTO> _validator;
    private readonly IMapper _mapper;

    public UsuarioController(
      ILogger<Usuario> logger,
      IRepositorio<Usuario> repositorio,
      IValidator<UsuarioLoginDTO> validator,
      IMapper mapper) {

      _logger = logger;
      _mapper = mapper;
      _repositorio = repositorio;
      _validator = validator;
    }


    // POST: usuario/login
    // <snippet_Create>
    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UsuarioLoginDTO modelo) {
      var validacao = _validator.Validate(modelo);
      if (!validacao.IsValid)
        return BadRequest(validacao.Errors);

      try {
        var entidade = await _repositorio.ObterAsync(x => x.Email == modelo.Login && x.Senha == modelo.Senha && !x.Excluido && (x.DataInativacao == null || x.DataInativacao > DateTime.UtcNow.Date));
        if (entidade == null) return NotFound();

        var claimsPrincipal = new ClaimsPrincipal(
          new ClaimsIdentity(new[] {
            new Claim("Id", entidade.Id.ToString()),
            new Claim(ClaimTypes.Email, entidade.Email),
            new Claim(ClaimTypes.Role, entidade.Perfil.ToString())
          },
          BearerTokenDefaults.AuthenticationScheme)
        );

        return SignIn(claimsPrincipal);
      }
      catch (RepositorioException) {
        return NotFound();
      }

    }
    // </snippet_Create>



    //public IActionResult Index() {
    //  return View();
    //}
  }
}
