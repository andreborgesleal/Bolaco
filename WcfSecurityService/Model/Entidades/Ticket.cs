﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DWM.Models.Entidades
{
    [Table("Ticket")]
    public class Ticket
    {
        [Key]
        [DisplayName("PI")]
        public string ticketId { get; set; }

        [DisplayName("Cliente")]
        public int clienteId { get; set; }

        [DisplayName("Dt_Compra")]
        public DateTime dt_compra { get; set; }

        [DisplayName("Dt_Inscricao")]
        public DateTime dt_inscricao { get; set; }

        [DisplayName("Score1_Brasil")]
        public System.Nullable<int> score1Brasil { get; set; }

        [DisplayName("Score_Croacia")]
        public System.Nullable<int> score1Croacia { get; set; }

        [DisplayName("Scroe2_Brasil")]
        public System.Nullable<int> score2Brasil { get; set; }

        [DisplayName("Score_Mexico")]
        public System.Nullable<int> score2Mexico { get; set; }

        [DisplayName("Score3_Brasil")]
        public System.Nullable<int> score3Brasil { get; set; }

        [DisplayName("Score_Camaroes")]
        public System.Nullable<int> score3Camaroes { get; set; }

        [DisplayName("Selecao1_Final")]
        public System.Nullable<int> selecao1Id_Final { get; set; }

        [DisplayName("Selecao2_Final")]
        public System.Nullable<int> selecao2Id_Final { get; set; }

        [DisplayName("Score1_Final")]
        public System.Nullable<int> score1_final { get; set; }

        [DisplayName("Score2_Final")]
        public System.Nullable<int> score2_final { get; set; }

        [DisplayName("Dt_Avaliacao")]
        public System.Nullable<DateTime> dt_avaliacao { get; set; }

        [DisplayName("Ind_Avalacao")]
        public string ind_avaliacao { get; set; }
    }
}