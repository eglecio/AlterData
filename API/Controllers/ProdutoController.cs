using AutoMapper;
using Dominio.Entidades;
using Dominio.Interfaces;
using Dominio.ModelosDTO;
using Dominio.Servicos;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace API.Controllers {

  /// <summary>
  /// Controlador respons�vel pelo gerenciamento de produtos no sistema.
  /// Requer autentica��o para todos os endpoints.
  /// </summary>
  /// <remarks>
  /// Rota base: /Produto
  /// Autentica��o: Obrigat�ria
  /// Opera��es suportadas:
  /// - Cria��o de novos produtos
  /// - Atualiza��o de produtos existentes
  /// - Exclus�o de produtos
  /// - Consulta de detalhes de produtos
  /// - Listagem de produtos com pagina��o e busca
  /// - Estat�sticas de dashboard e estoque
  /// </remarks>
  [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class ProdutoController : ControllerBase {
    private readonly ILogger<Produto> _logger;
    private readonly IMapper _mapper;
    private readonly IRepositorio<Produto> _repositorio;
    private readonly IValidator<Produto> _validator;

    public ProdutoController(
      ILogger<Produto> logger,
      IMapper mapper,
      IRepositorio<Produto> repositorio,
      IValidator<Produto> validator) {

      _logger = logger;
      _mapper = mapper;
      _repositorio = repositorio;
      _validator = validator;
    }

    /// <summary>
    /// Adiciona um novo produto no sistema.
    /// </summary>
    /// <param name="modelo">Objeto DTO contendo as informa��es do produto</param>
    /// <returns>O resultado da opera��o</returns>
    /// <response code="200">Se o produto foi criado com sucesso</response>
    /// <response code="400">Se os dados do produto forem inv�lidos</response>
    /// <response code="401">Se o usu�rio n�o estiver autenticado</response>
    /// <response code="403">Se o usu�rio n�o tiver permiss�o de editor ou admin</response>
    /// <remarks>
    /// Perfil Necess�rio: Editor ou Admin
    /// Exemplo de requisi��o:
    /// 
    ///     POST /Produto
    ///     {
    ///         "nome": "Produto Exemplo",
    ///         "descricao": "Descri��o do produto",
    ///         "preco": 99.90,
    ///         "quantidadeEstoque": 100
    ///     }
    /// </remarks>
    [HttpPost]
    [Authorize(Roles = "Editor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProdutoDTO>> Add(ProdutoDTO modelo) {
      try {
        var entidade = _mapper.Map<Produto>(modelo);

        var validacao = _validator.Validate(entidade);
        if (!validacao.IsValid)
          return BadRequest(validacao.Errors);

        await _repositorio.AdicionarAsync(entidade);
      }
      catch (Exception ex) {
        _logger.LogError($"Add produto: {ex.Message + " - inner execption: " + ex.InnerException}");
        return Problem();
      }

      return Ok();
    }

    /// <summary>
    /// Atualiza as informa��es de um produto existente.
    /// </summary>
    /// <param name="modelo">Objeto DTO contendo as informa��es atualizadas do produto</param>
    /// <returns>O resultado da opera��o</returns>
    /// <response code="200">Se o produto foi atualizado com sucesso</response>
    /// <response code="400">Se os dados atualizados do produto forem inv�lidos</response>
    /// <response code="404">Se o produto n�o for encontrado</response>
    /// <remarks>
    /// Perfil Necess�rio: Editor ou Admin
    /// Exemplo de requisi��o:
    /// 
    ///     PUT /Produto
    ///     {
    ///         "id": 1,
    ///         "nome": "Produto Atualizado",
    ///         "descricao": "Nova descri��o",
    ///         "preco": 149.90,
    ///         "quantidadeEstoque": 150
    ///     }
    /// </remarks>
    [HttpPut]
    [Authorize(Roles = "Editor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoDTO>> Update(ProdutoDTO modelo) {
      try {
        var entidadeExistente = await _repositorio.ObterPorIdAsync(modelo.Id);
        _mapper.Map(modelo, entidadeExistente);// Mapea propriedades, ignorando DataCadastro e ID...

        var validacao = _validator.Validate(entidadeExistente);
        if (!validacao.IsValid)
          return BadRequest(validacao.Errors);

        await _repositorio.AtualizarAsync(entidadeExistente);
      }
      catch (RepositorioException) {
        return NotFound();
      }

      return Ok();
    }

    /// <summary>
    /// Remove um produto do sistema.
    /// </summary>
    /// <param name="id">O ID do produto a ser removido</param>
    /// <returns>O resultado da opera��o</returns>
    /// <response code="200">Se o produto foi removido com sucesso</response>
    /// <response code="404">Se o produto n�o foi encontrado</response>
    /// <remarks>
    /// Perfil Necess�rio: Editor ou Admin
    /// 
    ///     DELETE /Produto/{id}
    /// </remarks>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Editor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoDTO>> Delete(int id) {
      try {
        var entidadeExistente = await _repositorio.ObterPorIdAsync(id);
        await _repositorio.RemoverAsync(entidadeExistente);
      }
      catch (RepositorioException) {
        return NotFound();
      }

      return Ok();
    }

    /// <summary>
    /// Recupera os detalhes de um produto espec�fico pelo ID.
    /// </summary>
    /// <param name="id">O ID do produto a ser consultado</param>
    /// <returns>Os detalhes do produto</returns>
    /// <response code="200">Retorna as informa��es do produto solicitado</response>
    /// <response code="404">Se o produto n�o foi encontrado</response>
    /// <remarks>
    /// Perfil Necess�rio: Padr�o, Editor ou Admin
    /// 
    ///     GET /Produto/{id}
    /// </remarks>
    [HttpGet("{id}")]
    [Authorize(Roles = "Padrao,Editor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoDTO>> Get(int id) {
      try {
        var entidade = await _repositorio.ObterPorIdAsync(id);
        var modelo = _mapper.Map<ProdutoVisualizacaoDTO>(entidade);
        return Ok(modelo);
      }
      catch (RepositorioException) {
        return NotFound();
      }
    }

    /// <summary>
    /// Recupera uma lista paginada de produtos com funcionalidade de busca opcional.
    /// </summary>
    /// <param name="pagina">N�mero da p�gina (padr�o: 1)</param>
    /// <param name="totalPorPagina">Itens por p�gina (padr�o: 10)</param>
    /// <param name="termo">Termo de busca opcional para filtrar produtos por nome</param>
    /// <returns>Uma lista de produtos que correspondem aos crit�rios de busca</returns>
    /// <response code="200">Retorna a lista de produtos</response>
    /// <response code="404">Se nenhum produto for encontrado</response>
    /// <remarks>
    /// Perfil Necess�rio: Padr�o, Editor ou Admin
    /// 
    ///     GET /Produto/{pagina}/{totalPorPagina}/{termo?}
    /// </remarks>
    [Authorize(Roles = "Padrao,Editor,Admin")]
    [HttpGet("{pagina}/{totalPorPagina}/{termo?}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ProdutoListagemDTO>>> Get(int pagina = 1, int totalPorPagina = 10, string termo = "") {
      try {
        IEnumerable<Produto> produtos;
        if (!string.IsNullOrEmpty(termo.Trim())) {
          produtos = await _repositorio.BuscarPaginadoAsync(x => !x.Excluido && EF.Functions.ILike(x.Nome, $"%{termo}%"), pagina, totalPorPagina, z => z.Id);
        }
        else {
          produtos = await _repositorio.BuscarPaginadoAsync(x => !x.Excluido, pagina, totalPorPagina, z => z.Id);
        }

        return Ok(_mapper.Map<List<ProdutoListagemDTO>>(produtos
           .Skip((pagina - 1) * totalPorPagina)
           .Take(totalPorPagina).ToList()));
      }
      catch (RepositorioException) {
        return NotFound();
      }
    }

    /// <summary>
    /// Obt�m o total de produtos ativos no sistema para fins de dashboard.
    /// </summary>
    /// <returns>O n�mero total de produtos ativos</returns>
    /// <response code="200">Retorna o total de produtos ativos</response>
    /// <remarks>
    /// Perfil Necess�rio: Padr�o, Editor ou Admin
    /// 
    ///     GET /Produto/dashboard
    /// </remarks>
    [Authorize(Roles = "Padrao,Editor,Admin")]
    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> Get([FromServices] ContextoBancoDeDados contextoBancoDeDados) {
      try {
        return Ok(await contextoBancoDeDados.Produtos.Where(x => !x.Excluido).CountAsync());
      }
      catch (RepositorioException) {
        return Ok(0);
      }
    }

    /// <summary>
    /// Lista os 10 produtos com maior quantidade em estoque.
    /// </summary>
    /// <returns>Lista dos top 10 produtos com nome e quantidade em estoque</returns>
    /// <response code="200">Retorna a lista dos produtos com maior estoque</response>
    /// <remarks>
    /// Perfil Necess�rio: Padr�o, Editor ou Admin
    /// Retorna uma lista de pares chave-valor onde:
    /// - Chave (string): Nome do produto
    /// - Valor (int): Quantidade em estoque
    /// 
    ///     GET /Produto/top
    /// </remarks>
    [Authorize(Roles = "Padrao,Editor,Admin")]
    [HttpGet("top")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<KeyValuePair<string, int>>>> GetProdutosComMaiorEstoque([FromServices] ContextoBancoDeDados contextoBancoDeDados) {
      try {
        var topProdutos = await contextoBancoDeDados.Produtos
            .Where(x => !x.Excluido && x.QuantidadeEstoque > 0)
            .OrderByDescending(x => x.QuantidadeEstoque)
            .Take(10)
            .Select(p => new KeyValuePair<string, int>(p.Nome, (int)p.QuantidadeEstoque))
            .ToListAsync();

        return Ok(topProdutos);
      }
      catch (RepositorioException) {
        return Ok(new List<KeyValuePair<string, int>>());
      }
    }

    /// <summary>
    /// Lista os 10 produtos com estoque zerado ou negativo.
    /// </summary>
    /// <returns>Lista de produtos com estoque cr�tico</returns>
    /// <response code="200">Retorna a lista dos produtos com estoque zerado ou negativo</response>
    /// <remarks>
    /// Perfil Necess�rio: Padr�o, Editor ou Admin
    /// Retorna uma lista de pares chave-valor onde:
    /// - Chave (string): Nome do produto
    /// - Valor (int): Quantidade em estoque (zero ou negativo)
    /// 
    ///     GET /Produto/estoque-negativo
    /// </remarks>
    [Authorize(Roles = "Padrao,Editor,Admin")]
    [HttpGet("estoque-negativo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<KeyValuePair<string, int>>>> GetProdutosComEstoqueZeradoOuNegativo([FromServices] ContextoBancoDeDados contextoBancoDeDados) {
      try {
        var topProdutos = await contextoBancoDeDados.Produtos
            .Where(x => !x.Excluido && x.QuantidadeEstoque <= 0)
            .OrderBy(x => x.QuantidadeEstoque)
            .Take(10)
            .Select(p => new KeyValuePair<string, int>(p.Nome, (int)p.QuantidadeEstoque))
            .ToListAsync();

        return Ok(topProdutos);
      }
      catch (RepositorioException) {
        return Ok(new List<KeyValuePair<string, int>>());
      }
    }

  }
}
