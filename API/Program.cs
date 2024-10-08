using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using ProjetoFechadura.DAO;

var builder = WebApplication.CreateBuilder(args);

// Configuração de logs
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Carregar configurações JWT do appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("SecretKey");
var issuer = jwtSettings.GetValue<string>("Issuer");
var audience = jwtSettings.GetValue<string>("Audience");

// Adicionar serviços ao contêiner
builder.Services.AddControllers();

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configuração da autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,  // Valida o Issuer
            ValidateAudience = true, // Valida o Audience
            ValidateLifetime = true, // Valida o tempo de vida do token
            ValidateIssuerSigningKey = true, // Valida a chave de assinatura
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? throw new InvalidOperationException("Secret key is null"))),
            ClockSkew = TimeSpan.Zero // Elimina diferença de tempo entre servidores
        };

        // Eventos para lidar com desafios de autenticação
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse(); // Impede a resposta padrão do desafio
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var result = new { message = "Token inválido ou não fornecido." };
                return context.Response.WriteAsJsonAsync(result);
            },
            OnTokenValidated = async context =>
            {
                var tokenDao = context.HttpContext.RequestServices.GetRequiredService<TokenDAO>();
                var tokenString = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                
                if (!tokenDao.IsTokenValidAndActive(tokenString))
                {
                    context.Fail("Token inválido, expirado ou não encontrado no banco de dados.");
                }
            }
        };
    });

builder.Services.AddAuthorization();

// Configuração do Swagger com segurança JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProjetoFechadura", Version = "v1" });

    // Definição de segurança para o JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato **Bearer {seu_token}**"
    });

    // Requisito de segurança global para todas as rotas
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}  // Nenhuma permissão adicional é necessária
        }
    });
});

builder.Services.AddScoped<TokenDAO>(); // Register TokenDAO as a service

// Adicionar serviço de limpeza de tokens expirados (a cada 1 hora)
builder.Services.AddHostedService(provider => new TokenCleanupService(provider, TimeSpan.FromHours(1)));

var app = builder.Build();

// Configuração do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjetoFechadura v1");
        c.RoutePrefix = string.Empty; // Carregar UI do Swagger na raiz
    });
}

app.UseHttpsRedirection(); // Se o projeto usa HTTPS

// Habilitar CORS
app.UseCors("AllowAllOrigins");

// Habilitar autenticação e autorização JWT
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();