using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjetoFechadura.Models;

namespace ProjetoFechadura.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionarioController : ControllerBase
    {
        private readonly FuncionarioDAO _funcionarioDao;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public FuncionarioController(IConfiguration configuration)
        {
            _funcionarioDao = new FuncionarioDAO();
            _jwtSecret = configuration.GetValue<string>("JwtSettings:SecretKey");
            _jwtIssuer = configuration.GetValue<string>("JwtSettings:Issuer");
            _jwtAudience = configuration.GetValue<string>("JwtSettings:Audience");
        }

        // Método para obter todos os funcionários
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

            Console.WriteLine("Usuário é admin");
            var funcionarios = _funcionarioDao.Read();
            return Ok(funcionarios);
        }

        // Método para obter funcionário por ID
        [HttpGet("{id:int}")]
        [Authorize]
        public IActionResult ReadById(int id)
        {
            var idFuncionario = GetValidadorIdFromToken();
            if (!IsUserAdmin(idFuncionario)) return Forbid(); // Verifica se o usuário é admin

            var funcionario = _funcionarioDao.ReadById(id);
            if (funcionario == null) return NotFound();
            return Ok(funcionario);
        }

        // Método para atualizar um funcionário existente
        [HttpPut("{id:int}")]
        [Authorize]
        public IActionResult Put(int id, [FromBody] Funcionario funcionario)
        {
            var idFuncionario = GetValidadorIdFromToken();
            if (!IsUserAdmin(idFuncionario)) return Forbid(); // Verifica se o usuário é admin

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
            if (!IsUserAdmin(idFuncionario)) return Forbid(); // Verifica se o usuário é admin

            if (_funcionarioDao.ReadById(id) == null || id == 1) return NotFound();
            _funcionarioDao.Delete(id);
            return NoContent();
        }

        [HttpPost("confirmacaoFuncionario")] // Admin
[Authorize] // Garante que apenas usuários autenticados podem acessar este endpoint
public IActionResult ConfirmacaoFuncionario(
    [FromQuery] int idFuncionario,
    [FromQuery] int novoCargoId,
    [FromQuery] int novoPerfilId)
{
    // Verificar se o ID do funcionário é válido
    if (idFuncionario <= 1)
    {
        return BadRequest(new { Message = "ID do Funcionário inválido" });
    }

    try
    {
        // Obter o ID do validador a partir do token
        var idValidador = GetValidadorIdFromToken();

        // Validar se o validador é um administrador
        if (!IsUserAdmin(idValidador))
        {
            return BadRequest(new { Message = "Usuário não autorizado para promover funcionários" });
        }

        // Recuperar os dados atuais do funcionário pelo ID
        var funcionarioAtual = _funcionarioDao.ReadById(idFuncionario);

        // Verificar se o funcionário foi encontrado
        if (funcionarioAtual == null)
        {
            return NotFound(new { Message = "Funcionário não encontrado" });
        }

        // Garantir que o funcionário não está ativo
        if (funcionarioAtual.IsAtivo == 1)
        {
            return BadRequest(new { Message = "Funcionário já está ativo" });
        }

        // Gerar uma credencial de cartão única
        string credencialCartao = _funcionarioDao.GenerateUniqueCredencialCartao(funcionarioAtual);

        // Gerar uma senha de teclado única
        int credencialTeclado = int.Parse(_funcionarioDao.GenerateUniqueRandomPassword());

        // Atualizar os campos do funcionário para ativação
        funcionarioAtual.IsAtivo = 1;  // Ativar o funcionário
        funcionarioAtual.CredencialCartao = credencialCartao;  // Nova credencial de cartão
        funcionarioAtual.CredencialTeclado = credencialTeclado;  // Nova senha de teclado
        funcionarioAtual.Cargo_IdCargo = novoCargoId;  // Atualizar o cargo
        funcionarioAtual.Perfil_IdPerfil = novoPerfilId;  // Atualizar o perfil

        // Atualizar o funcionário no banco de dados
        _funcionarioDao.Update(idFuncionario, funcionarioAtual);

        return Ok(new { Message = "Funcionário promovido e ativado com sucesso", Funcionario = funcionarioAtual });
    }
    catch (Exception ex)
    {
        // Tratar erros inesperados
        return StatusCode(500, new { Message = "Erro ao promover o funcionário", Error = ex.Message });
    }
}

        // Método para obter o ID do validador a partir do token
        private int GetValidadorIdFromToken()
        {
            var idClaim = User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (idClaim != null)
            {
                Console.WriteLine($"Claim encontrada: {idClaim.Type} = {idClaim.Value}");
                if (int.TryParse(idClaim.Value, out int idFuncionario))
                {
                    Console.WriteLine($"ID do funcionário extraído: {idFuncionario}");
                    return idFuncionario;
                }
                else
                {
                    Console.WriteLine("Falha ao converter o valor da claim para int");
                }
            }
            else
            {
                Console.WriteLine("Claim 'sub' não encontrada no token");
            }
            return 0;
        }

        private bool IsUserAdmin(int idFuncionario)
        {
            return _funcionarioDao.IsFuncionarioAdmin(idFuncionario); // Verifica se é admin
        }

        // Endpoint de login que gera um token JWT
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Validação dos parâmetros de entrada
            if (request == null || string.IsNullOrWhiteSpace(request.NomeUsuario) || string.IsNullOrWhiteSpace(request.Senha))
            {
                return BadRequest(new { Message = "Nome de usuário e senha são obrigatórios" });
            }

            // Autenticação do usuário
            int idFuncionario = _funcionarioDao.AuthenticateUser(request.NomeUsuario, request.Senha);
            Console.WriteLine($"ID do funcionário autenticado: {idFuncionario}");

            // Verificação do resultado da autenticação
            if (idFuncionario == 0) // Assumindo que 0 significa falha na autenticação
            {
                return Unauthorized(new { Message = "Nome de usuário ou senha inválidos" });
            }

            // Recuperação do funcionário após a autenticação bem-sucedida
            var funcionario = _funcionarioDao.ReadById(idFuncionario);
            if (funcionario == null || funcionario.IsAtivo != 1)
            {
                return Unauthorized(new { Message = "Usuário não está ativo no sistema" });
            }

            // Geração do token JWT
            var token = GenerateJwtToken(idFuncionario);

            // Retorna o token gerado
            return Ok(new { Token = token });
        }

        // Função para gerar o token JWT
        private string GenerateJwtToken(int idFuncionario)
        {
            Console.WriteLine($"Gerando token para o funcionário ID: {idFuncionario}");
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, idFuncionario.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine($"Token gerado: {tokenString}");
            return tokenString;
        }
    }

    // Classe de request para o login
    public class LoginRequest
    {
        public string NomeUsuario { get; set; }
        public string Senha { get; set; }
    }
}
