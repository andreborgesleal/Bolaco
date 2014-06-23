using App_Dominio.Component;

namespace DWM.Models.Repositories
{
    public class ResumoGerencial3ViewModel : Repository
    {
        public string nome { get; set; }

        public string email { get; set; }

        public System.DateTime dt_cadastro { get; set; }
    }
}