using AutoMapper;
using Dominio.Entidades;
using Dominio.Interfaces;
using Dominio.ModelosDTO;
using Dominio.Servicos;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers {

  /// <summary>
  /// Controlador responsável pelo gerenciamento de operações relacionadas a clientes no sistema.
  /// Requer autenticação para todos os endpoints.
  /// </summary>
  /// <remarks>
  /// Rota base: /Cliente
  /// Autenticação: Obrigatória
  /// Operações suportadas:
  /// - Criação de novos clientes
  /// - Atualização de clientes existentes
  /// - Exclusão de clientes
  /// - Consulta de detalhes de clientes
  /// - Listagem de clientes com paginação e busca
  /// - Obtenção de estatísticas para dashboard
  /// </remarks>
  [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class ClienteController : ControllerBase {
    private readonly ILogger<Cliente> _logger;
    private readonly IMapper _mapper;
    private readonly IRepositorio<Cliente> _repositorio;
    private readonly IValidator<Cliente> _validator;

    public ClienteController(
      ILogger<Cliente> logger,
      IMapper mapper,
      IRepositorio<Cliente> repositorio,
      IValidator<Cliente> validator) {

      _logger = logger;
      _mapper = mapper;
      _repositorio = repositorio;
      _validator = validator;
    }


    /// <summary>
    /// Adiciona um novo cliente no sistema.
    /// </summary>
    /// <param name="modelo">Objeto DTO contendo as informações do cliente</param>
    /// <returns>O ID do cliente recém-criado</returns>
    /// <response code="200">Retorna o ID do cliente criado</response>
    /// <response code="400">Se os dados do cliente forem inválidos</response>
    /// <response code="500">Se ocorrer um erro interno durante o processamento</response>
    /// <remarks>
    /// Perfil Necessário: Editor ou Admin
    /// Exemplo de requisição:
    /// 
    ///     POST /Cliente
    ///     {
    ///         "nome": "João Silva",
    ///         "email": "joao@exemplo.com",
    ///         "telefone": "11999999999"
    ///     }
    /// </remarks>
    [HttpPost]
    [Authorize(Roles = "Editor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ClienteDTO>> Add(ClienteDTO modelo) {
      var entidade = _mapper.Map<Cliente>(modelo);

      var validacao = _validator.Validate(entidade);
      if (!validacao.IsValid)
        return BadRequest(validacao.Errors);

      try {
        await _repositorio.AdicionarAsync(entidade);
      }
      catch (Exception ex) {
        _logger.LogError($"Add cliente: {ex.Message + " - inner execption: " + ex.InnerException}");
        return Problem();
      }

      return Ok(entidade.Id);
    }

    /// <summary>
    /// Atualiza as informações de um cliente existente.
    /// </summary>
    /// <param name="modelo">Objeto DTO contendo as informações atualizadas do cliente</param>
    /// <returns>Sem conteúdo em caso de sucesso</returns>
    /// <response code="200">Se o cliente foi atualizado com sucesso</response>
    /// <response code="400">Se os dados atualizados do cliente forem inválidos</response>
    /// <response code="404">Se o cliente não for encontrado</response>
    /// <remarks>
    /// Perfil Necessário: Editor ou Admin
    /// Exemplo de requisição:
    /// 
    ///     PUT /Cliente
    ///     {
    ///         "id": 1,
    ///         "nome": "João Silva Atualizado",
    ///         "email": "joao.atual@exemplo.com",
    ///         "telefone": "11988888888"
    ///     }
    /// </remarks>
    [HttpPut]
    [Authorize(Roles = "Editor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDTO>> Update(ClienteDTO modelo) {
      try {
        var clienteExistente = await _repositorio.ObterPorIdAsync(modelo.Id);
        _mapper.Map(modelo, clienteExistente);// Mapea propriedades, ignorando DataCadastro e ID...

        var validacao = _validator.Validate(clienteExistente);
        if (!validacao.IsValid)
          return BadRequest(validacao.Errors);

        await _repositorio.AtualizarAsync(clienteExistente);
      }
      catch (RepositorioException) {
        return NotFound();
      }

      return Ok();
    }

    /// <summary>
    /// Remove um cliente do sistema.
    /// </summary>
    /// <param name="clienteId">O ID do cliente a ser removido</param>
    /// <returns>Sem conteúdo em caso de sucesso</returns>
    /// <response code="200">Se o cliente foi removido com sucesso</response>
    /// <response code="404">Se o cliente não foi encontrado</response>
    /// <remarks>
    /// Perfil Necessário: Editor ou Admin
    /// 
    ///     DELETE /Cliente/{clienteId}
    /// </remarks>
    [HttpDelete("{clienteId}")]
    [Authorize(Roles = "Editor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDTO>> Delete(int clienteId) {
      try {
        var clienteExistente = await _repositorio.ObterPorIdAsync(clienteId);
        await _repositorio.RemoverAsync(clienteExistente);
      }
      catch (RepositorioException) {
        return NotFound();
      }

      return Ok();
    }

    /// <summary>
    /// Recupera os detalhes de um cliente específico pelo ID.
    /// </summary>
    /// <param name="clienteId">O ID do cliente a ser consultado</param>
    /// <returns>Os detalhes do cliente</returns>
    /// <response code="200">Retorna as informações do cliente solicitado</response>
    /// <response code="404">Se o cliente não foi encontrado</response>
    /// <remarks>
    /// Perfil Necessário: Padrão, Editor ou Admin
    /// 
    ///     GET /Cliente/{clienteId}
    /// </remarks>
    [HttpGet("{clienteId}")]
    [Authorize(Roles = "Padrao,Editor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDTO>> Get(int clienteId) {
      try {
        var cliente = await _repositorio.ObterPorIdAsync(clienteId);
        var modelo = _mapper.Map<ClienteVisualizacaoDTO>(cliente);
        return Ok(modelo);
      }
      catch (RepositorioException) {
        return NotFound();
      }
    }

    /// <summary>
    /// Recupera uma lista paginada de clientes com funcionalidade de busca opcional.
    /// </summary>
    /// <param name="pagina">Número da página (padrão: 1)</param>
    /// <param name="totalPorPagina">Itens por página (padrão: 10)</param>
    /// <param name="termo">Termo de busca opcional para filtrar clientes por nome, email ou telefone</param>
    /// <returns>Uma lista de clientes que correspondem aos critérios de busca</returns>
    /// <response code="200">Retorna a lista de clientes</response>
    /// <response code="404">Se nenhum cliente for encontrado</response>
    /// <remarks>
    /// Perfil Necessário: Padrão, Editor ou Admin
    /// 
    ///     GET /Cliente/{pagina}/{totalPorPagina}/{termo?}
    /// </remarks>
    [Authorize(Roles = "Padrao,Editor,Admin")]
    [HttpGet("{pagina}/{totalPorPagina}/{termo?}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ClienteListagemDTO>>> Get(int pagina = 1, int totalPorPagina = 10, string termo = "") {
      try {
        IEnumerable<Cliente> clientes;
        if (!string.IsNullOrEmpty(termo.Trim())) {
          clientes = await _repositorio.BuscarPaginadoAsync(x => !x.Excluido &&
            (EF.Functions.ILike(x.Nome, $"%{termo}%") ||
             EF.Functions.ILike(x.Email, $"%{termo}%") ||
             EF.Functions.ILike(x.Telefone, $"%{termo}%")
            ), pagina, totalPorPagina, z => z.Id);
        }
        else {
          clientes = await _repositorio.BuscarPaginadoAsync(x => !x.Excluido, pagina, totalPorPagina, z => z.Id);
        }

        return Ok(_mapper.Map<List<ClienteListagemDTO>>(clientes
           .Skip((pagina - 1) * totalPorPagina)
           .Take(totalPorPagina).ToList()));
      }
      catch (RepositorioException) {
        return NotFound();
      }
    }

    /// <summary>
    /// Obtém o total de clientes ativos no sistema para fins de dashboard.
    /// </summary>
    /// <returns>O número total de clientes ativos</returns>
    /// <response code="200">Retorna o total de clientes ativos</response>
    /// <remarks>
    /// Perfil Necessário: Padrão, Editor ou Admin
    /// 
    ///     GET /Cliente/dashboard
    /// </remarks>
    [Authorize(Roles = "Padrao,Editor,Admin")]
    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> Get([FromServices] ContextoBancoDeDados contextoBancoDeDados) {
      try {
        return Ok(await contextoBancoDeDados.Clientes.Where(x => !x.Excluido).CountAsync());
      }
      catch (RepositorioException) {
        return Ok(0);
      }
    }

  }
}
