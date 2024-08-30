using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjetoFechadura.Models;

namespace ProjetoFechadura.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalaController : ControllerBase
{
    private readonly SalaDAO _salaDao;

    public SalaController()
    {
        _salaDao = new SalaDAO();
    }



    [HttpGet]
    public IActionResult Read()
    {
        var salas = _salaDao.Read();
        return Ok(salas);
    }

    [HttpGet("{id:int}")]
    public IActionResult ReadById(int id)
    {
        var sala = _salaDao.ReadById(id);
        if (sala == null) return NotFound();
        return Ok(sala);
    }

    [HttpGet("validarCredencial")]
    public IActionResult ValidarCredencial([FromQuery] string credencialCartao = null, [FromQuery] int? credencialTeclado = null)
    {
        int idFuncionario = -1;

        if (!string.IsNullOrEmpty(credencialCartao))
        {
            idFuncionario = _salaDao.IsCredencialUsuarioCartaoValida(credencialCartao);
        }
        else if (credencialTeclado.HasValue)
        {
            idFuncionario = _salaDao.IsCredencialUsuarioTecladoValida(credencialTeclado.Value);
        }

        if (idFuncionario != -1)
        {
            return Ok(new { Message = "Credencial válida", FuncionarioId = idFuncionario });
        }

        return Unauthorized(new { Message = "Credencial inválida" });
    }

    [HttpPost]
    public IActionResult Post(Sala sala)
    {
        _salaDao.Create(sala);
        return CreatedAtAction(nameof(ReadById), new { id = sala.IdSala }, sala);
    }

    [HttpPut("{id:int}")]
    public IActionResult AtualizaUsuarioSala(int id, [FromBody] Sala sala)
    {
        if (id != sala.IdSala) return BadRequest();
        if (_salaDao.ReadById(id) == null) return NotFound();
        _salaDao.Update(id, sala);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (_salaDao.ReadById(id) == null) return NotFound();
        _salaDao.Delete(id);
        return NoContent();
    }
}
