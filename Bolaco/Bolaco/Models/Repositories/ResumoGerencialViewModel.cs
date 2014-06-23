using App_Dominio.Component;
using System.Collections.Generic;

namespace DWM.Models.Repositories
{
    public class ResumoGerencialViewModel : Repository
    {
        public IEnumerable<ResumoGerencial1ViewModel> resumo1 { get; set; }
        public IEnumerable<ResumoGerencial2ViewModel> resumo2 { get; set; }
        public IEnumerable<ResumoGerencial3ViewModel> resumo3 { get; set; }
        public IEnumerable<ResumoGerencial4ViewModel> resumo4 { get; set; }
        public ResumoGerencial5ViewModel resumo5 { get; set; }
    }
}