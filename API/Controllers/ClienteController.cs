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

    // POST: cliente
    // <snippet_Create>
    [HttpPost]
    [Authorize(Roles = "Editor,Admin")]
    public async Task<ActionResult<ClienteDTO>> Add(ClienteDTO modelo) {
      var entidade = _mapper.Map<Cliente>(modelo);

      var validacao = _validator.Validate(entidade);
      if (!validacao.IsValid)
        return BadRequest(validacao.Errors);

      await _repositorio.AdicionarAsync(entidade);

      return Ok(entidade.Id);
    }
    // </snippet_Create>

    // PUT: cliente
    // <snippet_Create>
    [HttpPut]
    [Authorize(Roles = "Editor,Admin")]
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
    // </snippet_Create>


    // DELETE: cliente
    // <snippet_Create>
    [HttpDelete]
    [Authorize(Roles = "Editor,Admin")]
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
    // </snippet_Create>


    // GET: cliente
    // <snippet_Create>
    [HttpGet("{clienteId}")]
    [Authorize(Roles = "Editor,Admin")]
    public async Task<ActionResult<ClienteDTO>> Get(int clienteId) {
      try {
        var cliente = await _repositorio.ObterPorIdAsync(clienteId);
        var modelo = _mapper.Map<ClienteDTO>(cliente);
        return Ok(modelo);
      }
      catch (RepositorioException) {
        return NotFound();
      }
    }
    // </snippet_Create>


    // GET: cliente
    // <snippet_Create>
    [Authorize(Roles = "Usuario,Editor,Admin")]
    [HttpGet("{termo}/{pagina}/{totalPorPagina}")]
    public async Task<ActionResult<IEnumerable<Cliente>>> Get(string termo, int pagina = 1, int totalPorPagina = 10) {
      try {
        var clientes = await _repositorio.BuscarAsync(x =>
            EF.Functions.ILike(x.Nome, $"%{termo}%") ||
            EF.Functions.ILike(x.Email, $"%{termo}%") ||
            EF.Functions.ILike(x.Telefone, $"%{termo}%"));

        if (!clientes.Any())
          return NoContent();

        return Ok(clientes
           .Skip((pagina - 1) * totalPorPagina)
           .Take(totalPorPagina));
      }
      catch (RepositorioException) {
        return NotFound();
      }
    }
    // </snippet_Create>



  }
}
