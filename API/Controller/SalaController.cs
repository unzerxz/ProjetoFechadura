using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjetoFechadura.DAO;
using ProjetoFechadura.Models;

namespace ProjetoFechadura.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalaController : ControllerBase
{
    private readonly SalaDAO _salaDao;
    private readonly RegistroDAO _registroDao;
    private readonly FuncionarioDAO _funcionarioDao;

    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    public SalaController(IConfiguration configuration)
    {
        _salaDao = new SalaDAO();
        _registroDao = new RegistroDAO();
        _funcionarioDao = new FuncionarioDAO();
        _jwtSecret = configuration.GetValue<string>("JwtSettings:SecretKey");
        _jwtIssuer = configuration.GetValue<string>("JwtSettings:Issuer");
        _jwtAudience = configuration.GetValue<string>("JwtSettings:Audience");
    }

    [HttpGet]
    [Authorize]
    public IActionResult Read()
    {
        var salas = _salaDao.Read();
        return Ok(salas);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public IActionResult ReadById(int id)
    {
        var sala = _salaDao.ReadById(id);
        if (sala == null) return NotFound();
        return Ok(sala);
    }

    [HttpPost("criarSala")]
    [Authorize]
    public IActionResult CriarSala([FromBody] string identificacaoSala)
    {
        var idFuncionario = GetValidadorIdFromToken();
        if (!IsUserAdmin(idFuncionario)) return Forbid();

        if (string.IsNullOrEmpty(identificacaoSala))
        {
            return BadRequest(new { Message = "Dados inválidos. Verifique a identificação da sala." });
        }

        try
        {
            // Gerar uma credencial de sala única
            string credencialSala = _salaDao.GenerateUniqueCredencialSala(identificacaoSala);

            // Criar um novo objeto de Sala
            var novaSala = new Sala
            {
                IdentificacaoSala = identificacaoSala,
                Status = 0,  // Definir como ativa
                CredencialSala = credencialSala,
                IsAtivo = 1,  // Definir a sala como ativa
                Funcionario_IdFuncionario = 1
            };

            // Salvar a nova sala no banco de dados
            int salaId = _salaDao.Create(novaSala);

            if (salaId > 0)
            {
                return Ok(new { Message = "Sala criada com sucesso", Sala = novaSala });
            }
            else
            {
                return StatusCode(500, new { Message = "Erro ao criar a sala" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Erro ao criar a sala", Error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public IActionResult Delete(int id)
    {
        var idFuncionario = GetValidadorIdFromToken();
        if (!IsUserAdmin(idFuncionario)) return Forbid();

        if (_salaDao.ReadById(id) == null) return NotFound();
        _salaDao.Delete(id);
        return NoContent();
    }

    private bool IsUserAdmin(int idFuncionario)
    {
        return _funcionarioDao.IsFuncionarioAdmin(idFuncionario);
    }

 private int GetValidadorIdFromToken()
        {
            // Obter o token JWT do cabeçalho Authorization
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            Console.WriteLine("Token: " + authHeader);

            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                Console.WriteLine("Token não fornecido ou inválido no cabeçalho");
                return 0; // Retorna 0 se o token não estiver presente ou for inválido
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            Console.WriteLine("Token extraído: " + token);

            try
            {
                // Parâmetros de validação do token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSecret);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _jwtIssuer,
                    ValidAudience = _jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true // Verifica se o token não expirou
                };

                // Valida o token e obtém as claims
                SecurityToken validatedToken;
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                Console.WriteLine("Token validado com sucesso.");

                // Log de todas as claims
                foreach (var claim in claimsPrincipal.Claims)
                {
                    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                }

                var idClaim = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == "idFuncionario");

                if (idClaim != null)
                {
                    Console.WriteLine($"ID do funcionário (claim): {idClaim.Value}");
                    if (int.TryParse(idClaim.Value, out int idFuncionario))
                    {
                        Console.WriteLine($"ID do funcionário extraído do token: {idFuncionario}");
                        return idFuncionario;
                    }
                    else
                    {
                        Console.WriteLine("Não foi possível converter o valor da claim para um inteiro.");
                    }
                }
                else
                {
                    Console.WriteLine("Claim 'idFuncionario' não encontrada.");
                }
            }
            catch (SecurityTokenExpiredException ex)
            {
                Console.WriteLine("Token expirado: " + ex.Message);
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                Console.WriteLine("Assinatura do token inválida: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao validar o token: {ex.Message}");
            }

            Console.WriteLine("Falha ao obter ID do validador a partir do token.");
            return 0; // Retorna 0 caso a validação falhe
        }

        [HttpGet("validarEntradaSaida")]
public IActionResult ValidarEntradaSaida([FromQuery] int idSala, [FromQuery] string ?credencialCartao = null, [FromQuery] int? credencialTeclado = null)
{
    int idFuncionario = -1;
    int idRegistro = -1;

    // Validando a credencial do cartão ou do teclado
    if (!string.IsNullOrEmpty(credencialCartao))
    {
        idFuncionario = _salaDao.IsCredencialUsuarioCartaoValida(credencialCartao);
    }
    else if (credencialTeclado.HasValue)
    {
        idFuncionario = _salaDao.IsCredencialUsuarioTecladoValida(credencialTeclado.Value);
    }

    if (idFuncionario == -1)
    {
        return Unauthorized(new { Message = "Credencial inválida" });
    }

    // Verificar se o funcionário está ativo
    bool isFuncionarioAtivo = _funcionarioDao.IsFuncionarioAtivo(idFuncionario); // Adicione esta linha

    if (!isFuncionarioAtivo) // Verifica se o funcionário não está ativo
    {
        return Unauthorized(new { Message = "Funcionário inativo" });
    }

    // Verificando o status da sala
    bool isSalaAtiva = _salaDao.IsSalaAtiva(idSala);
    bool isSalaVazia = _salaDao.IsSalaVazia(idSala);

    if (isSalaAtiva)
    {
        if (isSalaVazia)
        {
            // Sala vazia, abrir a sala para o funcionário
            bool salaAberta = _salaDao.AbrirSala(idSala, idFuncionario);
            if (salaAberta)
            {
                idRegistro = _registroDao.CriarRegistro(idSala, idFuncionario);
                return Ok(new { Message = "Sala aberta com sucesso", FuncionarioId = idFuncionario, SalaId = idSala, RegistroId = idRegistro });
            }
            else
            {
                return StatusCode(500, new { Message = "Erro ao abrir a sala" });
            }
        }
        else
        {
            // Sala ocupada, obter o registro ativo
            idRegistro = _registroDao.ObterRegistroAtivo(idSala);
            if (idRegistro != -1)
            {
                // Tentar fechar a sala se o funcionário estiver ocupando a sala
                bool salaFechada = _salaDao.FecharSala(idSala, idFuncionario);
                if (salaFechada)
                {
                    _registroDao.AtualizarHorarioSaida(idRegistro);
                    return Ok(new { Message = "Sala fechada com sucesso", FuncionarioId = idFuncionario, SalaId = idSala, RegistroId = idRegistro });
                }
                else
                {
                    return Unauthorized(new { Message = "Não autorizado a fechar a sala", FuncionarioId = idFuncionario, SalaId = idSala });
                }
            }
            else
            {
                return NotFound(new { Message = "Registro ativo não encontrado", SalaId = idSala });
            }
        }
    }
    else
    {
        return BadRequest(new { Message = "Sala não está ativa" });
    }
}

    [HttpGet("autorizar-entrada")]
    public ApiResponse AutorizarEntrada(int idSala, string credencial)
    {
        if (!_salaDao.IsSalaAtiva(idSala))
        {
            return new ApiResponse
            {
                StatusCode = 403,
                Message = "A sala está inativa."
            };
        }

        if (_salaDao.IsSalaVazia(idSala))
        {
            return new ApiResponse
            {
                StatusCode = 403,
                Message = "A sala está desocupada. Não é possível entrar."
            };
        }

        if (!_salaDao.IsCredencialCorreta(idSala, credencial))
        {
            return new ApiResponse
            {
                StatusCode = 401,
                Message = "Credencial inválida."
            };
        }

        return new ApiResponse
        {
            StatusCode = 200,
            Message = "Entrada autorizada."
        };
    }
}



public class ApiResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
}

