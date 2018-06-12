using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;

namespace DWM.Models.Repositories
{
    public class PalpiteViewModel 
    {
        public string ticketId { get; set; }
        public string dt_compra { get; set; }
        public string cpf { get; set; }
        public string nome { get; set; }
        /// <summary>
        /// 1-Ticket Pendente, 2-Ticket Validado ou 3-Ticket Não Validado 
        /// </summary>
        public string Situacao { get; set; }
        public string Justificativa { get; set; }
        public string Consultor { get; set; }
    }
}