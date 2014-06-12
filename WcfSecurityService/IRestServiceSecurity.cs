using App_Dominio.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Mvc;
using TimMaia.Models.Entidades;
using TimMaia.Models.Repositories;

namespace WcfSecurityService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRestServiceSecurity" in both code and config file together.
    [ServiceContract]
    public interface IRestServiceSecurity
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Autenticar/{id}/{password}/{sistemaId}")]
        AssociadoViewModel Autenticar(string id, string password, string sistemaId);
    }
}
