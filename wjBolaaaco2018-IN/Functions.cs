using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace wjBolaaaco2018_IN
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
        public string Consultor { get; set; }
    }

    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("bolaaaco2018-in")] string message,
                                               TextWriter log)
        {
            string Filename = JsonConvert.DeserializeObject<PalpiteViewModel>(message).ticketId + ".json";

            log.WriteLine(message);

            string connectionString = AmbientConnectionStringProvider.Instance.GetConnectionString(ConnectionStringNames.Storage);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("bolaaaco2018");

            container.CreateIfNotExists();

            CloudBlockBlob blob = container.GetBlockBlobReference(Filename);

            //GetBlobSasUri(container, "bolaaaco2018", blob);

            blob.UploadText(message);

            // Salva arquivo .json na pasta "Enviados"
            PalpiteViewModel value = JsonConvert.DeserializeObject<PalpiteViewModel>(message);
            RunAsync(value, log).Wait();
        }

        static async Task RunAsync(PalpiteViewModel value, TextWriter log)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri("http://bolaco2018.azurewebsites.net/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("api/PalpiteAPI/SaveFile");
                //if (response.IsSuccessStatusCode)
                //{  //GET
                //    Produto produto = await response.Content.ReadAsAsync<Produto>();
                //    Console.WriteLine("{0}\tR${1}\t{2}", produto.Nome, produto.Preco, produto.Categoria);
                //    Console.WriteLine("Produto acessado e exibido.  Tecle algo para incluir um novo produto.");
                //    Console.ReadKey();
                //}
                //POST
                //var cha = new Produto() { Nome = "Chá Verde", Preco = 1.50M, Categoria = "Bebidas" };
                response = await client.PostAsJsonAsync<PalpiteViewModel>("api/PalpiteAPI/SaveFile", value);
                if (response.IsSuccessStatusCode)
                    log.WriteLine("Palpite " + value.ticketId + " incluído.");

                //if (response.IsSuccessStatusCode)
                //{   //PUT
                //    Uri chaUrl = response.Headers.Location;
                //    response = await client.PutAsJsonAsync(chaUrl, value);
                //    Console.WriteLine("Produto preço do atualizado. Tecle algo para excluir o produto");
                //    Console.ReadKey();
                //    //DELETE
                //    response = await client.DeleteAsync(chaUrl);
                //    Console.WriteLine("Produto deletado");
                //    Console.ReadKey();
                //}
            }
        }

        //private static string GetBlobSasUri(CloudBlobContainer container, string blobName, CloudBlockBlob blob, string policyName = null)
        //{
        //    string sasBlobToken;

        //    // Get a reference to a blob within the container.
        //    // Note that the blob may not exist yet, but a SAS can still be created for it.
        //    //CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

        //    if (policyName == null)
        //    {
        //        // Create a new access policy and define its constraints.
        //        // Note that the SharedAccessBlobPolicy class is used both to define the parameters of an ad-hoc SAS, and
        //        // to construct a shared access policy that is saved to the container's shared access policies.
        //        SharedAccessBlobPolicy adHocSAS = new SharedAccessBlobPolicy()
        //        {
        //            // When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request.
        //            // Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
        //            SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
        //            Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create
        //        };

        //        // Generate the shared access signature on the blob, setting the constraints directly on the signature.
        //        sasBlobToken = blob.GetSharedAccessSignature(adHocSAS);

        //        Console.WriteLine("SAS for blob (ad hoc): {0}", sasBlobToken);
        //        Console.WriteLine();
        //    }
        //    else
        //    {
        //        // Generate the shared access signature on the blob. In this case, all of the constraints for the
        //        // shared access signature are specified on the container's stored access policy.
        //        sasBlobToken = blob.GetSharedAccessSignature(null, policyName);

        //        Console.WriteLine("SAS for blob (stored access policy): {0}", sasBlobToken);
        //        Console.WriteLine();
        //    }

        //    // Return the URI string for the container, including the SAS token.
        //    return blob.Uri + sasBlobToken;
        //}
    }



}
