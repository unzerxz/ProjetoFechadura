using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjetoFechadura.Models;
using ProjetoFechadura.DAO;

namespace ProjetoFechadura.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionarioController : ControllerBase
    {
        private readonly FuncionarioDAO _funcionarioDao;
        private readonly TokenDAO _tokenDao;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public FuncionarioController(IConfiguration configuration)
        {
            _funcionarioDao = new FuncionarioDAO();
            _tokenDao = new TokenDAO();
            _jwtSecret = configuration.GetValue<string>("JwtSettings:SecretKey");
            _jwtIssuer = configuration.GetValue<string>("JwtSettings:Issuer");
            _jwtAudience = configuration.GetValue<string>("JwtSettings:Audience");
        }

        // Método para obter todos os funcionários (somente Admin)
        [HttpGet]
        [Authorize]
        public IActionResult Read()
        {
            var idFuncionario = GetValidadorIdFromToken();
            Console.WriteLine($"ID do funcionário: {idFuncionario}");

            if (!IsUserAdmin(idFuncionario))
            {
                Console.WriteLine("Usuário não é admin");
                return Forbid();
            }

            var funcionarios = _funcionarioDao.Read();
            return Ok(funcionarios);
        }

        // Método para obter funcionário por ID
        [HttpGet("{id:int}")]
        [Authorize]
        public IActionResult ReadById(int id)
        {
            var idFuncionario = GetValidadorIdFromToken();
            if (!IsUserAdmin(idFuncionario)) return Forbid();

            var funcionario = _funcionarioDao.ReadById(id);
            if (funcionario == null) return NotFound();
            return Ok(funcionario);
        }

        // Método para criar novo funcionário
        [HttpPost]
        public IActionResult Post(Funcionario funcionario)
        {
            _funcionarioDao.Create(funcionario);
            return CreatedAtAction(nameof(ReadById), new { id = funcionario.IdFuncionario }, funcionario);
        }

        // Método para atualizar um funcionário
        [HttpPut("{id:int}")]
        [Authorize]
        public IActionResult Put(int id, [FromBody] Funcionario funcionario)
        {
            var idFuncionario = GetValidadorIdFromToken();
            if (!IsUserAdmin(idFuncionario)) return Forbid();

            if (id != funcionario.IdFuncionario || id == 1) return BadRequest();
            if (_funcionarioDao.ReadById(id) == null) return NotFound();
            _funcionarioDao.Update(id, funcionario);
            return NoContent();
        }

        // Método para deletar um funcionário
        [HttpDelete("{id:int}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var idFuncionario = GetValidadorIdFromToken();
            if (!IsUserAdmin(idFuncionario)) return Forbid();

            if (_funcionarioDao.ReadById(id) == null || id == 1) return NotFound();
            _funcionarioDao.Delete(id);
            return NoContent();
        }

        // Método para promoção de funcionário (somente Admin)
        [HttpPost("confirmacaoFuncionario")]
        [Authorize]
        public IActionResult ConfirmacaoFuncionario([FromQuery] int idFuncionario, [FromQuery] int novoCargoId, [FromQuery] int novoPerfilId)
        {
            if (idFuncionario <= 1) return BadRequest(new { Message = "ID do Funcionário inválido" });

            try
            {
                var idValidador = GetValidadorIdFromToken();
                if (!IsUserAdmin(idValidador)) return BadRequest(new { Message = "Usuário não autorizado" });

                var funcionarioAtual = _funcionarioDao.ReadById(idFuncionario);
                if (funcionarioAtual == null) return NotFound(new { Message = "Funcionário não encontrado" });

                if (funcionarioAtual.IsAtivo == 1) return BadRequest(new { Message = "Funcionário já está ativo" });

                string credencialCartao = _funcionarioDao.GenerateUniqueCredencialCartao(funcionarioAtual);
                int credencialTeclado = int.Parse(_funcionarioDao.GenerateUniqueRandomPassword());

                funcionarioAtual.IsAtivo = 1;
                funcionarioAtual.CredencialCartao = credencialCartao;
                funcionarioAtual.CredencialTeclado = credencialTeclado;
                funcionarioAtual.Cargo_IdCargo = novoCargoId;
                funcionarioAtual.Perfil_IdPerfil = novoPerfilId;

                _funcionarioDao.Update(idFuncionario, funcionarioAtual);
                return Ok(new { Message = "Funcionário promovido e ativado com sucesso", Funcionario = funcionarioAtual });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro ao promover o funcionário", Error = ex.Message });
            }
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

        // Método para verificar se o usuário é admin
        private bool IsUserAdmin(int idFuncionario)
        {
            return _funcionarioDao.IsFuncionarioAdmin(idFuncionario);
        }

        // Endpoint de login que gera um token JWT
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NomeUsuario) || string.IsNullOrWhiteSpace(request.Senha))
            {
                return BadRequest(new { Message = "Nome de usuário e senha são obrigatórios" });
            }

            int idFuncionario = _funcionarioDao.AuthenticateUser(request.NomeUsuario, request.Senha);
            if (idFuncionario == 0) return Unauthorized(new { Message = "Nome de usuário ou senha inválidos" });

            var funcionario = _funcionarioDao.ReadById(idFuncionario);
            if (funcionario == null || funcionario.IsAtivo != 1) return Unauthorized(new { Message = "Usuário não está ativo" });

            // Verifica se já existe um token válido para este funcionário
            var existingToken = _tokenDao.GetValidTokenForFuncionario(idFuncionario);
            if (existingToken != null)
            {
                return Ok(new { Token = existingToken.Token, ExpirationTime = existingToken.TimeExpiracao });
            }

            // Se não existe um token válido, cria um novo
            var token = GenerateJwtToken(idFuncionario);
            var expirationTime = DateTime.UtcNow.AddHours(24); // Alterado para 24 horas
            _tokenDao.SaveToken(token, expirationTime, idFuncionario);

            return Ok(new { Token = token, ExpirationTime = expirationTime });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { Message = "Token não fornecido" });
            }

            _tokenDao.RemoveToken(token);
            return Ok(new { Message = "Logout realizado com sucesso" });
        }

        // Função para gerar o token JWT
        private string GenerateJwtToken(int idFuncionario)
        {
            var claims = new[]
            {
                new Claim("idFuncionario", idFuncionario.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24), // Alterado para 24 horas
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // Classe de request para o login
    public class LoginRequest
    {
        public string NomeUsuario { get; set; }
        public string Senha { get; set; }
    }
}
