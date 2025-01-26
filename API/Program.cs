using Dominio.Entidades;
using Dominio.Interfaces;
using Dominio.Servicos;
using Dominio.Validacao;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ContextoBancoDeDados>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Dominio")));

builder.Services.AddAutoMapper(typeof(Program));
//builder.Services.AddScoped(typeof(IRepositorio<>), typeof(RepositorioBase<,>));
builder.Services.AddScoped<IRepositorio<Usuario>, RepositorioBase<Usuario, ContextoBancoDeDados>>();
builder.Services.AddScoped<IRepositorio<Cliente>, RepositorioBase<Cliente, ContextoBancoDeDados>>();
builder.Services.AddScoped<IRepositorio<Produto>, RepositorioBase<Produto, ContextoBancoDeDados>>();
builder.Services.AddValidatorsFromAssemblyContaining<ClienteValidador>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
