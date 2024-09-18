using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoFechadura.Models;

namespace ProjetoFechadura.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistroController : ControllerBase
{
    private readonly RegistroDAO _registroDao;

    public RegistroController()
    {
        _registroDao = new RegistroDAO();
    }

    [HttpGet]
    [Authorize]
    public IActionResult Read()
    {
        var registros = _registroDao.Read();
        return Ok(registros);
    }

    [HttpGet("{id:int}")]
    public IActionResult ReadById(int id)
    {
        var registro = _registroDao.ReadById(id);
        if (registro == null) return NotFound();
        return Ok(registro);
    }

    [HttpPost]
    public IActionResult Post(Registro registro)
    {
        _registroDao.Create(registro);
        return CreatedAtAction(nameof(ReadById), new { id = registro.IdRegistro }, registro);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public IActionResult Put(int id, [FromBody] Registro registro)
    {
        if (id != registro.IdRegistro) return BadRequest();
        if (_registroDao.ReadById(id) == null) return NotFound();
        _registroDao.Update(id, registro);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public IActionResult Delete(int id)
    {
        if (_registroDao.ReadById(id) == null) return NotFound();
        _registroDao.Delete(id);
        return NoContent();
    }
}
