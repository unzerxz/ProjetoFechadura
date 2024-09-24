using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjetoFechadura.DAO;
using ProjetoFechadura.Models;

namespace ProjetoFechadura.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CargoController : ControllerBase
{
    private readonly CargoDAO _cargoDao;
    private readonly FuncionarioDAO _funcionarioDao;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    

    public CargoController(IConfiguration configuration)
    {
        _cargoDao = new CargoDAO();
        _funcionarioDao = new FuncionarioDAO();
        _jwtSecret = configuration.GetValue<string>("JwtSettings:SecretKey");
        _jwtIssuer = configuration.GetValue<string>("JwtSettings:Issuer");
        _jwtAudience = configuration.GetValue<string>("JwtSettings:Audience");
    }

    [HttpGet]
    public IActionResult Read()
    {
        var cargos = _cargoDao.Read();
        return Ok(cargos);
    }

    [HttpGet("{id:int}")]
    public IActionResult ReadById(int id)
    {
        var cargo = _cargoDao.ReadById(id);
        if (cargo == null) return NotFound();
        return Ok(cargo);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Post(Cargo cargo)
    {
        var idFuncionario = GetValidadorIdFromToken();
        if (!IsUserAdmin(idFuncionario)) return Forbid();

        var novoId = _cargoDao.Create(cargo);
        if (novoId == 0) return StatusCode(500, new { Message = "Erro ao criar cargo" });

        var cargoCriado = _cargoDao.ReadById(novoId);
        return CreatedAtAction(nameof(ReadById), new { id = cargoCriado.IdCargo }, cargoCriado);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public IActionResult Put(int id, [FromBody] Cargo cargo)
    {
        var idFuncionario = GetValidadorIdFromToken();
        if (!IsUserAdmin(idFuncionario)) return Forbid();

        if (id != cargo.IdCargo || id == 1) return BadRequest(new { Message = "ID inválido ou tentativa de modificar o cargo padrão." });
        if (_cargoDao.ReadById(id) == null) return NotFound(new { Message = "Cargo não encontrado" });

        _cargoDao.Update(id, cargo);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public IActionResult Delete(int id)
    {
        var idFuncionario = GetValidadorIdFromToken();
        if (!IsUserAdmin(idFuncionario)) return Forbid();

        if (_cargoDao.ReadById(id) == null || id == 1) return NotFound(new { Message = "Cargo não encontrado ou tentativa de deletar o cargo padrão." });
        _cargoDao.Delete(id);
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
}
