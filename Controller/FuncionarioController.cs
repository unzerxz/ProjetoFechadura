using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjetoFechadura.Models;

namespace ProjetoFechadura.Controller;

[ApiController]
[Route("api/[controller]")]
public class FuncionarioController : ControllerBase
{
    private readonly FuncionarioDao _funcionarioDao;

    public FuncionarioController()
    {
        _funcionarioDao = new FuncionarioDao();
    }

    [HttpGet]
    public IActionResult Read()
    {
        var funcionarios = _funcionarioDao.Read();
        return Ok(funcionarios);
    }

    [HttpGet("{id:int}")]
    public IActionResult ReadById(int id)
    {
        var funcionario = _funcionarioDao.ReadById(id);
        if (funcionario == null) return NotFound();
        return Ok(funcionario);
    }

    [HttpPost]
    public IActionResult Post(Funcionario funcionario)
    {
        _funcionarioDao.Create(funcionario);
        return CreatedAtAction(nameof(ReadById), new { id = funcionario.IdFuncionario }, funcionario);
    }

    [HttpPut("{id:int}")]
    public IActionResult Put(int id, [FromBody] Funcionario funcionario)
    {
        if (id != funcionario.IdFuncionario) return BadRequest();
        if (_funcionarioDao.ReadById(id) == null) return NotFound();
        _funcionarioDao.Update(id, funcionario);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (_funcionarioDao.ReadById(id) == null) return NotFound();
        _funcionarioDao.Delete(id);
        return NoContent();
    }
}
