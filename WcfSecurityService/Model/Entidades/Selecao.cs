using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;


namespace DWM.Models.Entidades
{
    [Table("Selecao")]
    public class Selecao
    {
        [Key]
        [DisplayName("ID")]
        public int selecaoId { get; set; }

        [DisplayName("Nome")]
        public string nome { get; set; }

        [DisplayName("Bandeira")]
        public string bandeira { get; set; }
    }
}