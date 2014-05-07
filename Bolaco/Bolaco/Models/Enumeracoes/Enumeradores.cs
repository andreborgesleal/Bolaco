using App_Dominio.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DWM.Models.Enumeracoes
{
    public class Enumeradores
    {
        public enum Param
        {
            GRUPO_USUARIO = 1,
            SISTEMA = 3,
            EMPRESA = 4,
            EMAIL_ADMIN = 5,
            HABILITA_EMAIL = 6,
            FUSO = 7,
            DT_INICIO_PROMOCAO = 8,
            DT_FIM_PROMOCAO = 9,
            SCORE1_BRASIL = 10,
            SCORE1_CROACIA = 11,
            SCORE2_BRASIL = 12,
            SCORE2_MEXICO = 13,
            SCORE3_BRASIL = 14,
            SCORE3_CAMAROES = 15,
            SCORE1_FINAL = 16,
            SCORE2_FINAL = 17,
            SELECAO1_FINAL = 18,
            SELECAO2_FINAL = 19
        }
    }
}