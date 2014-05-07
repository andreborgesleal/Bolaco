using System.ComponentModel;
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
        
        [DisplayName("Dt_Inscricao")]
        public DateTime dt_inscricao { get; set; }

        [DisplayName("Brasil")]
        public int score1Brasil { get; set; }

        public string bandeira_brasil { get; set; }

        [DisplayName("Croácia")]
        public int score1Croacia { get; set; }

        public string bandeira_croacia { get; set; }

        [DisplayName("Brasil")]
        public int score2Brasil { get; set; }

        [DisplayName("México")]
        public int score2Mexico { get; set; }

        public string bandeira_mexico { get; set; }

        [DisplayName("Brasil")]
        public int score3Brasil { get; set; }

        [DisplayName("Camarões")]
        public int score3Camaroes { get; set; }

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

        [DisplayName("Palpites do cliente")]
        public IEnumerable<TicketViewModel> Palpites { get; set; }

        public IEnumerable<ParametroViewModel> Parametros { get; set; }
    }
}