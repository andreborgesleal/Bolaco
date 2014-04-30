using App_Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DWM.Models.Entidades
{
    public class ApplicationContext : App_DominioContext
    {
        public DbSet<Cliente> Clientes { get; set;  }
        public DbSet<Selecao> Selecaos { get; set; }
        public DbSet<Parametro> Parametros { get; set; }
    }
}
