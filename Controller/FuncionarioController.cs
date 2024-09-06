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
        private readonly string _jwtSecret = "vX0/Yt+K2@J3dN5r4K8kzG$9XbFs5Wq3p!zC&6L7L8O2H9I0J0";

        public FuncionarioController()
        {
            _funcionarioDao = new FuncionarioDAO();
        }

        // Endpoint para obter todos os funcionários
        [HttpGet]
        [Authorize]
        public IActionResult Read()
        {
            var funcionarios = _funcionarioDao.Read();
            return Ok(funcionarios);
        }

        // Endpoint para obter funcionário por ID
        [HttpGet("{id:int}")]
        [Authorize]
        public IActionResult ReadById(int id)
        {
            if (id != 1)
            {
                var funcionario = _funcionarioDao.ReadById(id);
                if (funcionario == null) return NotFound();
                return Ok(funcionario);
            }

            return NotFound();
        }

        // Endpoint para criar um novo funcionário
        [HttpPost]
        public IActionResult Post(Funcionario funcionario)
        {
            _funcionarioDao.Create(funcionario);
            return CreatedAtAction(nameof(ReadById), new { id = funcionario.IdFuncionario }, funcionario);
        }

        // Endpoint para atualizar um funcionário existente
        [HttpPut("{id:int}")]
        [Authorize]
        public IActionResult Put(int id, [FromBody] Funcionario funcionario)
        {
            if (id != funcionario.IdFuncionario || id == 1) return BadRequest();
            if (_funcionarioDao.ReadById(id) == null) return NotFound();
            _funcionarioDao.Update(id, funcionario);
            return NoContent();
        }

        // Endpoint para deletar um funcionário
        [HttpDelete("{id:int}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            if (_funcionarioDao.ReadById(id) == null || id == 1) return NotFound();
            _funcionarioDao.Delete(id);
            return NoContent();
        }

        [HttpPost("confirmacaoFuncionario")]
[Authorize]
public IActionResult ConfirmacaoFuncionario(
    [FromQuery] int idFuncionario,
    [FromQuery] int novoCargoId,
    [FromQuery] int novoPerfilId)
{
    if (idFuncionario <= 1)
    {
        return BadRequest(new { Message = "ID do Funcionário inválido" });
    }

    try
    {
        // Recuperar o id do validador do token JWT
        var validadorIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        if (validadorIdClaim == null)
        {
            return Unauthorized(new { Message = "Token inválido ou não autorizado" });
        }

        int validadorId = int.Parse(validadorIdClaim.Value);

        // Recuperar os dados do validador pelo ID
        var validador = _funcionarioDao.ReadById(validadorId);

        if (validador == null)
        {
            return NotFound(new { Message = "Validador não encontrado" });
        }

        // Garantir que o validador tem o perfil adm (perfilId = 3)
        if (validador.Perfil_IdPerfil != 3)
        {
            return BadRequest(new { Message = "Usuário não autorizado para promover funcionários" });
        }

        // Recuperar os dados atuais do funcionário pelo ID
        var funcionarioAtual = _funcionarioDao.ReadById(idFuncionario);

        if (funcionarioAtual == null)
        {
            return NotFound(new { Message = "Funcionário não encontrado" });
        }

        // Garantir que o funcionário não está atualmente ativo
        if (funcionarioAtual.IsAtivo == 1)
        {
            return BadRequest(new { Message = "Funcionário já está ativo" });
        }

        // Gerar uma credencial de cartão única
        string credencialCartao = _funcionarioDao.GenerateUniqueCredencialCartao(funcionarioAtual);

        // Gerar uma senha de teclado única
        int credencialTeclado = int.Parse(_funcionarioDao.GenerateUniqueRandomPassword());

        // Atualizar os campos para ativar o funcionário
        funcionarioAtual.IsAtivo = 1;  // Ativar o funcionário
        funcionarioAtual.CredencialCartao = credencialCartao;  // Definir nova credencial de cartão
        funcionarioAtual.CredencialTeclado = credencialTeclado;  // Definir nova senha de teclado
        funcionarioAtual.Cargo_IdCargo = novoCargoId;  // Atualizar o cargo
        funcionarioAtual.Perfil_IdPerfil = novoPerfilId;  // Atualizar o perfil

        // Atualizar o funcionário no banco de dados
        _funcionarioDao.Update(idFuncionario, funcionarioAtual);

        return Ok(new { Message = "Funcionário promovido e ativado com sucesso", Funcionario = funcionarioAtual });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { Message = "Erro ao promover o funcionário", Error = ex.Message });
    }
}


        // Endpoint de login que gera um token JWT
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.NomeUsuario) || string.IsNullOrWhiteSpace(request.Senha))
            {
                return BadRequest(new { Message = "Nome de usuário e senha são obrigatórios" });
            }

            int idFuncionario = _funcionarioDao.AuthenticateUser(request.NomeUsuario, request.Senha);
            if (idFuncionario == 1)
            {
                return Unauthorized(new { Message = "Nome de usuário ou senha inválidos" });
            }

            var funcionario = _funcionarioDao.ReadById(idFuncionario);
            if (funcionario == null || funcionario.IsAtivo != 1)
            {
                return Unauthorized(new { Message = "Usuário não está ativo no sistema" });
            }

            var token = GenerateJwtToken(idFuncionario);
            return Ok(new { Token = token });
        }

        // Função para gerar o token JWT
        private string GenerateJwtToken(int idFuncionario)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, idFuncionario.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

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
