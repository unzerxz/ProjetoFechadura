using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ProjetoFechadura.Models
{
    public class Perfil
    {
        [Column("idPerfil")]
        public int IdPerfil { get; set; } //1 -> Indefinido; 2 -> Usuário, 3- Admin

        [Column("tipoPerfil")]
        public string? TipoPerfil {get;set;} 
    }
}