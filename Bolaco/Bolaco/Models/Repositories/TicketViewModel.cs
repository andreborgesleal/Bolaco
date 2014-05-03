using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;

namespace DWM.Models.Repositories
{
    public class TicketViewModel : Repository
    {
        [DisplayName("Número do PI")]
        [Required(ErrorMessage = "O campo Número do PI dever ser informado")]
        [StringLength(6, ErrorMessage = "Número do PI deve possuir no máximo 6 dígitos")]
        public string ticketId { get; set; }

        [DisplayName("Cliente")]
        public ClienteViewModel ClienteViewModel { get; set; }

        [DisplayName("Dt.Compra")]
        [Required(ErrorMessage = "O campo Data da compra dever ser informado")]
        public DateTime dt_compra { get; set; }
        
        [DisplayName("Dt_Inscricao")]
        public DateTime dt_inscricao { get; set; }

        [DisplayName("Brasil")]
        public System.Nullable<int> score1Brasil { get; set; }

        public string bandeira_brasil { get; set; }

        [DisplayName("Croácia")]
        public System.Nullable<int> score1Croacia { get; set; }

        public string bandeira_croacia { get; set; }

        [DisplayName("Brasil")]
        public System.Nullable<int> score2Brasil { get; set; }

        [DisplayName("México")]
        public System.Nullable<int> score2Mexico { get; set; }

        public string bandeira_mexico { get; set; }

        [DisplayName("Brasil")]
        public System.Nullable<int> score3Brasil { get; set; }

        [DisplayName("Camarões")]
        public System.Nullable<int> score3Camaroes { get; set; }

        [DisplayName("Seleção Finalista da Copa")]
        public System.Nullable<int> selecao1Id_Final { get; set; }

        public string bandeira_finalista1 { get; set; }

        [DisplayName("Seleção Finalista da Copa")]
        public System.Nullable<int> selecao2Id_Final { get; set; }

        public string bandeira_finalista2 { get; set; }

        [DisplayName("Placar da Final")]
        public System.Nullable<int> score1_final { get; set; }

        [DisplayName("Placar da Final")]
        public System.Nullable<int> score2_final { get; set; }
    }
}