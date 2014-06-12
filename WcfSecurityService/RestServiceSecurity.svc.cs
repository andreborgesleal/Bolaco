using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Repositories;
using App_Dominio.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Mvc;
using TimMaia.Models.Entidades;
using TimMaia.Models.Persistence;
using TimMaia.Models.Repositories;


namespace WcfSecurityService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RestServiceSecurity" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select RestServiceSecurity.svc or RestServiceSecurity.svc.cs at the Solution Explorer and start debugging.
    public class RestServiceSecurity : IRestServiceSecurity
    {
        //public JsonResult Autenticar(string login, string password, int sistemaId)
        public AssociadoViewModel Autenticar(string id, string password, string sistemaId)
        {
            EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();

            AssociadoViewModel associado = new AssociadoViewModel();

            Validate validate = security.Autenticar(id, password, int.Parse(sistemaId));

            if (validate.Code == 0)
            {
                AssociadoModel model = new AssociadoModel();
                associado = model.getAssociadoByLogin(id);
            }

            return associado;

            //return new JsonResult()
            //{
            //    Data = usuarioRepository,
            //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
            //};
        }
    }
}
