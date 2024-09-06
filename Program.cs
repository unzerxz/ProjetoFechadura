using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Adicione serviços ao contêiner.
builder.Services.AddControllers();

// Configuração do JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,  // Ajuste se você usar Issuer
        ValidateAudience = false, // Ajuste se você usar Audience
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("vX0/Yt+K2@J3dN5r4K8kzG$9XbFs5Wq3p!zC&6L7L8O2H9I0J0")),
        ClockSkew = TimeSpan.Zero // Elimina a diferença de tempo entre servidores
    };

    // Adicione eventos para lidar com desafios de autenticação
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            // Personalize a resposta de erro para tokens inválidos ou ausentes
            context.HandleResponse(); // Impede a resposta padrão do desafio
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var result = new { message = "Token inválido ou não fornecido." };
            return context.Response.WriteAsJsonAsync(result);
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
            new string[] {}  // Especifica que nenhuma permissão adicional é necessária
        }
    });
});

var app = builder.Build();

// Configuração do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjetoFechadura v1");
        c.RoutePrefix = string.Empty; // Carregar a UI do Swagger na raiz
    });
}

app.UseHttpsRedirection(); // Se o projeto usa HTTPS

app.UseAuthentication(); // Habilitar a autenticação JWT
app.UseAuthorization();

app.MapControllers();

app.Run();
