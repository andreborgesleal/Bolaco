using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;

namespace DWM.Models.Repositories
{
    public class EstatisticaViewModel : Repository
    {
        public int jogo { get; set; }

        public string bandeira_selecao1 { get; set; }

        public string nome_selecao1 { get; set; }

        public int score_selecao1 { get; set; }

        public string bandeira_selecao2 { get; set; }

        public string nome_selecao2 { get; set; }

        public int score_selecao2 { get; set; }

        public int quantidade { get; set; }

        public int total { get; set; }

        public double percentual {
            get { 
                if (total > 0)
                    return (double)(quantidade / total) * 100; 
                else
                    return 0;
            }
        }
    }
}