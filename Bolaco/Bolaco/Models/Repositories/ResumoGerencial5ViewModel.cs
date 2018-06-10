using App_Dominio.Component;

namespace DWM.Models.Repositories
{
    public class ResumoGerencial5ViewModel : Repository
    {
        public int total_dias { get; set; }

        public int total_cadastros { get; set; }

        public int total_palpites { get; set; }

        public int total_palpites_rejeitados { get; set; }

        public int total_palpites_pendentes { get; set; }

        public int total_palpites_aprovados { get; set; }

        public int media_diaria_palpite { get; set; }
    }
}