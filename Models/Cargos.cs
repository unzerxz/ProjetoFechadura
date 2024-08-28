using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoFechadura.Models
{
    public class Cargos
    {
        [Column("idCargo")]
        public int idCargo { get; set; }

        [Column("nomeCargo")]
        public string nomeCargo { get; set; }
    }
}