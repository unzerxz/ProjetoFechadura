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
        public int IdPerfil { get; set; }

        [Column("tipoPerfil")]
        public int TipoPerfil {get;set;} //0 -> Usuário; 1 -> Admin
    }
}