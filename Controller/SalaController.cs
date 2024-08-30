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

    [HttpGet("validarEntradaSaida")]
    public IActionResult ValidarEntradaSaida([FromQuery] int idSala, [FromQuery] string credencialCartao = null, [FromQuery] int? credencialTeclado = null)
    {
        int idFuncionario = -1;

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
                    return Ok(new { Message = "Sala aberta com sucesso", FuncionarioId = idFuncionario, SalaId = idSala });
                }
                else
                {
                    return StatusCode(500, new { Message = "Erro ao abrir a sala" });
                }
            }
            else
            {
                // Sala ocupada, tentar fechar se o funcionário estiver ocupando a sala
                bool salaFechada = _salaDao.FecharSala(idSala, idFuncionario);
                if (salaFechada)
                {
                    return Ok(new { Message = "Sala fechada com sucesso", FuncionarioId = idFuncionario, SalaId = idSala });
                }
                else
                {
                    return Unauthorized(new { Message = "Não autorizado a fechar a sala", FuncionarioId = idFuncionario, SalaId = idSala });
                }
            }
        }
        else
        {
            return BadRequest(new { Message = "Sala não está ativa" });
        }
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

