using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.dominio.dtos;
using minimal_api.dominio.entidades;
using minimal_api.dominio.Enuns;
using minimal_api.dominio.ModelViews;
using minimal_api.dominio.servicos;
using minimal_api.infraestutura.DB;
using minimal_api.infraestutura.Interfaces;

#region Builder
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IAdministradorServico, AdministradoServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculosServico>();

//Adicionar Jwt
var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddAuthorization();


//Adicionar contexto do banco
builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

//add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token jwt aqui"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
#endregion
#region App
var app = builder.Build();

//usar swagger
app.UseSwagger();
app.UseSwaggerUI();

//adicionar para o Jwt
app.UseAuthentication();
app.UseAuthorization();

#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores

string GerarTokenJwt(Administrador administrador)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>(){
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil)
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );
    return new JwtSecurityTokenHandler().WriteToken(token);
}

ErrosDeValidacao ValidaDTO2(AdministradorDTO adm)
{
    var validacao = new ErrosDeValidacao();

    if (string.IsNullOrEmpty(adm.Email))
        validacao.Mensagens.Add("O e-mail não pode ser vazio");
    if (string.IsNullOrEmpty(adm.Senha))
        validacao.Mensagens.Add("A Senha não pode ser vazia");
    if (adm.Perfil == null)
        validacao.Mensagens.Add("O perfil não pode ser vazio");
    return validacao;
}

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, [FromServices] IAdministradorServico administradorServico) =>
{
    var adm = administradorServico.Login(loginDTO);
    if (adm != null)
    {
        string token = GerarTokenJwt(adm);
        return Results.Ok(new AdmLogado
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }

    else return Results.Unauthorized();
}).AllowAnonymous().WithTags("Administradores");

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, [FromServices] IAdministradorServico administradorServico) =>
{
    var validacao = ValidaDTO2(administradorDTO);
    if (validacao.Mensagens.Count() > 0)
        return Results.BadRequest(validacao);

    var adm = new Administrador
    {
        Email = administradorDTO.Email,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString(),
        Senha = administradorDTO.Senha
    };
    administradorServico.Incluir(adm);
    return Results.Created($"/veiculo/{adm.Id}", adm);
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).RequireAuthorization().WithTags("Administradores");

app.MapGet("/administradores", ([FromQuery] int? pagina, [FromServices] IAdministradorServico administradorServico) =>
{
    var administradores = administradorServico.Todos(pagina);
    var adms = administradores.Select(a => new AdministradorModelView { Email = a.Email, Perfil = a.Perfil }).ToList();
    return Results.Ok(adms);
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).RequireAuthorization().WithTags("Administradores");

app.MapGet("/administradores/{id}", ([FromRoute] int id, [FromServices] IAdministradorServico administradorServico) =>
{
    var adm = administradorServico.BuscarPorId(id);
    return Results.Ok(adm);
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).RequireAuthorization().WithTags("Administradores");

app.MapPut("/administradores/{id}", ([FromRoute] int id, [FromBody] AdministradorDTO administradorDTO, [FromServices] IAdministradorServico administradorServico) =>
{
    var validacao = ValidaDTO2(administradorDTO);
    if (validacao.Mensagens.Count() > 0)
        return Results.BadRequest(validacao);

    var adm = administradorServico.BuscarPorId(id);
    if (adm == null) return Results.NotFound("Administrador não encontrado");
    adm.Email = administradorDTO.Email;
    adm.Senha = administradorDTO.Senha;
    adm.Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString();

    administradorServico.Atualizar(adm);
    return Results.Ok(adm);
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).RequireAuthorization().WithTags("Administradores");

app.MapDelete("/administradores/{id}", ([FromRoute] int id, [FromServices] IAdministradorServico administradorServico) =>
{
    var adm = administradorServico.BuscarPorId(id);
    if (adm == null) return Results.NotFound("Administrador não encontrado");

    administradorServico.Apagar(adm);
    return Results.Ok(adm);
}).RequireAuthorization().WithTags("Administradores");



#endregion

#region Veiculos
ErrosDeValidacao ValidaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidacao();

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
        validacao.Mensagens.Add("O nome não pode ser vazio");
    if (string.IsNullOrEmpty(veiculoDTO.Marca))
        validacao.Mensagens.Add("A Marca não pode ser vazia");
    if (veiculoDTO.Ano < 1950)
        validacao.Mensagens.Add("Veiculo muito antigo, deve ser superior a 1950");
    return validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, [FromServices] IVeiculoServico veiculoServico) =>
{
    var validacao = ValidaDTO(veiculoDTO);
    if (validacao.Mensagens.Count() > 0)
        return Results.BadRequest(validacao);

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    veiculoServico.Incluir(veiculo);
    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.RequireAuthorization(new AuthorizeAttribute { Roles = "Editor" })
.RequireAuthorization().WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int? pagina, [FromServices] IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);
    return Results.Ok(veiculos);
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).RequireAuthorization().WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, [FromServices] IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound("Veiculo não encontrado");
    return Results.Ok(veiculo);
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).RequireAuthorization().WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, [FromBody] VeiculoDTO veiculoDTO, [FromServices] IVeiculoServico veiculoServico) =>
{
    var validacao = ValidaDTO(veiculoDTO);
    if (validacao.Mensagens.Count() > 0)
        return Results.BadRequest(validacao);

    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound("Veiculo não encontrado");
    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);
    return Results.Ok(veiculo);
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.RequireAuthorization().WithTags("Veiculos");

app.MapDelete("veiculos/{id}", ([FromRoute] int id, [FromServices] IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound("Veiculo não encontrado");
    veiculoServico.Apagar(veiculo);
    return Results.Ok(veiculo);
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.RequireAuthorization().WithTags("Veiculos");

#endregion

app.Run();


