using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoFechadura.Models
{
    public class Sala
    {
        [Column("idSala")]
        public int IdSala { get; set; }

        [Column("identificacaoSala")]
        public string IdentificacaoSala { get;set; }

        [Column("status")] // 1 -> Ocupada/0 -> Não ocupada
        public int Status { get; set; }

        [Column("credencialSala")]
        public string? CredencialSala { get;set; }

        [Column("isAtivo")] // 1 -> Sala ativa (funciona no sistema) ou 0 -> não ativa
        public int IsAtivo { get; set; }

        [Column("funcionario_idFucionario")] // Funcionario utilizando a sala
        public int Funcionario_IdFuncionario { get; set; }
    }
}