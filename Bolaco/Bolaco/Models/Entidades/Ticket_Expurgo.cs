using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel;

namespace DWM.Models.Entidades
{
    [Table("Ticket_Expurgo")]
    public class Ticket_Expurgo
    {
        [Key]
        [DisplayName("Sequencial")]
        public int sequencial { get; set; }

        [DisplayName("PI")]
        public string ticketId { get; set; }

        [DisplayName("Cliente")]
        public int clienteId { get; set; }
    }
}