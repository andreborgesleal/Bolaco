using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DWM.Models.Entidades
{
    [Table("Cliente")]
    public class Cliente
    {
        [Key]
        [DisplayName("ID")]
        public int clienteId { get; set; }

        [DisplayName("Nome")]
        public string nome { get; set; }
        [DisplayName("CPF")]
        public string cpf { get; set; }
        [DisplayName("E_Mail")]
        public string email { get; set; }
        [DisplayName("Telefone")]
        public string telefone { get; set; }
        [DisplayName("Endreco")]
        public string endereco { get; set; }
        [DisplayName("Complemento")]
        public string complemento { get; set; }
        [DisplayName("Cidade")]
        public string cidade { get; set; }
        [DisplayName("UF")]
        public string uf { get; set; }
        [DisplayName("Cep")]
        public string cep { get; set; }
        [DisplayName("Usuario")]
        public int usuarioId { get; set; }

    }
}