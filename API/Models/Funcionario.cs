using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoFechadura.Models
{
    public class Funcionario
    {
        [Column("idFuncionario")] // 0 -> sentinela
        public int IdFuncionario { get; set; }

        [Column("Nome")]
        public string Nome { get; set; }

        [Column("NomeUsuario")]
        public string NomeUsuario { get; set; }
        
        [Column("email")]
        public string Email { get; set; } // Novo atributo

        [Column("credencialCartao")]
        public string? CredencialCartao { get; set; }

        [Column("credencialTeclado")]
        public int? CredencialTeclado { get; set; }

        [Column("senha")]
        public string Senha { get; set; }

        [Column("isAtivo")]
        public int IsAtivo { get; set; }

        [Column("cargo_idCargo")]
        public int Cargo_IdCargo { get; set; }

        [Column("perfil_idPerfil")]
        public int Perfil_IdPerfil { get; set; } // 0 -> sentinela, 1 -> usuario, 2 -> admin

    }
}