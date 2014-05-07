using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;

namespace DWM.Models.Repositories
{
    public class ParametroViewModel : Repository
    {
        public int paramId { get; set; }

        public string valor { get; set; }

    }
}