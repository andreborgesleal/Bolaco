using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Enumeracoes;
using App_Dominio.Models;
using App_Dominio.Security;
using DWM.Controllers.API;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
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
        public string Consultor { get; set; }
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
        public IList<PalpiteArquivoViewModel> Enviados()
        {
            string FolderName = "Enviados";
            IList<PalpiteArquivoViewModel> listArquivo = new List<PalpiteArquivoViewModel>();
            string[] files = Directory.GetFiles(@"d:\home\site\wwwroot\Data\" + FolderName + @"\");
            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                PalpiteArquivoViewModel value = new PalpiteArquivoViewModel()
                {
                    URI = "http://bolaco2018.azurewebsites.net/Data/" + FolderName + "/" + info.Name,
                    Name = info.Name
                };

                listArquivo.Add(value);
            }

            return listArquivo;
        }

        [HttpGet]
        public IList<PalpiteArquivoViewModel> Validados()
        {
            string FolderName = "Validados";
            IList<PalpiteArquivoViewModel> listArquivo = new List<PalpiteArquivoViewModel>();
            string[] files = Directory.GetFiles(@"d:\home\site\wwwroot\Data\" + FolderName + @"\");
            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                PalpiteArquivoViewModel value = new PalpiteArquivoViewModel()
                {
                    URI = "http://bolaco2018.azurewebsites.net/Data/" + FolderName + "/" + info.Name,
                    Name = info.Name
                };

                listArquivo.Add(value);
            }

            return listArquivo;
        }

        [HttpGet]
        public IList<PalpiteArquivoViewModel> Processados()
        {
            string FolderName = "Processados";
            IList<PalpiteArquivoViewModel> listArquivo = new List<PalpiteArquivoViewModel>();
            string[] files = Directory.GetFiles(@"d:\home\site\wwwroot\Data\" + FolderName + @"\");
            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                PalpiteArquivoViewModel value = new PalpiteArquivoViewModel()
                {
                    URI = "http://bolaco2018.azurewebsites.net/Data/" + FolderName + "/" + info.Name,
                    Name = info.Name
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

        [HttpGet]
        public ResumoGerencialViewModel ResumoGerencial()
        {
            ListViewResumoGerencial resumo = new ListViewResumoGerencial();
            ResumoGerencialViewModel repository = new ResumoGerencialViewModel();
            using (ApplicationContext db = new ApplicationContext())
                repository = ((IEnumerable<ResumoGerencialViewModel>)resumo.Bind(db, 0, 1000, null)).FirstOrDefault();

            return repository;
        }

        [HttpPost]
        public string SaveFile(PalpiteViewModel message)
        {
            string Filename = message.ticketId + ".json";
            System.IO.FileInfo f = new FileInfo(@"d:\home\site\wwwroot\Data\Enviados\" + Filename);
            StreamWriter ws = f.CreateText();
            ws.WriteLine(JsonConvert.SerializeObject(message));
            ws.Close();
            return "Sucesso!!!";
        }

        [HttpGet]
        public string CheckFiles()
        {
            string message = "Sucesso!!!";
            try
            {
                string[] files = Directory.GetFiles(@"d:\home\site\wwwroot\Data\Validados\");
                //string[] files = Directory.GetFiles(@"C:\Sistemas\Bolaco\Bolaco\Bolaco\Data\Validados\");
                if (files.Count() > 0)
                    using (ApplicationContext db = new ApplicationContext())
                        foreach (string file in files)
                        {
                            #region carrega o json e deserializa no objeto PalpiteViewModel
                            System.IO.FileInfo f = new FileInfo(file);
                            StreamReader fs = f.OpenText();
                            PalpiteViewModel value = JsonConvert.DeserializeObject<PalpiteViewModel>(fs.ReadToEnd());
                            fs.Close();
                            #endregion

                            if ("2|3".Contains(value.Situacao))
                            {
                                Ticket entity = db.Tickets.Find(value.ticketId);
                                if (entity != null)
                                {
                                    #region Atualiza o palpite
                                    entity.Situacao = value.Situacao;
                                    entity.Justificativa = value.Justificativa;
                                    entity.Consultor = value.Consultor;
                                    db.Entry(entity).State = EntityState.Modified;
                                    db.SaveChanges();
                                    #endregion

                                    #region Move o arquivo para a pasta "Processados"
                                    FileInfo info = new FileInfo(file);
                                    info.MoveTo(@"d:\home\site\wwwroot\Data\Processados\" + info.Name + "." + System.DateTime.Now.Second.ToString() + System.DateTime.Now.Millisecond.ToString());
                                    #endregion

                                    #region Envia o SMS para os clientes que não foram validados (palpites rejeitados)
                                    string _CHAVE_SMS = "0876f1a3-44db-48e8-8393-256c1cbd312a";
                                    Cliente cliente = db.Clientes.Find(entity.clienteId);
                                    string ret = "";
                                    if (cliente.telefone != null && cliente.telefone.Trim().Length > 0)
                                    {
                                        if (value.Situacao == "3")
                                            ret = Torpedo.EnviarSMS(_CHAVE_SMS, "Norte Refrigeracao", cliente.telefone, "[Bolaaaco 2018] Seu palpite com o numero da sorte [" + entity.ticketId + "] nao foi validado. Motivo [" + value.Justificativa.Trim() + "]. Favor entrar em contato com nossa loja.");
                                        else
                                            ret = Torpedo.EnviarSMS(_CHAVE_SMS, "Norte Refrigeracao", cliente.telefone, "[Bolaaaco 2018] Parabens, seu palpite foi validado. Boa sorte!");
                                        if (ret.Trim().Length > 0)
                                        {
                                            throw new App_DominioException(new Validate()
                                            {
                                                Code = 60,
                                                Message = MensagemPadrao.Message(60, ret).ToString(),
                                                MessageBase = ret,
                                                MessageType = MsgType.WARNING
                                            });
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
            }
             catch(App_DominioException ex)
            {
                return ex.Result.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return message;
        }

        [HttpGet]
        public string CreateFiles()
        {
            return "ok";
            string message = "Sucesso!!!";
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    foreach (Ticket ticket in db.Tickets)
                    {
                        PalpiteViewModel value = new PalpiteViewModel()
                        {
                            ticketId = ticket.ticketId,
                            dt_compra = ticket.dt_compra.ToString("yyyy-MM-dd"),
                            cpf = db.Clientes.Find(ticket.clienteId).cpf,
                            nome = db.Clientes.Find(ticket.clienteId).nome,
                            Situacao = "1",
                            Justificativa = "", //ticket.Justificativa == null ? "" : ticket.Justificativa.Trim(),
                            Consultor = "" // ticket.Consultor == null ? "" : ticket.Consultor.Trim()
                        };

                        string Filename = value.ticketId + ".json";
                        //System.IO.FileInfo f = new FileInfo(@"C:\Sistemas\Bolaco\Bolaco\Bolaco\Data\Enviados\" + Filename);
                        System.IO.FileInfo f = new FileInfo(@"d:\home\site\wwwroot\Data\Enviados\" + Filename);
                        
                        StreamWriter ws = f.CreateText();
                        ws.WriteLine(JsonConvert.SerializeObject(value));
                        ws.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return message;
        }

        [HttpPost]
        public string SetBrasilResult(TicketViewModel value)
        {
            string Result = "Sucesso !!!";

            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    int flag = -1;
                    #region grava o resultado da partida
                    if (Funcoes.Brasilia().Date == new DateTime(2018,6,22))
                    {
                        // Brasil 2 jogo
                        Parametro entity1 = db.Parametros.Find(12);
                        entity1.valor = value.score2Brasil.ToString();
                        db.Entry(entity1).State = EntityState.Modified;

                        // Costa Rica 2 jogo
                        Parametro entity2 = db.Parametros.Find(13);
                        entity2.valor = value.score2Mexico.ToString();
                        db.Entry(entity2).State = EntityState.Modified;
                        flag = -3;
                    }

                    if (Funcoes.Brasilia().Date == new DateTime(2018, 6, 27))
                    {
                        // Brasil 3 jogo
                        Parametro entity3 = db.Parametros.Find(14);
                        entity3.valor = value.score3Brasil.ToString();
                        db.Entry(entity3).State = EntityState.Modified;

                        // Suíça 3 jogo
                        Parametro entity4 = db.Parametros.Find(15);
                        entity4.valor = value.score3Camaroes.ToString();
                        db.Entry(entity4).State = EntityState.Modified;
                        flag = -4;
                    }

                    db.SaveChanges();
                    #endregion

                    #region Envia o SMS para os ganhadores
                    string _CHAVE_SMS = "0876f1a3-44db-48e8-8393-256c1cbd312a";
                    ListViewGanhadores ganhadores = new ListViewGanhadores();
                    IEnumerable<TicketViewModel> winners = new List<TicketViewModel>();

                    foreach (DWM.Models.Repositories.TicketViewModel t in winners.Where(info => info.score1Brasil == flag))
                    {
                        string ret = "";
                        if (t.clienteViewModel.telefone != null && t.clienteViewModel.telefone.Trim().Length > 0)
                        {
                            if (flag==-3)
                                ret = Torpedo.EnviarSMS(_CHAVE_SMS, "Norte Refrigeracao", t.clienteViewModel.telefone, "[Bolaaaco 2018] Parabens, seu palpite do jogo Brasil " + value.score2Brasil.ToString() + " x " + value.score2Mexico.ToString() + " Costa Rica com o Numero da Sorte [" + t.ticketId + "] foi o vencedor!");
                            else if (flag==-4)
                                ret = Torpedo.EnviarSMS(_CHAVE_SMS, "Norte Refrigeracao", t.clienteViewModel.telefone, "[Bolaaaco 2018] Parabens, seu palpite do jogo Brasil " + value.score3Brasil.ToString() + " x " + value.score3Camaroes.ToString() + " Servia com o Numero da Sorte [" + t.ticketId + "] foi o vencedor!");

                            if (ret.Trim().Length > 0)
                            {
                                throw new App_DominioException(new Validate()
                                {
                                    Code = 60,
                                    Message = MensagemPadrao.Message(60, ret).ToString(),
                                    MessageBase = ret,
                                    MessageType = MsgType.WARNING
                                });
                            }
                        }
                    }

                    #endregion

                    #region Envia o e-mail para os ganhadores
                    #endregion
                }
            }
            catch (App_DominioException ex)
            {
                return ex.Result.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


            return Result;
        }

        [HttpGet]
        public IEnumerable<PalpiteViewModel> Ganhadores()
        {
            ListViewGanhadores ganhadores = new ListViewGanhadores();
            IEnumerable<TicketViewModel> winner = new List<TicketViewModel>();
            IList<PalpiteViewModel> palpites = new List<PalpiteViewModel>();
            using (ApplicationContext db = new ApplicationContext())
                winner = ganhadores.Bind(db, 0, 5000, null);

            foreach (TicketViewModel value in winner)
            {
                PalpiteViewModel palpite = new PalpiteViewModel()
                {
                    ticketId = value.ticketId,
                    dt_compra = value.dt_inscricao.ToString("dd/MM/yyyy HH:mm"),
                    nome = value.clienteViewModel.nome,
                    cpf = value.clienteViewModel.cpf,
                    Consultor = value.Consultor,                    
                };

                palpites.Add(palpite);
            }

            return palpites;
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