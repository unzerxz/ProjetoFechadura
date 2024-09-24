using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoFechadura.Models
{
    [Table("tokenFuncionario")]
    public class TokenFuncionario
    {
        [Key]
        [Column("idToken")]
        public int IdToken { get; set; }

        [Required]
        [Column("token")]
        [StringLength(500)] // Atualize este valor para corresponder ao novo tamanho da coluna
        public string Token { get; set; }

        [Required]
        [Column("timeExpiracao")]
        public DateTime TimeExpiracao { get; set; }

        [Required]
        [Column("funcionario_idFuncionario")]
        public int FuncionarioId { get; set; }

        [ForeignKey("FuncionarioId")]
        public virtual Funcionario Funcionario { get; set; }
    }
}