using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjetoFechadura.Models;

namespace ProjetoFechadura.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerfilController : ControllerBase
{
    private readonly PerfilDAO _perfilDao;

    public PerfilController()
    {
        _perfilDao = new PerfilDAO();
    }

    [HttpGet]
    public IActionResult Read()
    {
        var perfis = _perfilDao.Read();
        return Ok(perfis);
    }

    [HttpGet("{id:int}")]
    public IActionResult ReadById(int id)
    {
        var perfil = _perfilDao.ReadById(id);
        if (perfil == null) return NotFound();
        return Ok(perfil);
    }

    [HttpPost]
    public IActionResult Post(Perfil perfil)
    {
        _perfilDao.Create(perfil);
        return CreatedAtAction(nameof(ReadById), new { id = perfil.IdPerfil }, perfil);
    }

    [HttpPut("{id:int}")]
    public IActionResult Put(int id, [FromBody] Perfil perfil)
    {
        if (id != perfil.IdPerfil) return BadRequest();
        if (_perfilDao.ReadById(id) == null) return NotFound();
        _perfilDao.Update(id, perfil);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (_perfilDao.ReadById(id) == null) return NotFound();
        _perfilDao.Delete(id);
        return NoContent();
    }
}
