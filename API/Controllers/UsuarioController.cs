using AutoMapper;
using Dominio.Entidades;
using Dominio.Interfaces;
using Dominio.ModelosDTO;
using Dominio.Servicos;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers {

  /// <summary>
  /// Controlador responsável pelo gerenciamento de usuários no sistema.
  /// Requer autenticação para a maioria dos endpoints, exceto login.
  /// </summary>
  /// <remarks>
  /// Rota base: /Usuario
  /// Autenticação: Obrigatória (exceto para login)
  /// Operações suportadas:
  /// - Autenticação de usuários
  /// - Criação de novos usuários
  /// - Atualização de usuários existentes
  /// - Exclusão de usuários
  /// - Consulta de detalhes de usuários
  /// - Listagem de usuários com paginação e busca
  /// - Consulta de perfil do usuário autenticado
  /// </remarks>
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

    /// <summary>
    /// Realiza a autenticação do usuário no sistema.
    /// </summary>
    /// <param name="modelo">Credenciais do usuário</param>
    /// <returns>Token de autenticação</returns>
    /// <response code="200">Retorna o token de autenticação (.NET Core 8 Bearer Token)</response>
    /// <response code="400">Se as credenciais forem inválidas</response>
    /// <response code="404">Se o usuário não for encontrado ou estiver inativo</response>
    /// <remarks>
    /// Endpoint público que não requer autenticação prévia.
    /// Exemplo de requisição:
    /// 
    ///     POST /Usuario/login
    ///     {
    ///         "login": "usuario@exemplo.com",
    ///         "senha": "senhaSegura123"
    ///     }
    /// 
    /// O token retornado deve ser incluído no header Authorization das requisições subsequentes.
    /// </remarks>
    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Obtém o perfil do usuário atualmente autenticado.
    /// </summary>
    /// <returns>String indicando o perfil do usuário</returns>
    /// <response code="200">Retorna o perfil do usuário (ex: "Admin", "Editor", "Padrao")</response>
    /// <response code="401">Se o usuário não estiver autenticado</response>
    /// <remarks>
    /// Endpoint protegido que requer autenticação.
    /// 
    ///     GET /Usuario/perfil
    /// </remarks>
    [HttpGet]
    [Route("perfil")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<string> ObterPerfil() {
      return Ok(User?.FindFirst(ClaimTypes.Role)?.Value ?? "");
    }

    /// <summary>
    /// Adiciona um novo usuário ao sistema.
    /// </summary>
    /// <param name="modelo">Dados do novo usuário</param>
    /// <returns>Resultado da operação</returns>
    /// <response code="200">Se o usuário foi criado com sucesso</response>
    /// <response code="400">Se os dados forem inválidos ou o email já existir</response>
    /// <response code="401">Se o usuário não estiver autenticado</response>
    /// <response code="403">Se o usuário não tiver perfil de administrador</response>
    /// <remarks>
    /// Perfil Necessário: Admin
    /// Exemplo de requisição:
    /// 
    ///     POST /Usuario
    ///     {
    ///         "email": "novo@exemplo.com",
    ///         "senha": "senhaSegura123",
    ///         "perfil": "Padrao",
    ///         "status": "Ativo"
    ///     }
    /// 
    /// O email deve ser único no sistema.
    /// </remarks>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

    /// <summary>
    /// Atualiza os dados de um usuário existente.
    /// </summary>
    /// <param name="modelo">Dados atualizados do usuário</param>
    /// <returns>Resultado da operação</returns>
    /// <response code="200">Se o usuário foi atualizado com sucesso</response>
    /// <response code="400">Se os dados forem inválidos ou o email já existir</response>
    /// <response code="404">Se o usuário não for encontrado</response>
    /// <remarks>
    /// Perfil Necessário: Admin
    /// - Se o status for alterado para Inativo, a data de inativação será registrada
    /// - O email deve permanecer único no sistema
    /// 
    ///     PUT /Usuario
    ///     {
    ///         "id": 1,
    ///         "email": "atualizado@exemplo.com",
    ///         "senha": "novaSenha123",
    ///         "perfil": "Editor",
    ///         "status": "Ativo"
    ///     }
    /// </remarks>
    [HttpPut]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Remove um usuário do sistema.
    /// </summary>
    /// <param name="id">ID do usuário a ser removido</param>
    /// <returns>Resultado da operação</returns>
    /// <response code="200">Se o usuário foi removido com sucesso</response>
    /// <response code="404">Se o usuário não for encontrado</response>
    /// <remarks>
    /// Perfil Necessário: Admin
    /// 
    ///     DELETE /Usuario/{id}
    /// </remarks>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Obtém os dados de um usuário específico.
    /// </summary>
    /// <param name="id">ID do usuário a ser consultado</param>
    /// <returns>Dados do usuário</returns>
    /// <response code="200">Retorna os dados do usuário solicitado</response>
    /// <response code="404">Se o usuário não for encontrado</response>
    /// <remarks>
    /// Perfil Necessário: Admin
    /// 
    ///     GET /Usuario/{id}
    /// </remarks>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Lista usuários do sistema com paginação e busca opcional.
    /// </summary>
    /// <param name="pagina">Número da página (começa em 1)</param>
    /// <param name="totalPorPagina">Quantidade de registros por página</param>
    /// <param name="termo">Termo opcional para filtrar usuários por email</param>
    /// <returns>Lista paginada de usuários</returns>
    /// <response code="200">Retorna a lista de usuários</response>
    /// <response code="404">Se nenhum usuário for encontrado</response>
    /// <remarks>
    /// Perfil Necessário: Admin
    /// - O usuário logado é excluído da listagem para evitar auto-modificação
    /// - A busca é case-insensitive no email do usuário
    /// 
    ///     GET /Usuario/{pagina}/{totalPorPagina}/{termo?}
    /// </remarks>
    [Authorize(Roles = "Admin")]
    [HttpGet("{pagina}/{totalPorPagina}/{termo?}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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


  }
}
