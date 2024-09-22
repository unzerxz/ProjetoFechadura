using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoFechadura.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace ProjetoFechadura.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistroController : ControllerBase
{
    private readonly RegistroDAO _registroDao;
    private readonly FuncionarioDAO _funcionarioDao;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    public RegistroController(IConfiguration configuration)
    {
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
        var idFuncionario = GetValidadorIdFromToken();
        if (!IsUserAdmin(idFuncionario))
        {
            return Forbid();
        }

        var registros = _registroDao.Read();
        return Ok(registros);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public IActionResult ReadById(int id)
    {
        var idFuncionario = GetValidadorIdFromToken();
        if (!IsUserAdmin(idFuncionario))
        {
            return Forbid();
        }

        var registro = _registroDao.ReadById(id);
        if (registro == null) return NotFound();
        return Ok(registro);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Post(Registro registro)
    {
        var idFuncionario = GetValidadorIdFromToken();
        if (!IsUserAdmin(idFuncionario))
        {
            return Forbid();
        }

        _registroDao.Create(registro);
        return CreatedAtAction(nameof(ReadById), new { id = registro.IdRegistro }, registro);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public IActionResult Put(int id, [FromBody] Registro registro)
    {
        var idFuncionario = GetValidadorIdFromToken();
        if (!IsUserAdmin(idFuncionario))
        {
            return Forbid();
        }

        if (id != registro.IdRegistro) return BadRequest();
        if (_registroDao.ReadById(id) == null) return NotFound();
        _registroDao.Update(id, registro);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public IActionResult Delete(int id)
    {
        var idFuncionario = GetValidadorIdFromToken();
        if (!IsUserAdmin(idFuncionario))
        {
            return Forbid();
        }

        if (_registroDao.ReadById(id) == null) return NotFound();
        _registroDao.Delete(id);
        return NoContent();
    }

    private int GetValidadorIdFromToken()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer "))
        {
            return 0;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        try
        {
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
                ValidateLifetime = true
            };

            SecurityToken validatedToken;
            var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

            var idClaim = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == "idFuncionario");

            if (idClaim != null && int.TryParse(idClaim.Value, out int idFuncionario))
            {
                return idFuncionario;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao validar o token: {ex.Message}");
        }

        return 0;
    }

    private bool IsUserAdmin(int idFuncionario)
    {
        return _funcionarioDao.IsFuncionarioAdmin(idFuncionario);
    }
}
