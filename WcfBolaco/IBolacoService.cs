using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using DWM.Models.Repositories;

namespace WcfBolaco
{
    public interface IBolacoService
    {
        [ServiceContract]
        public interface IRestServiceSecurity
        {
            [OperationContract]
            [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "RetornaPI/{id}/{password}/{sistemaId}")]
            IEnumerable<TicketViewModel> RetornaPI(string id, string password, string sistemaId);
        }
    }
}
