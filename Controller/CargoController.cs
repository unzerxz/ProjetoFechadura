using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjetoFechadura.Models;

namespace ProjetoFechadura.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CargoController : ControllerBase
{
    private readonly CargoDAO _cargoDao;

    public CargoController()
    {
        _cargoDao = new CargoDAO();
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
    public IActionResult Post(Cargo cargo)
    {
        _cargoDao.Create(cargo);
        return CreatedAtAction(nameof(ReadById), new { id = cargo.IdCargo }, cargo);
    }

    [HttpPut("{id:int}")]
    public IActionResult Put(int id, [FromBody] Cargo cargo)
    {
        if (id != cargo.IdCargo || id == 1) return BadRequest();
        if (_cargoDao.ReadById(id) == null) return NotFound();
        _cargoDao.Update(id, cargo);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (_cargoDao.ReadById(id) == null || id == 1) return NotFound();
        _cargoDao.Delete(id);
        return NoContent();
    }
}
