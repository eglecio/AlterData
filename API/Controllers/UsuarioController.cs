using AutoMapper;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Interfaces;
using Dominio.ModelosDTO;
using Dominio.Servicos;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security;
using System.Security.Claims;


namespace API.Controllers {

  [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class UsuarioController : ControllerBase {

    private readonly ILogger<Usuario> _logger;
    private readonly IRepositorio<Usuario> _repositorio;
    private readonly IValidator<UsuarioLoginDTO> _validator;
    private readonly IValidator<Usuario> _validatorUsuario;
    private readonly IMapper _mapper;

    public UsuarioController(
      ILogger<Usuario> logger,
      IRepositorio<Usuario> repositorio,
      IValidator<UsuarioLoginDTO> validator,
      IMapper mapper,
      IValidator<Usuario> validatorUsuario) {

      _logger = logger;
      _mapper = mapper;
      _repositorio = repositorio;
      _validator = validator;
      _validatorUsuario = validatorUsuario;
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
        var entidade = await _repositorio.ObterAsync(x => 
                        x.Email == modelo.Login 
                        && x.Senha == modelo.Senha 
                        && !x.Excluido
                        && x.Status == Dominio.Enumeradores.StatusUsuario.Ativo);

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



    // POST: usuario/validarToken
    // <snippet_Create>
    [HttpGet]
    [Route("validarToken")]
    public ActionResult<bool> ValidarToken() {
      // Necessario essa validacao extra, pois infelizmente no interceptador do axios esta dando erro de rede quando recebe http 401 entre outros codigos de retorno...
      return Ok(User?.Identity?.IsAuthenticated ?? false);
    }
    // </snippet_Create>


    // GET: usuario/perfil
    /// <summary>
    /// Obtém o perfil do usuario autenticao, para bloquear ou liberar funcoes no app...
    /// Todos os perfis de usuário tem acesso a este metodo.
    /// </summary>
    /// <returns>Um núumero inteiro.</returns>
    [HttpGet]
    [Route("perfil")]
    public ActionResult<string> ObterPerfil() {
      return Ok(User?.FindFirst(ClaimTypes.Role)?.Value ?? "");
    }
    // </snippet_Create>



    // POST: usuario
    // <snippet_Create>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Add(UsuarioDTO modelo) {
      try {
        var entidade = _mapper.Map<Usuario>(modelo);

        var validacao = _validatorUsuario.Validate(entidade);
        if (!validacao.IsValid)
          return BadRequest(validacao.Errors);

        // Verifica se o email ja esta vinculada a algum outro usuario...
        var usuarioComEmailJaExiste = await _repositorio.ObterAsync(x => x.Email == modelo.Email.ToLower() && !x.Excluido);
        if (usuarioComEmailJaExiste != null) {
          validacao.Errors.Add(new ValidationFailure { ErrorMessage = "E-mail já cadastrado na base de dados!" });
          return BadRequest(validacao.Errors);
        }

        await _repositorio.AdicionarAsync(entidade);
      }
      catch (Exception ex) {
        _logger.LogError($"Add usuario: {ex.Message + " - inner execption: " + ex.InnerException}");
        return Problem();
      }

      return Ok();
    }
    // </snippet_Create>

    // PUT: usuario
    // <snippet_Create>
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(UsuarioDTO modelo) {
      try {
        var entidadeExistente = await _repositorio.ObterPorIdAsync(modelo.Id);
        var inativadoAgora = modelo.Status == Dominio.Enumeradores.StatusUsuario.Inativo && entidadeExistente.Status == Dominio.Enumeradores.StatusUsuario.Ativo;
        _mapper.Map(modelo, entidadeExistente);

        var validacao = _validatorUsuario.Validate(entidadeExistente);
        if (!validacao.IsValid)
          return BadRequest(validacao.Errors);

        if (inativadoAgora) {
          entidadeExistente.DataInativacao = DateTime.Now;
        }

        // Verifica se trocou o email e se o novo ja vinculado a algum outro usuario...
        var usuarioComEmailJaExiste = await _repositorio.ObterAsync(x => x.Email == modelo.Email.ToLower() && !x.Excluido);
        if (usuarioComEmailJaExiste != null && usuarioComEmailJaExiste.Id != entidadeExistente.Id) {
          validacao.Errors.Add(new ValidationFailure { ErrorMessage = "E-mail já cadastrado na base de dados!" });
          return BadRequest(validacao.Errors);
        }

        await _repositorio.AtualizarAsync(entidadeExistente);
      }
      catch (RepositorioException) {
        return NotFound();
      }

      return Ok();
    }
    // </snippet_Create>


    // DELETE: usuario
    // <snippet_Create>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id) {
      try {
        var usuarioExistente = await _repositorio.ObterPorIdAsync(id);
        await _repositorio.RemoverAsync(usuarioExistente);
      }
      catch (RepositorioException) {
        return NotFound();
      }

      return Ok();
    }
    // </snippet_Create>


    // GET: usuario
    // <snippet_Create>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UsuarioVisualizacaoDTO>> Get(int id) {
      try {
        var usuario = await _repositorio.ObterPorIdAsync(id);
        var modelo = _mapper.Map<UsuarioVisualizacaoDTO>(usuario);

        return Ok(modelo);
      }
      catch (RepositorioException) {
        return NotFound();
      }
      catch (Exception ex) {
        _logger.LogError($"Obter usuario visualizar: {ex.Message + " - inner execption: " + ex.InnerException}");
        return Problem();
      }
    }
    // </snippet_Create>


    // GET: usuarios
    // <snippet_Create>
    [Authorize(Roles = "Admin")]
    [HttpGet("{pagina}/{totalPorPagina}/{termo?}")]
    //[HttpGet("{pagina}/{totalPorPagina}")]
    public async Task<ActionResult<IEnumerable<UsuarioListagemDTO>>> Get(int pagina = 1, int totalPorPagina = 10, string termo = "") {
      try {
        var usuarioLogadoId = int.Parse(User.FindFirst("Id")?.Value ?? "0");// removo da listagem, para evitar que ele cometa erros com o seu proprio usuario...
        IEnumerable<Usuario> usuarios;
        if (!string.IsNullOrEmpty(termo.Trim())) {
          usuarios = await _repositorio.BuscarPaginadoAsync(x => !x.Excluido && EF.Functions.ILike(x.Email, $"%{termo}%") && x.Id != usuarioLogadoId, pagina, totalPorPagina, z => z.Id);
        }
        else {
          usuarios = await _repositorio.BuscarPaginadoAsync(x => !x.Excluido && x.Id != usuarioLogadoId, pagina, totalPorPagina, z => z.Id);
        }

        return Ok(_mapper.Map<List<UsuarioListagemDTO>>(usuarios
           .Skip((pagina - 1) * totalPorPagina)
           .Take(totalPorPagina).ToList()));
      }
      catch (RepositorioException) {
        return NotFound();
      }
      catch (Exception ex) {
        _logger.LogError($"Listagem de usuarios: {ex.Message + " - inner execption: " + ex.InnerException}");
        return Problem();
      }
    }
    // </snippet_Create>

  }
}
