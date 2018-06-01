using DWM.Controllers.API;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Bolaco.Controllers
{
    public class PalpiteViewModel
    {
        public string ticketId { get; set; }
        public string dt_compra { get; set; }
        public string cpf { get; set; }
        public string nome { get; set; }
        /// <summary>
        /// 1-Ticket Pendente, 2-Ticket Validado ou 3-Ticket Não Validado 
        /// </summary>
        public string Situacao { get; set; }
        public string Justificativa { get; set; }
    }

    public class PalpiteArquivoViewModel
    {
        public string Name { get; set; }
        public string URI { get; set; }
    }

    public class PalpiteAPIController : DwmApiController
    {
        [HttpGet]
        public IList<PalpiteArquivoViewModel> Files()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=dwmsistemas;AccountKey=mYOiPtcUPSPwtCUipbcm+iSX1kD1Uap7u34VbJlhT4o5Q8eO9lLHfIUX8Y/DfvoLpGhoGClOLYBhXchpwyvoeg==";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("bolaaaco2018");

            IEnumerable<IListBlobItem> list = container.ListBlobs(null, false);
            IList<PalpiteArquivoViewModel> listArquivo = new List<PalpiteArquivoViewModel>();

            foreach (IListBlobItem item in list)
            {
                SharedAccessBlobPolicy adHocSAS = new SharedAccessBlobPolicy()
                {
                    // When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request.
                    // Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                    Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write 
                };

                string SASToken = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlockBlob)item).GetSharedAccessSignature(adHocSAS);

                PalpiteArquivoViewModel value = new PalpiteArquivoViewModel()
                {
                    URI = item.Uri.ToString() + SASToken,
                    Name = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlockBlob)item).Name
                };

                listArquivo.Add(value);
            }

            return listArquivo;
        }

        [HttpGet]
        public IList<PalpiteViewModel> Palpites()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=dwmsistemas;AccountKey=mYOiPtcUPSPwtCUipbcm+iSX1kD1Uap7u34VbJlhT4o5Q8eO9lLHfIUX8Y/DfvoLpGhoGClOLYBhXchpwyvoeg==";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("bolaaaco2018");

            IEnumerable<IListBlobItem> list = container.ListBlobs(null, false);
            IList<PalpiteViewModel> listArquivo = new List<PalpiteViewModel>();

            foreach (IListBlobItem item in list)
            {
                string p = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlockBlob)item).DownloadText();
                PalpiteViewModel value = JsonConvert.DeserializeObject<PalpiteViewModel>(p);
                listArquivo.Add(value);
            }

            return listArquivo;
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}