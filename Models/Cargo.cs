using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoFechadura.Models
{
    public class Cargo
    {
        [Column("idCargo")] // 0 -> sentinela = "Indefinido"
        public int IdCargo { get; set; } 

        [Column("nomeCargo")]
        public string NomeCargo { get; set; }
    }
}