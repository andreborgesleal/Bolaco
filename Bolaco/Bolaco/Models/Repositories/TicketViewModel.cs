﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;

namespace DWM.Models.Repositories
{
    public class TicketViewModel : Repository
    {
        [DisplayName("Número da Sorte")]
        [Required(ErrorMessage = "O campo Número da Sorte dever ser informado")]
        [StringLength(6, ErrorMessage = "Número da Sorte deve possuir 6 dígitos", MinimumLength=6)]
        public string ticketId { get; set; }

        [DisplayName("Cliente")]
        public ClienteViewModel clienteViewModel { get; set; }

        [DisplayName("Dt.Compra")]
        [Required(ErrorMessage = "O campo Data da compra dever ser informado")]
        public DateTime dt_compra { get; set; }

        public string getDt_compra 
        {
            get 
            { 
                return dt_compra.ToString("dd/MM/yyyy"); 
            }
        }
        
        [DisplayName("Dt_Inscricao")]
        public DateTime dt_inscricao { get; set; }

        public string getDt_inscricao
        {
            get
            {
                return dt_inscricao.ToString("dd/MM/yyyy HH:mm") + " h.";
            }
        }


        [DisplayName("Brasil")]
        public int score1Brasil { get; set; }

        public string getScore1Brasil 
        { 
            get
            {
                return score1Brasil == -1 ? "-" : score1Brasil.ToString();
            } 
        }

        public string bandeira_brasil { get; set; }

        [DisplayName("Croácia")]
        public int score1Croacia { get; set; }

        public string getScore1Croacia
        {
            get
            {
                return score1Croacia == -1 ? "-" : score1Croacia.ToString();
            }
        }

        public string bandeira_croacia { get; set; }

        [DisplayName("Brasil")]
        public int score2Brasil { get; set; }

        public string getScore2Brasil
        {
            get
            {
                return score2Brasil == -1 ? "-" : score2Brasil.ToString();
            }
        }

        [DisplayName("México")]
        public int score2Mexico { get; set; }

        public string getScore2Mexico
        {
            get
            {
                return score2Mexico == -1 ? "-" : score2Mexico.ToString();
            }
        }

        public string bandeira_mexico { get; set; }

        [DisplayName("Brasil")]
        public int score3Brasil { get; set; }

        public string getScore3Brasil
        {
            get
            {
                return score3Brasil == -1 ? "-" : score3Brasil.ToString();
            }
        }

        [DisplayName("Camarões")]
        public int score3Camaroes { get; set; }

        public string getScore3Camaroes
        {
            get
            {
                return score3Camaroes == -1 ? "-" : score3Camaroes.ToString();
            }
        }

        public string bandeira_camaroes { get; set; }

        [DisplayName("Seleção Finalista da Copa")]
        public int selecao1Id_Final { get; set; }

        public string bandeira_finalista1 { get; set; }

        public string nome_selecao1Final { get; set; }

        [DisplayName("Seleção Finalista da Copa")]
        public int selecao2Id_Final { get; set; }

        public string bandeira_finalista2 { get; set; }

        public string nome_selecao2Final { get; set; }

        [DisplayName("Placar da Final")]
        public int score1_final { get; set; }

        [DisplayName("Placar da Final")]
        public int score2_final { get; set; }

        [DisplayName("Dt.Avaliação")]
        public System.Nullable<DateTime> dt_avaliacao { get; set; }

        [DisplayName("Remessa ID")]
        public string remessaId { get; set; }

        [DisplayName("Palpites do cliente")]
        public IEnumerable<TicketViewModel> Palpites { get; set; }

        public IEnumerable<ParametroViewModel> Parametros { get; set; }
    }
}