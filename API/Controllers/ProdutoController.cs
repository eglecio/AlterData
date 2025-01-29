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

    // POST: produto
    // <snippet_Create>
    [HttpPost]
    [Authorize(Roles = "Editor,Admin")]
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
    // </snippet_Create>


    // PUT: produto
    // <snippet_Create>
    [HttpPut]
    [Authorize(Roles = "Editor,Admin")]
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
    // </snippet_Create>


    // DELETE: produto
    // <snippet_Create>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Editor,Admin")]
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
    // </snippet_Create>


    // GET: produto
    // <snippet_Create>
    [HttpGet("{id}")]
    [Authorize(Roles = "Editor,Admin")]
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
    // </snippet_Create>


    // GET: produto
    // <snippet_Create>
    [Authorize(Roles = "Usuario,Editor,Admin")]
    [HttpGet("{pagina}/{totalPorPagina}/{termo?}")]
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
    // </snippet_Create>



  }
}
