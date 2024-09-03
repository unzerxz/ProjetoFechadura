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
    private readonly FuncionarioDAO _funcionarioDao;

    public FuncionarioController()
    {
        _funcionarioDao = new FuncionarioDAO();
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
        if (id != 1){
            var funcionario = _funcionarioDao.ReadById(id);
            if (funcionario == null) return NotFound();
            return Ok(funcionario);
        }

        return NotFound();
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
        if (id != funcionario.IdFuncionario || id == 1) return BadRequest();
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

   [HttpPost("confirmacaoFuncionario")]
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


}
