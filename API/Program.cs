using Dominio.Entidades;
using Dominio.Interfaces;
using Dominio.Servicos;
using Dominio.Validacao;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options => {
  options.AddPolicy("AllowAll",
      builder => {
        builder
          .AllowAnyOrigin()
          .AllowAnyMethod() // Permite qualquer método HTTP (GET, POST, etc)
          .AllowAnyHeader(); // Permite qualquer cabeçalho
      });
});


// Add services to the container.
builder.Services.AddControllers()
  ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c => {
  c.SwaggerDoc("v1", new OpenApiInfo {
    Title = "Api AlterData",
    Version = "v1",
    Description = "Documentação da API Alterdata",
    Contact = new OpenApiContact {
      Name = "Eglecio Alexandre Pereira",
      Email = "eglecio@gmail.com",
      Url = new Uri("https://github.com/eglecio/AlterData")
    }
  });

  // Para o swagger poder ler os comentários dos XML...
  var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
  c.IncludeXmlComments(xmlPath);

  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() {
    Description = "Token estilo JWT de autorizacao usado no header atraves do esqueema Bearer",
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header
  });

  c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
      new OpenApiSecurityScheme {
        Reference = new OpenApiReference {
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer"
        }
      },
      Array.Empty<string>()
    }
  });

});

builder.Services.AddAuthentication().AddBearerToken();
builder.Services.AddDbContext<ContextoBancoDeDados>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Dominio")));

builder.Services.AddAutoMapper(typeof(Program));

//builder.Services.AddScoped(typeof(IRepositorio<>), typeof(RepositorioBase<,>));
builder.Services.AddScoped<IRepositorio<Usuario>, RepositorioBase<Usuario, ContextoBancoDeDados>>();
builder.Services.AddScoped<IRepositorio<Cliente>, RepositorioBase<Cliente, ContextoBancoDeDados>>();
builder.Services.AddScoped<IRepositorio<Produto>, RepositorioBase<Produto, ContextoBancoDeDados>>();
builder.Services.AddValidatorsFromAssemblyContaining<ClienteValidador>();
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioLoginDTOValidador>();
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioValidador>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
  //app.UseSwaggerUI(c => {
  //  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1");
  //  c.RoutePrefix = string.Empty; // Swagger será carregado na raiz do aplicativo
  //});
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowAll");

app.Run();
