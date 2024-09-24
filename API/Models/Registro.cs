using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoFechadura.Models
{
    public class Registro
    {
        [Column("idRegistro")]
        public int IdRegistro { get; set; }

        [Column("horarioEntrada")]
        public DateTime HorarioEntrada { get; set; }

        [Column("horarioSaida")]
        public DateTime? HorarioSaida { get; set; }

        [Column("sala_idSala")]
        public int Sala_IdSala { get; set; } //Sala ocupada

        [Column("funcionario_idFuncionario")]
        public int Funcionario_IdFuncionario { get; set; } //Funcionario ocupante
    }
}