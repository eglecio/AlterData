using AutoMapper;
using Dominio.Entidades;
using Dominio.Interfaces;
using Dominio.ModelosDTO;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    // POST: cliente/add
    // <snippet_Create>
    [HttpPost]
    public async Task<ActionResult<ClienteDTO>> Add(ClienteDTO modelo) {
      var entidade = _mapper.Map<Cliente>(modelo);

      var validacao = _validator.Validate(entidade);
      if (!validacao.IsValid)
        return BadRequest(validacao.Errors);

      await _repositorio.AdicionarAsync(entidade);

      return Ok(entidade.Id);
    }
    // </snippet_Create>



    [Authorize(Roles = "Editor,Admin")]
    [HttpGet(Name = "Listar")]
    public async Task<IEnumerable<Cliente>> Get() {
      return await _repositorio.BuscarAsync(x => x.Id > 0);
      //return Enumerable.Range(1, 5).Select(index => new WeatherForecast {
      //  Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
      //  TemperatureC = Random.Shared.Next(-20, 55),
      //  Summary = Summaries[Random.Shared.Next(Summaries.Length)]
      //})
      //.ToArray();
    }



  }
}
