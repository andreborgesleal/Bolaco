using App_Dominio.Component;
using System;

namespace DWM.Models.Repositories
{
    public class ResumoGerencial4ViewModel : Repository
    {
        public int dia { get; set; }

        public int mes { get; set; }

        public int ano { get; set; }

        public System.DateTime dt_palpite 
        {
            get { return new DateTime(ano, mes, dia); }
        }

        public int qte_palpites { get; set; }

    }
}