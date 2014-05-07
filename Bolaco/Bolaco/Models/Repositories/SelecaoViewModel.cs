using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;

namespace DWM.Models.Repositories
{
    public class SelecaoViewModel : Repository
    {
        [DisplayName("ID")]
        public int selecaoId { get; set; }

        [DisplayName("Nome")]
        [Required(ErrorMessage = "O campo Nome da seleção dever ser informado")]
        [StringLength(25, ErrorMessage = "O nome da seleção deve ter no máximo 25 caracteres")]
        public string nome { get; set; }

        [DisplayName("Bandeira")]
        public string bandeira { get; set; }
    }
}