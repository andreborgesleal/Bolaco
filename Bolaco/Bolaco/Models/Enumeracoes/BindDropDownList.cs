using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App_Dominio.Entidades;
using DWM.Models.Entidades;
using App_Dominio.Security;
using App_Dominio.Models;

namespace DWM.Models.Enumeracoes
{
    public class BindDropDownList
    {
       
        public IEnumerable<SelectListItem> Selecao(params object[] param)
        {
            // params[0] -> cabeçalho (Selecione..., Todos...)
            // params[1] -> SelectedValue
            string cabecalho = param[0].ToString();
            string selectedValue = param[1].ToString();

            //IList<SelectListItem> q = new List<SelectListItem>();

            // if (cabecalho != "")
            //    q.Add(new SelectListItem() { Value = "", Text = cabecalho });

            //q.Add(new SelectListItem() { Value = "1", Text = "Brasil" });
            //q.Add(new SelectListItem() { Value = "2", Text = "Inglaterra" });
            //q.Add(new SelectListItem() { Value = "3", Text = "Alemanha" });
            //q.Add(new SelectListItem() { Value = "4", Text = "Espanha" });
            //q.Add(new SelectListItem() { Value = "5", Text = "Itália" });

            //return q;

            using (ApplicationContext db = new ApplicationContext())
            {
                IList<SelectListItem> q = new List<SelectListItem>();

                if (cabecalho != "")
                    q.Add(new SelectListItem() { Value = "", Text = cabecalho });

                q = q.Union(from e in db.Selecaos.AsEnumerable()
                            orderby e.nome
                            select new SelectListItem()
                            {
                                Value = e.selecaoId.ToString(),
                                Text = e.nome,
                                Selected = (selectedValue != "" ? e.nome.Equals(selectedValue) : false)
                            }).ToList();

                return q;
            }
        }

        #region DropDownList Horario
        /// <summary>
        /// Retorna os valores constantes dos horários dos funcionário do condômino
        /// </summary>
        /// <param name="selectedValue">Item da lista que receberá o foco inicial</param>
        /// <param name="header">Informar o cabeçalho do dropdownlist. Exemplo: "Selecione...". Observação: Se não informado o dropdownlist não terá cabeçalho.</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> Horario(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = "00:00", Text = "00:00" }, 
                new SelectListItem() { Value = "01:00", Text = "01:00" }, 
                new SelectListItem() { Value = "01:30", Text = "01:30" },
                new SelectListItem() { Value = "02:00", Text = "02:00" }, 
                new SelectListItem() { Value = "02:30", Text = "02:30" }, 
                new SelectListItem() { Value = "03:00", Text = "03:00" }, 
                new SelectListItem() { Value = "03:30", Text = "03:30" }, 
                new SelectListItem() { Value = "04:00", Text = "04:00" }, 
                new SelectListItem() { Value = "04:30", Text = "04:30" }, 
                new SelectListItem() { Value = "05:00", Text = "05:00" }, 
                new SelectListItem() { Value = "05:30", Text = "05:30" }, 
                new SelectListItem() { Value = "06:00", Text = "06:00" }, 
                new SelectListItem() { Value = "06:30", Text = "06:30" }, 
                new SelectListItem() { Value = "07:00", Text = "07:00" }, 
                new SelectListItem() { Value = "07:30", Text = "07:30" }, 
                new SelectListItem() { Value = "08:00", Text = "08:00" }, 
                new SelectListItem() { Value = "08:30", Text = "08:30" }, 
                new SelectListItem() { Value = "09:00", Text = "09:00" }, 
                new SelectListItem() { Value = "09:30", Text = "09:30" }, 
                new SelectListItem() { Value = "10:00", Text = "10:00" }, 
                new SelectListItem() { Value = "10:30", Text = "10:30" }, 
                new SelectListItem() { Value = "11:00", Text = "11:00" }, 
                new SelectListItem() { Value = "11:30", Text = "11:30" }, 
                new SelectListItem() { Value = "12:00", Text = "12:00" }, 
                new SelectListItem() { Value = "12:30", Text = "12:30" }, 
                new SelectListItem() { Value = "13:00", Text = "13:00" }, 
                new SelectListItem() { Value = "13:30", Text = "13:30" }, 
                new SelectListItem() { Value = "14:00", Text = "14:00" }, 
                new SelectListItem() { Value = "14:30", Text = "14:30" }, 
                new SelectListItem() { Value = "15:00", Text = "15:00" }, 
                new SelectListItem() { Value = "15:30", Text = "15:30" }, 
                new SelectListItem() { Value = "16:00", Text = "16:00" }, 
                new SelectListItem() { Value = "16:30", Text = "16:30" }, 
                new SelectListItem() { Value = "17:00", Text = "17:00" }, 
                new SelectListItem() { Value = "17:30", Text = "17:30" }, 
                new SelectListItem() { Value = "18:00", Text = "18:00" }, 
                new SelectListItem() { Value = "18:30", Text = "18:30" }, 
                new SelectListItem() { Value = "19:00", Text = "19:00" }, 
                new SelectListItem() { Value = "19:30", Text = "19:30" }, 
                new SelectListItem() { Value = "20:00", Text = "20:00" }, 
                new SelectListItem() { Value = "20:30", Text = "20:30" }, 
                new SelectListItem() { Value = "21:00", Text = "21:00" }, 
                new SelectListItem() { Value = "21:30", Text = "21:30" }, 
                new SelectListItem() { Value = "22:00", Text = "22:00" }, 
                new SelectListItem() { Value = "22:30", Text = "22:30" }, 
                new SelectListItem() { Value = "23:00", Text = "23:00" }, 
                new SelectListItem() { Value = "23:30", Text = "23:30" }, 
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion


        #region DropDownList Dia do Mes
        /// <summary>
        /// Retorna os valores constantes dos 31 dias do mês
        /// </summary>
        /// <param name="selectedValue">Item da lista que receberá o foco inicial</param>
        /// <param name="header">Informar o cabeçalho do dropdownlist. Exemplo: "Selecione...". Observação: Se não informado o dropdownlist não terá cabeçalho.</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> CalendarDay(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = "", Text = "dd" }, 
                new SelectListItem() { Value = "01", Text = "01" }, 
                new SelectListItem() { Value = "02", Text = "02" }, 
                new SelectListItem() { Value = "03", Text = "03" },
                new SelectListItem() { Value = "04", Text = "04" }, 
                new SelectListItem() { Value = "05", Text = "05" }, 
                new SelectListItem() { Value = "06", Text = "06" }, 
                new SelectListItem() { Value = "07", Text = "07" }, 
                new SelectListItem() { Value = "08", Text = "08" }, 
                new SelectListItem() { Value = "09", Text = "09" }, 
                new SelectListItem() { Value = "10", Text = "10" }, 
                new SelectListItem() { Value = "11", Text = "11" }, 
                new SelectListItem() { Value = "12", Text = "12" }, 
                new SelectListItem() { Value = "13", Text = "13" }, 
                new SelectListItem() { Value = "14", Text = "14" }, 
                new SelectListItem() { Value = "15", Text = "15" }, 
                new SelectListItem() { Value = "16", Text = "16" }, 
                new SelectListItem() { Value = "17", Text = "17" }, 
                new SelectListItem() { Value = "18", Text = "18" }, 
                new SelectListItem() { Value = "19", Text = "19" }, 
                new SelectListItem() { Value = "20", Text = "20" }, 
                new SelectListItem() { Value = "21", Text = "21" }, 
                new SelectListItem() { Value = "22", Text = "22" }, 
                new SelectListItem() { Value = "23", Text = "23" }, 
                new SelectListItem() { Value = "24", Text = "24" }, 
                new SelectListItem() { Value = "25", Text = "25" }, 
                new SelectListItem() { Value = "26", Text = "26" }, 
                new SelectListItem() { Value = "27", Text = "27" }, 
                new SelectListItem() { Value = "28", Text = "28" }, 
                new SelectListItem() { Value = "29", Text = "29" }, 
                new SelectListItem() { Value = "30", Text = "30" }, 
                new SelectListItem() { Value = "31", Text = "31" }, 
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion


        #region DropDownList Mês
        /// <summary>
        /// Retorna os valores constantes dos 12 meses do ano
        /// </summary>
        /// <param name="selectedValue">Item da lista que receberá o foco inicial</param>
        /// <param name="header">Informar o cabeçalho do dropdownlist. Exemplo: "Selecione...". Observação: Se não informado o dropdownlist não terá cabeçalho.</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> CalendarMonth(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = "", Text = "mm" }, 
                new SelectListItem() { Value = "01", Text = "01" }, 
                new SelectListItem() { Value = "02", Text = "02" }, 
                new SelectListItem() { Value = "03", Text = "03" },
                new SelectListItem() { Value = "04", Text = "04" }, 
                new SelectListItem() { Value = "05", Text = "05" }, 
                new SelectListItem() { Value = "06", Text = "06" }, 
                new SelectListItem() { Value = "07", Text = "07" }, 
                new SelectListItem() { Value = "08", Text = "08" }, 
                new SelectListItem() { Value = "09", Text = "09" }, 
                new SelectListItem() { Value = "10", Text = "10" }, 
                new SelectListItem() { Value = "11", Text = "11" }, 
                new SelectListItem() { Value = "12", Text = "12" }, 
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion


        #region DropDownList Mês
        /// <summary>
        /// Retorna os valores constantes dos 12 meses do ano
        /// </summary>
        /// <param name="selectedValue">Item da lista que receberá o foco inicial</param>
        /// <param name="header">Informar o cabeçalho do dropdownlist. Exemplo: "Selecione...". Observação: Se não informado o dropdownlist não terá cabeçalho.</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> CalendarYear(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = "", Text = "aaaa" }, 
                new SelectListItem() { Value = "2014", Text = "2014" }, 
                new SelectListItem() { Value = "2015", Text = "2015" }, 
                new SelectListItem() { Value = "2016", Text = "2016" }, 
                new SelectListItem() { Value = "2017", Text = "2017" },  
                new SelectListItem() { Value = "2018", Text = "2018" },  
                new SelectListItem() { Value = "2019", Text = "2019" },  
                new SelectListItem() { Value = "2020", Text = "2020" },  
                new SelectListItem() { Value = "2021", Text = "2021" },  
                new SelectListItem() { Value = "2022", Text = "2022" },  
                new SelectListItem() { Value = "2023", Text = "2023" },  
                new SelectListItem() { Value = "2024", Text = "2024" },  
                new SelectListItem() { Value = "2025", Text = "2025" },  
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion
    }
}