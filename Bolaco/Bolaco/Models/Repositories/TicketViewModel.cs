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


        [DisplayName("Brasil 1ª fase")]
        public int score1Brasil { get; set; }

        public string getScore1Brasil 
        { 
            get
            {
                return score1Brasil == -1 ? "-" : score1Brasil.ToString();
            } 
        }

        public string bandeira_brasil { get; set; }

        [DisplayName("Suíça")]
        public int score1Croacia { get; set; }

        public string getScore1Croacia
        {
            get
            {
                return score1Croacia == -1 ? "-" : score1Croacia.ToString();
            }
        }

        public string bandeira_croacia { get; set; }

        [DisplayName("Brasil 1ª fase")]
        public int score2Brasil { get; set; }

        public string getScore2Brasil
        {
            get
            {
                return score2Brasil == -1 ? "-" : score2Brasil.ToString();
            }
        }

        [DisplayName("Costa Rica")]
        public int score2Mexico { get; set; }

        public string getScore2Mexico
        {
            get
            {
                return score2Mexico == -1 ? "-" : score2Mexico.ToString();
            }
        }

        public string bandeira_mexico { get; set; }

        [DisplayName("Brasil 1ª fase")]
        public int score3Brasil { get; set; }

        public string getScore3Brasil
        {
            get
            {
                return score3Brasil == -1 ? "-" : score3Brasil.ToString();
            }
        }

        [DisplayName("Sérvia")]
        public int score3Camaroes { get; set; }

        public string getScore3Camaroes
        {
            get
            {
                return score3Camaroes == -1 ? "-" : score3Camaroes.ToString();
            }
        }

        public string bandeira_camaroes { get; set; }


        [DisplayName("Brasil 8ª de Final")]
        public int score4Brasil { get; set; }

        public string getScore4Brasil
        {
            get
            {
                return score4Brasil == -1 ? "-" : score4Brasil.ToString();
            }
        }

        [DisplayName("Seleção 8ª de Final")]
        public int score4OutraSelecao { get; set; }

        public string getScore4OutraSelecao
        {
            get
            {
                return score4OutraSelecao == -1 ? "-" : score4OutraSelecao.ToString();
            }
        }

        [DisplayName("Brasil 4ª de Final")]
        public int score5Brasil { get; set; }

        public string getScore5Brasil
        {
            get
            {
                return score5Brasil == -1 ? "-" : score5Brasil.ToString();
            }
        }

        [DisplayName("Seleção 4ª de Final")]
        public int score5OutraSelecao { get; set; }

        public string getScore5OutraSelecao
        {
            get
            {
                return score5OutraSelecao == -1 ? "-" : score5OutraSelecao.ToString();
            }
        }

        [DisplayName("Brasil Semifinal")]
        public int score6Brasil { get; set; }

        public string getScore6Brasil
        {
            get
            {
                return score6Brasil == -1 ? "-" : score6Brasil.ToString();
            }
        }

        [DisplayName("Seleção Semifinal")]
        public int score6OutraSelecao { get; set; }

        public string getScore6OutraSelecao
        {
            get
            {
                return score6OutraSelecao == -1 ? "-" : score6OutraSelecao.ToString();
            }
        }

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

        public string Situacao { get; set; }

        public string GetSituacao()
        {
            switch (Situacao)
            {
                case "3":
                    return "<span class=\"strong text-danger\">Rejeitado</span>";
                case "2":
                    return "Aprovado";
                default:
                    return "Pendente";
            }
        }

        public string Justificativa { get; set; }


        [DisplayName("Palpites do cliente")]
        public IEnumerable<TicketViewModel> Palpites { get; set; }

        public IEnumerable<ParametroViewModel> Parametros { get; set; }
    }
}