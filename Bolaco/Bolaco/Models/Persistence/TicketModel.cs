using System;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Security;
using App_Dominio.Repositories;
using App_Dominio.Models;
using App_Dominio.Component;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.SqlServer;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace DWM.Models.Persistence
{
    public class TicketModel : ProcessContext<Ticket, TicketViewModel, ApplicationContext>
    {
        #region Métodos da classe CrudContext
        public override Ticket ExecProcess(TicketViewModel value, Crud operation)
        {
            //EmpresaSecurity<SecurityContext> empresaSecurity = new EmpresaSecurity<SecurityContext>();

            int _empresaId = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EMPRESA).valor);
            int _fuso = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.FUSO).valor);

            value.clienteViewModel = new ClienteViewModel();
            value.empresaId = _empresaId;
            value.dt_inscricao = Funcoes.Brasilia();//  DateTime.Now.AddHours(_fuso);
            value.clienteViewModel.clienteId = int.Parse(sessaoCorrente.value1);

            string _url = value.uri;

            #region Mapear repository para entity
            Ticket entity = MapToEntity(value);
            #endregion

            #region grava o palpite
            entity = this.db.Set<Ticket>().Add(entity);
            #endregion

            ClienteViewModel clienteViewModel = (from cli in db.Clientes
                                                 where cli.clienteId == entity.clienteId
                                                 select new ClienteViewModel()
                                                 {
                                                     clienteId = cli.clienteId,
                                                     nome = cli.nome,
                                                     cpf = cli.cpf,
                                                     email = cli.email,
                                                     telefone = cli.telefone,
                                                     endereco = cli.endereco,
                                                     complemento = cli.complemento,
                                                     cidade = cli.cidade,
                                                     uf = cli.uf,
                                                     usuarioId = cli.usuarioId,
                                                     cep = cli.cep
                                                 }).FirstOrDefault();

            value.clienteViewModel = clienteViewModel;

            return entity;
        }

        public override Validate AfterInsert(TicketViewModel value)
        {
            #region Enviar e-mail ao cliente e à administração
            if (db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.HABILITA_EMAIL).valor == "S")
            {
                SendEmail sendMail = new SendEmail();

                int _sistemaId = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.SISTEMA).valor);
                string _email_admin = db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EMAIL_ADMIN).valor;

                Empresa empresa = seguranca_db.Empresas.Find(value.empresaId);
                Sistema sistema = seguranca_db.Sistemas.Find(_sistemaId);

                MailAddress sender = new MailAddress(empresa.nome + " <" + empresa.email + ">");
                List<string> recipients = new List<string>();
                List<string> norte = new List<string>();

                recipients.Add(value.clienteViewModel.nome + "<" + value.clienteViewModel.email + ">");
                norte.Add(empresa.nome + " <" + empresa.email + ">");
                if (_email_admin != "")
                    norte.Add(_email_admin);

                string Subject = "Confirmação de Palpite no " + sistema.descricao;
                string Text = "<p>Confirmação de palpite</p>";
                string Html = "<p><span style=\"font-family: Verdana; font-size: x-large; font-weight: bold; color: #3e5b33\">" + sistema.descricao + "</span></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: large; color: #3e5b33\">" + value.clienteViewModel.nome + "</span></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Essa é uma mensagem de confirmação de seu palpite. Seu palpite foi incluído com sucesso e será validado pela administração conforme regras da campanha <b><a href=\"http://bolaco2018.azurewebsites.net/Account/_Regras\">Bolaaaço Norte Refrigeração 2018</a></b>.</span></p>" +
                              "<p></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: large; color: #000\">Número da Sorte: <b>" + value.ticketId.ToUpper() + "</b></span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: large; color: #000\">Data e hora do palpite: <b>" + value.dt_inscricao.ToString("dd/MM/yyyy HH:mm") + " h.</b></span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Data da Compra: <b>" + value.dt_compra.ToString("dd/MM/yyyy") + "</b></span></p>" +
                              "<hr />" +
                              "<table style=\"width: 95%; border: 0px solid #fff\">" +
                              "<tr>" +
                              "<td style=\"width: 55%\">" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Seus palpites:" +
                              "<p><span style=\"font-family: Verdana; padding-left: 35px; font-size: small; color: #000\">Brasil <b>" + value.getScore1Brasil.ToString() + "</b> X <b>" + value.getScore1Croacia.ToString() + "</b> Suíça</span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; padding-left: 35px; font-size: small; color: #000\">Brasil <b>" + value.getScore2Brasil.ToString() + "</b> X <b>" + value.getScore2Mexico.ToString() + "</b> Costa Rica</span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; padding-left: 35px; font-size: small; color: #000\">Brasil <b>" + value.getScore3Brasil.ToString() + "</b> X <b>" + value.getScore3Camaroes.ToString() + "</b> Sérvia</span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; padding-left: 35px; font-size: small; color: #000\">Brasil <b>" + value.getScore4Brasil.ToString() + "</b> X <b>" + value.getScore4OutraSelecao.ToString() + "</b> Seleção (8º de final)</span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; padding-left: 35px; font-size: small; color: #000\">Brasil <b>" + value.getScore5Brasil.ToString() + "</b> X <b>" + value.getScore5OutraSelecao.ToString() + "</b> Seleção (4º de final)</span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; padding-left: 35px; font-size: small; color: #000\">Brasil <b>" + value.getScore6Brasil.ToString() + "</b> X <b>" + value.getScore6OutraSelecao.ToString() + "</b> Seleção (Semifinal)</span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Final da Copa:" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; padding-left: 35px; font-size: small; color: #000\"><b>" + value.nome_selecao1Final + "  " + value.score1_final.ToString() + "</b> X <b>" + value.score2_final.ToString() + " " + value.nome_selecao2Final + "</b></span></p>" +
                              "</td>" +
                              "<td style=\"width: 45%; vertical-align: top; float: right; padding-right: 27px\"><img src=\"http://bolaco2018.azurewebsites.net/Content/images/selocircular.png\"></td>" +
                              "</tr>" +
                              "</table>";

                Html += "<div style=\"width: 100%\"><p></p>" +
                        "<p></p>" +
                        "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: large; color: #3e5b33\"><b>BOA SORTE !</b></span></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Cordialmente,</span></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Administração " + empresa.nome + "</span></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: x-small; color: #333333\">Este é um e-mail automático. Por favor não responda, pois ele não será lido.</span></p>" +
                        "</div>";

                Validate result = sendMail.Send(sender, recipients, Html, Subject, Text, norte);
                if (result.Code > 0)
                {
                    result.MessageBase = "Seu palpite foi realizado com sucesso mas não foi possível enviar seu e-mail de confirmação. Vá em \"Todos os seus palpites\" para consultar sua aposta.";
                    throw new App_DominioException(result);
                }
            }
            #endregion

            #region Escreve Palpite na Fila de entrada (queue)
            try
            {
                PalpiteViewModel palpite = new PalpiteViewModel()
                {
                    ticketId = value.ticketId,
                    dt_compra = value.dt_compra.ToString("yyyy-MM-dd"),
                    cpf = value.clienteViewModel.cpf,
                    nome = value.clienteViewModel.nome,
                    Situacao = "1",
                    Justificativa = ""
                };
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=dwmsistemas;AccountKey=mYOiPtcUPSPwtCUipbcm+iSX1kD1Uap7u34VbJlhT4o5Q8eO9lLHfIUX8Y/DfvoLpGhoGClOLYBhXchpwyvoeg==;EndpointSuffix=core.windows.net"; // AmbientConnectionStringProvider.Instance.GetConnectionString(ConnectionStringNames.Storage);
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue = queueClient.GetQueueReference("bolaaaco2018-in");
                queue.CreateIfNotExists();
                queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(palpite)));
            }
            catch(Exception ex)
            {
                return new Validate() { Code = 55, Message = MensagemPadrao.Message(55).ToString(), MessageBase = ex.Message, MessageType = MsgType.ERROR };
            }
            #endregion

            return new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };
        }

        public override Ticket MapToEntity(TicketViewModel value)
        {

            Ticket t = new Ticket()
            {
                ticketId = value.ticketId.ToUpper(),
                dt_compra = value.dt_compra,
                clienteId = value.clienteViewModel.clienteId,
                dt_inscricao = value.dt_inscricao,
                score1Brasil = value.score1Brasil,
                score1Croacia = value.score1Croacia,
                score2Brasil = value.score2Brasil,
                score2Mexico = value.score2Mexico,
                score3Brasil = value.score3Brasil,
                score3Camaroes = value.score3Camaroes,
                score4Brasil = value.score4Brasil,
                score4OutraSelecao = value.score4OutraSelecao,
                score5Brasil = value.score5Brasil,
                score5OutraSelecao = value.score5OutraSelecao,
                score6Brasil = value.score6Brasil,
                score6OutraSelecao = value.score6OutraSelecao,
                selecao1Id_Final = value.selecao1Id_Final,
                selecao2Id_Final = value.selecao2Id_Final,
                score1_final = value.score1_final,
                score2_final = value.score2_final,
                Situacao = value.Situacao == null ? "1" : value.Situacao,
                Justificativa = value.Justificativa
            };

            if (DateTime.Now.AddHours(-3) > DateTime.Parse("2018-06-17 15:00"))
            {
                t.score1Brasil = -1;
                t.score1Croacia = -1;
            }
            if (DateTime.Now.AddHours(-3) > DateTime.Parse("2018-06-22 09:00"))
            {
                t.score2Brasil = -1;
                t.score2Mexico = -1;
            }
            if (DateTime.Now.AddHours(-3) > DateTime.Parse("2018-06-27 15:00"))
            {
                t.score3Brasil = -1;
                t.score3Camaroes = -1;
            }
            if (DateTime.Now.AddHours(-3) > DateTime.Parse("2018-07-02 11:00"))
            {
                t.score4Brasil = -1;
                t.score4OutraSelecao = -1;
            }
            if (DateTime.Now.AddHours(-3) > DateTime.Parse("2018-07-06 15:00"))
            {
                t.score5Brasil = -1;
                t.score5OutraSelecao = -1;
            }
            if (DateTime.Now.AddHours(-3) > DateTime.Parse("2018-07-10 15:00"))
            {
                t.score6Brasil = -1;
                t.score6OutraSelecao = -1;
            }

            return t;
        }

        public override TicketViewModel MapToRepository(Ticket entity)
        {
            int _empresaId = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EMPRESA).valor);

            ClienteViewModel clienteViewModel = (from cli in db.Clientes
                                                 where cli.clienteId == entity.clienteId 
                                                 select new ClienteViewModel() 
                                                 { 
                                                     clienteId = cli.clienteId,
                                                     nome = cli.nome,
                                                     cpf = cli.cpf,
                                                     email = cli.email,
                                                     telefone = cli.telefone,
                                                     endereco = cli.endereco,
                                                     complemento = cli.complemento,
                                                     cidade = cli.cidade, 
                                                     uf = cli.uf,
                                                     usuarioId = cli.usuarioId,
                                                     cep = cli.cep                                                 
                                                 }).FirstOrDefault();

            SelecaoViewModel selecao1_final = (from sel in db.Selecaos 
                                               where sel.selecaoId == entity.selecao1Id_Final
                                               select new SelecaoViewModel()
                                               {
                                                   selecaoId = sel.selecaoId,
                                                   nome = sel.nome,
                                                   bandeira = sel.bandeira
                                               }).FirstOrDefault();

            SelecaoViewModel selecao2_final = (from sel in db.Selecaos 
                                               where sel.selecaoId == entity.selecao2Id_Final
                                               select new SelecaoViewModel()
                                               {
                                                   selecaoId = sel.selecaoId,
                                                   nome = sel.nome,
                                                   bandeira = sel.bandeira
                                               }).FirstOrDefault();

            SelecaoViewModel brasil = (from sel in db.Selecaos
                                       where sel.nome.ToUpper() == "BRASIL"
                                       select new SelecaoViewModel()
                                       {
                                           selecaoId = sel.selecaoId,
                                           nome = sel.nome,
                                           bandeira = sel.bandeira
                                       }).FirstOrDefault();

            SelecaoViewModel croacia = (from sel in db.Selecaos
                                        where sel.nome.ToUpper() == "SUÍÇA"
                                        select new SelecaoViewModel()
                                        {
                                            selecaoId = sel.selecaoId,
                                            nome = sel.nome,
                                            bandeira = sel.bandeira
                                        }).FirstOrDefault();

            SelecaoViewModel mexico = (from sel in db.Selecaos
                                       where sel.nome.ToUpper() == "COSTA RICA"
                                       select new SelecaoViewModel()
                                       {
                                           selecaoId = sel.selecaoId,
                                           nome = sel.nome,
                                           bandeira = sel.bandeira
                                       }).FirstOrDefault();

            SelecaoViewModel camaroes = (from sel in db.Selecaos
                                         where sel.nome.ToUpper() == "SÉRVIA"
                                         select new SelecaoViewModel()
                                         {
                                             selecaoId = sel.selecaoId,
                                             nome = sel.nome,
                                             bandeira = sel.bandeira
                                         }).FirstOrDefault();

            return new TicketViewModel()
            {
                ticketId = entity.ticketId,
                dt_inscricao = entity.dt_inscricao,
                dt_compra = entity.dt_compra,
                clienteViewModel = clienteViewModel,
                empresaId = _empresaId,
                score1Brasil = entity.score1Brasil.Value,
                bandeira_brasil = brasil.bandeira,
                score1Croacia = entity.score1Croacia.Value,
                bandeira_croacia = croacia.bandeira,
                score2Brasil = entity.score2Brasil.Value,
                score2Mexico = entity.score2Mexico.Value,
                bandeira_mexico = mexico.bandeira,
                score3Brasil = entity.score3Brasil.Value,
                score3Camaroes = entity.score3Camaroes.Value,
                bandeira_camaroes = camaroes.bandeira,
                selecao1Id_Final = entity.selecao1Id_Final.Value,
                score4Brasil = entity.score4Brasil.Value,
                score4OutraSelecao = entity.score4OutraSelecao.Value,
                score5Brasil = entity.score5Brasil.Value,
                score5OutraSelecao = entity.score5OutraSelecao.Value,
                score6Brasil = entity.score6Brasil.Value,
                score6OutraSelecao = entity.score6OutraSelecao.Value,
                nome_selecao1Final = selecao1_final.nome,
                bandeira_finalista1 = selecao1_final.bandeira,
                score1_final = entity.score1_final.Value,
                selecao2Id_Final = entity.selecao2Id_Final.Value,
                nome_selecao2Final = selecao2_final.nome,
                bandeira_finalista2 = selecao2_final.bandeira,
                score2_final = entity.score2_final.Value,
                Palpites = (from pal in db.Tickets where pal.clienteId == entity.clienteId
                            orderby pal.dt_inscricao descending
                            select new TicketViewModel()
                            {
                                ticketId = pal.ticketId,
                                dt_inscricao = pal.dt_inscricao,
                                dt_compra = pal.dt_compra,
                                clienteViewModel = new ClienteViewModel() { clienteId = clienteViewModel.clienteId, nome = clienteViewModel.nome },
                                score1Brasil = pal.score1Brasil.Value,
                                bandeira_brasil = brasil.bandeira,
                                score1Croacia = pal.score1Croacia.Value,
                                bandeira_croacia = croacia.bandeira,
                                score2Brasil = pal.score2Brasil.Value,
                                score2Mexico = pal.score2Mexico.Value,
                                bandeira_mexico = mexico.bandeira,
                                score3Brasil = pal.score3Brasil.Value,
                                score3Camaroes = pal.score3Camaroes.Value,
                                bandeira_camaroes = camaroes.bandeira,
                                score4Brasil = pal.score4Brasil.Value,
                                score4OutraSelecao = pal.score4OutraSelecao.Value,
                                score5Brasil = pal.score5Brasil.Value,
                                score5OutraSelecao = pal.score5OutraSelecao.Value,
                                score6Brasil = pal.score6Brasil.Value,
                                score6OutraSelecao = pal.score6OutraSelecao.Value,
                                selecao1Id_Final = pal.selecao1Id_Final.Value,
                                nome_selecao1Final = (from s1 in db.Selecaos where s1.selecaoId == pal.selecao1Id_Final select s1.nome).FirstOrDefault(),
                                bandeira_finalista1 = (from s1 in db.Selecaos where s1.selecaoId == pal.selecao1Id_Final select s1.bandeira).FirstOrDefault(),
                                score1_final = pal.score1_final.Value,
                                selecao2Id_Final = pal.selecao2Id_Final.Value,
                                nome_selecao2Final = (from s1 in db.Selecaos where s1.selecaoId == pal.selecao2Id_Final select s1.nome).FirstOrDefault(),
                                bandeira_finalista2 = (from s1 in db.Selecaos where s1.selecaoId == pal.selecao2Id_Final select s1.bandeira).FirstOrDefault(),
                                score2_final = pal.score2_final.Value
                            }).ToList(),
                Parametros = (from par in db.Parametros select new ParametroViewModel() { paramId = par.paramId, valor = par.valor }).ToList(),
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override Ticket Find(TicketViewModel key)
        {
            return db.Tickets.Find(key.ticketId);
        }

        public override Validate Validate(TicketViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };

            if (value.ticketId.Trim().Length != 6)
            {
                value.mensagem.Code = 4;
                value.mensagem.Message = MensagemPadrao.Message(4, "Número da Sorte", "O Número da sorte deve possuir 6 dígitos (número e letras)").ToString();
                value.mensagem.MessageBase = "Número da Sorte inválido.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (!"MT|CT|BR".Contains(value.ticketId.Trim().ToUpper().Substring(0,2)))
            {
                value.mensagem.Code = 4;
                value.mensagem.Message = MensagemPadrao.Message(4, "Número da Sorte", "O Número da sorte deve começar com MT ou CT ou BR").ToString();
                value.mensagem.MessageBase = "Número da Sorte inválido.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            
            if (db.Tickets.Find(value.ticketId) != null)
            {
                value.mensagem.Code = 19;
                value.mensagem.Message = MensagemPadrao.Message(19).ToString();
                value.mensagem.MessageBase = "Número da Sorte já existe em nossa base de dados.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            DateTime dt_inicio_promocao = DateTime.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.DT_INICIO_PROMOCAO).valor);
            DateTime dt_fim_promocao = DateTime.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.DT_FIM_PROMOCAO).valor);

            if (value.dt_compra > DateTime.Today)
            {
                value.mensagem.Code = 10;
                value.mensagem.Message = MensagemPadrao.Message(10, "Dt.Compra").ToString();
                value.mensagem.MessageBase = "Data da compra deve ser menor ou igual a data de hoje: " + DateTime.Today.ToString("dd/MM/yyyy");
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.dt_compra < dt_inicio_promocao || value.dt_compra > dt_fim_promocao)
            {
                value.mensagem.Code = 4;
                value.mensagem.Message = MensagemPadrao.Message(4, "Dt.Compra").ToString();
                value.mensagem.MessageBase = "Data da Compra deve estar no período da promoção: " + dt_inicio_promocao.ToString("dd/MM/yyyy") + " à " + dt_fim_promocao.ToString("dd/MM/yyyy");
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.selecao1Id_Final == value.selecao2Id_Final)
            {
                value.mensagem.Code = 49;
                value.mensagem.Message = MensagemPadrao.Message(49, "Seleção Finalista 1", "Seleção Finalista 2").ToString();
                value.mensagem.MessageBase = "As seleções da final não podem ser iguais. Selecione duas seleções diferentes uma da outra";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            return value.mensagem;
        }

        public override TicketViewModel CreateRepository(System.Web.HttpRequestBase Request = null)
        {
            EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();
            sessaoCorrente = security.getSessaoCorrente();
            int _clienteId = int.Parse(sessaoCorrente.value1);
            
            using (db = new ApplicationContext())
            {
                ClienteViewModel clienteViewModel = (from cli in db.Clientes
                                                     where cli.clienteId == _clienteId
                                                     select new ClienteViewModel()
                                                     {
                                                         clienteId = cli.clienteId,
                                                         nome = cli.nome,
                                                         cpf = cli.cpf,
                                                         email = cli.email,
                                                         telefone = cli.telefone,
                                                         endereco = cli.endereco,
                                                         complemento = cli.complemento,
                                                         cidade = cli.cidade,
                                                         uf = cli.uf,
                                                         usuarioId = cli.usuarioId,
                                                         cep = cli.cep
                                                     }).FirstOrDefault();

               
                SelecaoViewModel brasil = (from sel in db.Selecaos
                                           where sel.nome.ToUpper() == "BRASIL"
                                           select new SelecaoViewModel()
                                           {
                                               selecaoId = sel.selecaoId,
                                               nome = sel.nome,
                                               bandeira = sel.bandeira
                                           }).FirstOrDefault();

                SelecaoViewModel croacia = (from sel in db.Selecaos
                                            where sel.nome.ToUpper() == "SUÍÇA"
                                            select new SelecaoViewModel()
                                            {
                                                selecaoId = sel.selecaoId,
                                                nome = sel.nome,
                                                bandeira = sel.bandeira
                                            }).FirstOrDefault();

                SelecaoViewModel mexico = (from sel in db.Selecaos
                                           where sel.nome.ToUpper() == "COSTA RICA"
                                           select new SelecaoViewModel()
                                           {
                                               selecaoId = sel.selecaoId,
                                               nome = sel.nome,
                                               bandeira = sel.bandeira
                                           }).FirstOrDefault();

                SelecaoViewModel camaroes = (from sel in db.Selecaos
                                             where sel.nome.ToUpper() == "SÉRVIA"
                                             select new SelecaoViewModel()
                                             {
                                                 selecaoId = sel.selecaoId,
                                                 nome = sel.nome,
                                                 bandeira = sel.bandeira
                                             }).FirstOrDefault();

                DateTime _dt_compra = new DateTime();
                string _ticketId = "";
                int _score1Brasil = 0;
                int _score2Brasil = 0;
                int _score3Brasil = 0;
                int _score1Croacia = 0;
                int _score2Mexico = 0;
                int _score3Camaroes = 0;
                int _score4Brasil = 0;
                int _score4OutraSelecao = 0;
                int _score5Brasil = 0;
                int _score5OutraSelecao = 0;
                int _score6Brasil = 0;
                int _score6OutraSelecao = 0;
                string _nome_selecao1Final = "";
                string _nome_selecao2Final = "";
                int _score1_final = 0;
                int _score2_final = 0;

                if (Request != null)
                {
                    if (Request ["dt_compra"] != null && Request ["dt_compra"].ToString() != "")
                        try
                        {
                            _dt_compra = DateTime.Parse(Request["dt_compra"].Substring(0, 4) + "-" +
                                                        Request["dt_compra"].Substring(5, 2) + "-" +
                                                        Request["dt_compra"].Substring(8, 2));

                        }
                        catch
                        {
                            _dt_compra = new DateTime();
                        }

                    if (Request["ticketId"] != null && Request["ticketId"] != "")
                        _ticketId = Request["ticketId"];

                    if (Request["score1Brasil"] != null && Request["score1Brasil"] != "")
                        _score1Brasil = int.Parse(Request["score1Brasil"]);

                    if (Request["score2Brasil"] != null && Request["score2Brasil"] != "")
                        _score2Brasil = int.Parse(Request["score2Brasil"]);

                    if (Request["score3Brasil"] != null && Request["score3Brasil"] != "")
                        _score3Brasil = int.Parse(Request["score3Brasil"]);

                    if (Request["score4Brasil"] != null && Request["score4Brasil"] != "")
                        _score4Brasil = int.Parse(Request["score4Brasil"]);

                    if (Request["score4OutraSelecao"] != null && Request["score4OutraSelecao"] != "")
                        _score4OutraSelecao = int.Parse(Request["score4OutraSelecao"]);

                    if (Request["score5Brasil"] != null && Request["score5Brasil"] != "")
                        _score5Brasil = int.Parse(Request["score5Brasil"]);

                    if (Request["score5OutraSelecao"] != null && Request["score5OutraSelecao"] != "")
                        _score5OutraSelecao = int.Parse(Request["score5OutraSelecao"]);

                    if (Request["score6Brasil"] != null && Request["score6Brasil"] != "")
                        _score6Brasil = int.Parse(Request["score6Brasil"]);

                    if (Request["score6OutraSelecao"] != null && Request["score6OutraSelecao"] != "")
                        _score6OutraSelecao = int.Parse(Request["score6OutraSelecao"]);

                    if (Request["score1_final"] != null && Request["score1_final"] != "")
                        _score1_final = int.Parse(Request["score1_final"]);

                    if (Request["score2_final"] != null && Request["score2_final"] != "")
                        _score2_final = int.Parse(Request["score2_final"]);
                }

                return new TicketViewModel()
                {
                    dt_compra = _dt_compra ,
                    ticketId = _ticketId,
                    score1Brasil = _score1Brasil,
                    score2Brasil = _score2Brasil,
                    score3Brasil = _score3Brasil,
                    score1Croacia = _score1Croacia,
                    score2Mexico = _score2Mexico,
                    score3Camaroes = _score3Camaroes,
                    score4Brasil = _score4Brasil,
                    score4OutraSelecao = _score4OutraSelecao,
                    score5Brasil = _score5Brasil,
                    score5OutraSelecao = _score5OutraSelecao,
                    score6Brasil = _score6Brasil,
                    score6OutraSelecao = _score6OutraSelecao,
                    nome_selecao1Final = _nome_selecao1Final,
                    nome_selecao2Final = _nome_selecao2Final,
                    score1_final = _score1_final,
                    score2_final = _score2_final,
                    clienteViewModel = clienteViewModel,
                    bandeira_brasil = brasil.bandeira,
                    bandeira_croacia = croacia.bandeira,
                    bandeira_mexico = mexico.bandeira,
                    bandeira_camaroes = camaroes.bandeira,
                    Situacao = "1",
                    Palpites = (from pal in db.Tickets
                                where pal.clienteId == _clienteId
                                orderby pal.dt_inscricao descending
                                select new TicketViewModel()
                                {
                                    ticketId = pal.ticketId,
                                    dt_inscricao = pal.dt_inscricao,
                                    dt_compra = pal.dt_compra,
                                    Situacao = pal.Situacao,
                                    Justificativa = pal.Justificativa,
                                    clienteViewModel = new ClienteViewModel() { clienteId = _clienteId, nome = clienteViewModel.nome },
                                    score1Brasil = pal.score1Brasil.Value,
                                    bandeira_brasil = brasil.bandeira,
                                    score1Croacia = pal.score1Croacia.Value,
                                    bandeira_croacia = croacia.bandeira,
                                    score2Brasil = pal.score2Brasil.Value,
                                    score2Mexico = pal.score2Mexico.Value,
                                    bandeira_mexico = mexico.bandeira,
                                    score3Brasil = pal.score3Brasil.Value,
                                    score3Camaroes = pal.score3Camaroes.Value,
                                    bandeira_camaroes = camaroes.bandeira,
                                    score4Brasil = pal.score4Brasil.Value,
                                    score4OutraSelecao = pal.score4OutraSelecao.Value,
                                    score5Brasil = pal.score5Brasil.Value,
                                    score5OutraSelecao = pal.score5OutraSelecao.Value,
                                    score6Brasil = pal.score6Brasil.Value,
                                    score6OutraSelecao = pal.score6OutraSelecao.Value,
                                    selecao1Id_Final = pal.selecao1Id_Final.Value,
                                    nome_selecao1Final = (from s1 in db.Selecaos where s1.selecaoId == pal.selecao1Id_Final select s1.nome).FirstOrDefault(),
                                    bandeira_finalista1 = (from s1 in db.Selecaos where s1.selecaoId == pal.selecao1Id_Final select s1.bandeira).FirstOrDefault(),
                                    score1_final = pal.score1_final.Value,
                                    selecao2Id_Final = pal.selecao2Id_Final.Value,
                                    nome_selecao2Final = (from s1 in db.Selecaos where s1.selecaoId == pal.selecao2Id_Final select s1.nome).FirstOrDefault(),
                                    bandeira_finalista2 = (from s1 in db.Selecaos where s1.selecaoId == pal.selecao2Id_Final select s1.bandeira).FirstOrDefault(),
                                    score2_final = pal.score2_final.Value
                                }).ToList(),
                    Parametros = (from par in db.Parametros select new ParametroViewModel() { paramId = par.paramId, valor = par.valor }).ToList(),
                    mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
                };
            }
        }

        #endregion
    }

    public class ListViewEstatistica : ListViewRepository<EstatisticaViewModel, ApplicationContext>
    {
        #region Métodos da classe ListViewRepository
        public override IEnumerable<EstatisticaViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            int qte_tickets = db.Tickets.Where(info => info.score1Brasil.HasValue).Count();
            
            string _bandeira_brasil = db.Selecaos.Find(1).bandeira;
            string _bandeira_croacia = db.Selecaos.Find(2).bandeira;
            string _bandeira_mexico = db.Selecaos.Find(3).bandeira;
            string _bandeira_camaroes = db.Selecaos.Find(4).bandeira;

            IEnumerable<EstatisticaViewModel> est1 = from t in db.Tickets
                                                     where t.score1Brasil >= 0 && "1|2".Contains(t.Situacao)
                                                     group t by new { t.score1Brasil, t.score1Croacia } into T
                                                     select new EstatisticaViewModel()
                                                     {
                                                         jogo = 1,
                                                         bandeira_selecao1 = _bandeira_brasil,
                                                         nome_selecao1 = "Brasil",
                                                         score_selecao1 = T.Key.score1Brasil.Value,
                                                         bandeira_selecao2 = _bandeira_croacia,
                                                         nome_selecao2 = "Suíça",
                                                         score_selecao2 = T.Key.score1Croacia.Value,
                                                         quantidade = T.Count(),
                                                         total = qte_tickets
                                                     };

            IEnumerable<EstatisticaViewModel> est2 = from t in db.Tickets
                                                     where t.score2Brasil >= 0 && "1|2".Contains(t.Situacao)
                                                     group t by new { t.score2Brasil, t.score2Mexico } into T
                                                     select new EstatisticaViewModel()
                                                     {
                                                         jogo = 2,
                                                         bandeira_selecao1 = _bandeira_brasil,
                                                         nome_selecao1 = "Brasil",
                                                         score_selecao1 = T.Key.score2Brasil.Value,
                                                         bandeira_selecao2 = _bandeira_mexico,
                                                         nome_selecao2 = "Costa Rica",
                                                         score_selecao2 = T.Key.score2Mexico.Value,
                                                         quantidade = T.Count(),
                                                         total = qte_tickets
                                                     };

            IEnumerable<EstatisticaViewModel> est3 = from t in db.Tickets
                                                     where t.score3Brasil >= 0 && "1|2".Contains(t.Situacao)
                                                     group t by new { t.score3Brasil, t.score3Camaroes } into T
                                                     select new EstatisticaViewModel()
                                                     {
                                                         jogo = 3,
                                                         bandeira_selecao1 = _bandeira_brasil,
                                                         nome_selecao1 = "Brasil",
                                                         score_selecao1 = T.Key.score3Brasil.Value,
                                                         bandeira_selecao2 = _bandeira_camaroes,
                                                         nome_selecao2 = "Sérvia",
                                                         score_selecao2 = T.Key.score3Camaroes.Value,
                                                         quantidade = T.Count(),
                                                         total = qte_tickets
                                                     };

            IEnumerable<EstatisticaViewModel> est4 = from t in db.Tickets
                                                     where t.score4Brasil >= 0 && "1|2".Contains(t.Situacao)
                                                     group t by new { t.score4Brasil, t.score4OutraSelecao } into T
                                                     select new EstatisticaViewModel()
                                                     {
                                                         jogo = 4,
                                                         bandeira_selecao1 = _bandeira_brasil,
                                                         nome_selecao1 = "Brasil",
                                                         score_selecao1 = T.Key.score4Brasil.Value,
                                                         bandeira_selecao2 = "",
                                                         nome_selecao2 = "Seleção 8ª final",
                                                         score_selecao2 = T.Key.score4OutraSelecao.Value,
                                                         quantidade = T.Count(),
                                                         total = qte_tickets
                                                     };

            IEnumerable<EstatisticaViewModel> est5 = from t in db.Tickets
                                                     where t.score5Brasil >= 0 && "1|2".Contains(t.Situacao)
                                                     group t by new { t.score5Brasil, t.score5OutraSelecao } into T
                                                     select new EstatisticaViewModel()
                                                     {
                                                         jogo = 5,
                                                         bandeira_selecao1 = _bandeira_brasil,
                                                         nome_selecao1 = "Brasil",
                                                         score_selecao1 = T.Key.score5Brasil.Value,
                                                         bandeira_selecao2 = "",
                                                         nome_selecao2 = "Seleção 4ª final",
                                                         score_selecao2 = T.Key.score5OutraSelecao.Value,
                                                         quantidade = T.Count(),
                                                         total = qte_tickets
                                                     };

            IEnumerable<EstatisticaViewModel> est6 = from t in db.Tickets
                                                     where t.score6Brasil >= 0 && "1|2".Contains(t.Situacao)
                                                     group t by new { t.score6Brasil, t.score6OutraSelecao } into T
                                                     select new EstatisticaViewModel()
                                                     {
                                                         jogo = 6,
                                                         bandeira_selecao1 = _bandeira_brasil,
                                                         nome_selecao1 = "Brasil",
                                                         score_selecao1 = T.Key.score6Brasil.Value,
                                                         bandeira_selecao2 = "",
                                                         nome_selecao2 = "Seleção semifinal",
                                                         score_selecao2 = T.Key.score6OutraSelecao.Value,
                                                         quantidade = T.Count(),
                                                         total = qte_tickets
                                                     };

            IEnumerable<EstatisticaViewModel> est7 = from t in db.Tickets
                                                     where "1|2".Contains(t.Situacao)
                                                     group t by new { t.selecao1Id_Final, t.selecao2Id_Final } into T
                                                     select new EstatisticaViewModel()
                                                     {
                                                         jogo = 7,
                                                         bandeira_selecao1 = (from s in db.Selecaos where s.selecaoId == T.Key.selecao1Id_Final select s.bandeira).FirstOrDefault(),
                                                         nome_selecao1 = (from s in db.Selecaos where s.selecaoId == T.Key.selecao1Id_Final select s.nome).FirstOrDefault(),
                                                         bandeira_selecao2 = (from s in db.Selecaos where s.selecaoId == T.Key.selecao2Id_Final select s.bandeira).FirstOrDefault(),
                                                         nome_selecao2 = (from s in db.Selecaos where s.selecaoId == T.Key.selecao2Id_Final select s.nome).FirstOrDefault(),
                                                         quantidade = T.Count(),
                                                         total = qte_tickets
                                                     };

            #region Se houver na final da copa Brasil x Alemana e Alemanha x Brasil , o algoritmo abaixo fazer a troca e contabiliza somente um
            IList<EstatisticaViewModel> estTemp = new List<EstatisticaViewModel>();

            for (int i=0; i <= est7.Count()-1; i++)
            {
                EstatisticaViewModel tmp = est7.ElementAt(i);

                if ((from temp in estTemp
                     where temp.nome_selecao2 == tmp.nome_selecao1 
                            && temp.nome_selecao1 == tmp.nome_selecao2
                     select tmp).Count() == 0)
                {
                    EstatisticaViewModel e = new EstatisticaViewModel()
                    {
                        jogo = 7,
                        bandeira_selecao1 = tmp.bandeira_selecao1,
                        nome_selecao1 = tmp.nome_selecao1,
                        bandeira_selecao2 = tmp.bandeira_selecao2,
                        nome_selecao2 = tmp.nome_selecao2,
                        quantidade = tmp.quantidade,
                        total = tmp.total
                    };
                    if ((from e7 in est7
                         where e7.nome_selecao2 == tmp.nome_selecao1
                                && e7.nome_selecao1 == tmp.nome_selecao2
                         select e7).Count() > 0)
                        e.quantidade += (from e7 in est7
                                         where e7.nome_selecao2 == tmp.nome_selecao1
                                                && e7.nome_selecao1 == tmp.nome_selecao2
                                         select e7).FirstOrDefault().quantidade;

                    estTemp.Add(e);
                }
            }
            #endregion

            return (est1.OrderByDescending(info => info.quantidade).ToList().Union(est2.OrderByDescending(info => info.quantidade).ToList()).Union(est3.OrderByDescending(info => info.quantidade).ToList()).Union(est4.OrderByDescending(info => info.quantidade).ToList()).Union(est5.OrderByDescending(info => info.quantidade).ToList()).Union(est6.OrderByDescending(info => info.quantidade).ToList()).Union(estTemp.OrderByDescending(info => info.quantidade).ToList())).ToList();
        }


        public override Repository getRepository(Object id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class ListViewGanhadores : ListViewRepository<TicketViewModel, ApplicationContext>
    {
        #region Métodos da classe ListViewRepository
        public override IEnumerable<TicketViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            int _Score1Brasil = -2;
            int _Score1Croacia = -2;
            int _Score2Brasil = -2;
            int _Score2Mexico = -2;
            int _Score3Brasil = -2;
            int _Score3Camaroes = -2;
            int _Score4Brasil = -2;
            int _Score4OutraSelecao = -2;
            int _Score5Brasil = -2;
            int _Score5OutraSelecao = -2;
            int _Score6Brasil = -2;
            int _Score6OutraSelecao = -2;
            int _Selecao1_final = -2;
            int _Selecao2_final = -2;
            int _Score1_final = -2;
            int _Score2_final = -2;
            
            if (db.Parametros.Find(10).valor != "")
                _Score1Brasil = int.Parse(db.Parametros.Find(10).valor);

            if (db.Parametros.Find(11).valor != "")
                _Score1Croacia = int.Parse(db.Parametros.Find(11).valor);

            if (db.Parametros.Find(12).valor != "")
                _Score2Brasil = int.Parse(db.Parametros.Find(12).valor);

            if (db.Parametros.Find(13).valor != "")
                _Score2Mexico = int.Parse(db.Parametros.Find(13).valor);

            if (db.Parametros.Find(14).valor != "")
                _Score3Brasil = int.Parse(db.Parametros.Find(14).valor);

            if (db.Parametros.Find(15).valor != "")
                _Score3Camaroes = int.Parse(db.Parametros.Find(15).valor);

            if (db.Parametros.Find(20).valor != "")
                _Score4Brasil = int.Parse(db.Parametros.Find(20).valor);

            if (db.Parametros.Find(21).valor != "")
                _Score4OutraSelecao = int.Parse(db.Parametros.Find(21).valor);

            if (db.Parametros.Find(22).valor != "")
                _Score5Brasil = int.Parse(db.Parametros.Find(22).valor);

            if (db.Parametros.Find(23).valor != "")
                _Score5OutraSelecao = int.Parse(db.Parametros.Find(23).valor);

            if (db.Parametros.Find(24).valor != "")
                _Score6Brasil = int.Parse(db.Parametros.Find(24).valor);

            if (db.Parametros.Find(25).valor != "")
                _Score6OutraSelecao = int.Parse(db.Parametros.Find(25).valor);

            if (db.Parametros.Find(16).valor != "")
                _Score1_final = int.Parse(db.Parametros.Find(16).valor);

            if (db.Parametros.Find(17).valor != "")
                _Score2_final = int.Parse(db.Parametros.Find(17).valor);

            if (db.Parametros.Find(18).valor != "")
                _Selecao1_final = int.Parse(db.Parametros.Find(18).valor);

            if (db.Parametros.Find(19).valor != "")
                _Selecao2_final = int.Parse(db.Parametros.Find(19).valor);




            IEnumerable<TicketViewModel> result = (from t in db.Tickets
                                                   join c in db.Clientes on t.clienteId equals c.clienteId
                                                   where t.score1Brasil == _Score1Brasil && t.score1Croacia == _Score1Croacia
                                                   orderby c.nome
                                                   select new TicketViewModel()
                                                   {
                                                       clienteViewModel = new ClienteViewModel() { nome = c.nome },
                                                       ticketId = t.ticketId,
                                                       dt_inscricao = t.dt_inscricao,
                                                       score1Brasil = -2
                                                   }).ToList().Union(
                                                    (from t in db.Tickets
                                                     join c in db.Clientes on t.clienteId equals c.clienteId
                                                     where t.score2Brasil == _Score2Brasil && t.score2Mexico == _Score2Mexico
                                                     orderby c.nome
                                                     select new TicketViewModel()
                                                     {
                                                         clienteViewModel = new ClienteViewModel() { nome = c.nome },
                                                         ticketId = t.ticketId,
                                                         dt_inscricao = t.dt_inscricao,
                                                         score1Brasil = -3
                                                     }).ToList()
                                                    ).Union(
                                                   (from t in db.Tickets
                                                    join c in db.Clientes on t.clienteId equals c.clienteId
                                                    where t.score3Brasil == _Score3Brasil && t.score3Camaroes == _Score3Camaroes
                                                    orderby c.nome
                                                    select new TicketViewModel()
                                                    {
                                                        clienteViewModel = new ClienteViewModel() { nome = c.nome },
                                                        ticketId = t.ticketId,
                                                        dt_inscricao = t.dt_inscricao,
                                                        score1Brasil = -4
                                                    }).ToList()
                                                    ).Union(
                                                    (from t in db.Tickets
                                                     join c in db.Clientes on t.clienteId equals c.clienteId
                                                     where (t.score1Brasil == _Score1Brasil && t.score1Croacia == _Score1Croacia &&
                                                            t.score2Brasil == _Score2Brasil && t.score2Mexico == _Score2Mexico) ||
                                                           (t.score1Brasil == _Score1Brasil && t.score1Croacia == _Score1Croacia &&
                                                            t.score3Brasil == _Score3Brasil && t.score3Camaroes == _Score3Camaroes) ||
                                                           (t.score2Brasil == _Score2Brasil && t.score2Mexico == _Score2Mexico &&
                                                            t.score3Brasil == _Score3Brasil && t.score3Camaroes == _Score3Camaroes) 
                                                     orderby t.dt_inscricao
                                                     select new TicketViewModel()
                                                     {
                                                         clienteViewModel = new ClienteViewModel() { nome = c.nome },
                                                         ticketId = t.ticketId,
                                                         dt_inscricao = t.dt_inscricao,
                                                         score1Brasil = -5
                                                     }).ToList()
                                                    ).Union(
                                                    (from t in db.Tickets
                                                     join c in db.Clientes on t.clienteId equals c.clienteId
                                                     where t.score4Brasil == _Score4Brasil && t.score4OutraSelecao == _Score4OutraSelecao
                                                     orderby c.nome
                                                     select new TicketViewModel()
                                                     {
                                                         clienteViewModel = new ClienteViewModel() { nome = c.nome },
                                                         ticketId = t.ticketId,
                                                         dt_inscricao = t.dt_inscricao,
                                                         score1Brasil = -6
                                                     }).ToList()
                                                    ).Union(
                                                    (from t in db.Tickets
                                                     join c in db.Clientes on t.clienteId equals c.clienteId
                                                     where t.score5Brasil == _Score5Brasil && t.score5OutraSelecao == _Score5OutraSelecao
                                                     orderby c.nome
                                                     select new TicketViewModel()
                                                     {
                                                         clienteViewModel = new ClienteViewModel() { nome = c.nome },
                                                         ticketId = t.ticketId,
                                                         dt_inscricao = t.dt_inscricao,
                                                         score1Brasil = -7
                                                     }).ToList()
                                                    ).Union(
                                                    (from t in db.Tickets
                                                     join c in db.Clientes on t.clienteId equals c.clienteId
                                                     where t.score6Brasil == _Score6Brasil && t.score6OutraSelecao == _Score6OutraSelecao
                                                     orderby c.nome
                                                     select new TicketViewModel()
                                                     {
                                                         clienteViewModel = new ClienteViewModel() { nome = c.nome },
                                                         ticketId = t.ticketId,
                                                         dt_inscricao = t.dt_inscricao,
                                                         score1Brasil = -8
                                                     }).ToList()
                                                    ).Union(
                                                    (from t in db.Tickets
                                                     join c in db.Clientes on t.clienteId equals c.clienteId
                                                     where (t.score4Brasil == _Score4Brasil && t.score4OutraSelecao == _Score4OutraSelecao &&
                                                            t.score5Brasil == _Score5Brasil && t.score5OutraSelecao == _Score5OutraSelecao) ||
                                                           (t.score4Brasil == _Score4Brasil && t.score4OutraSelecao == _Score4OutraSelecao &&
                                                            t.score6Brasil == _Score6Brasil && t.score6OutraSelecao == _Score6OutraSelecao) ||
                                                           (t.score5Brasil == _Score5Brasil && t.score5OutraSelecao == _Score5OutraSelecao &&
                                                            t.score6Brasil == _Score6Brasil && t.score6OutraSelecao == _Score6OutraSelecao)
                                                     orderby t.dt_inscricao
                                                     select new TicketViewModel()
                                                     {
                                                         clienteViewModel = new ClienteViewModel() { nome = c.nome },
                                                         ticketId = t.ticketId,
                                                         dt_inscricao = t.dt_inscricao,
                                                         score1Brasil = -9
                                                     }).ToList()
                                                    ).Union(
                                                     (from t in db.Tickets
                                                      join c in db.Clientes on t.clienteId equals c.clienteId
                                                      where (t.selecao1Id_Final == _Selecao1_final  && t.selecao2Id_Final == _Selecao2_final) ||
                                                            (t.selecao1Id_Final == _Selecao2_final && t.selecao2Id_Final == _Selecao1_final)
                                                      orderby t.dt_inscricao
                                                      select new TicketViewModel()
                                                      {
                                                          clienteViewModel = new ClienteViewModel() { nome = c.nome },
                                                          ticketId = t.ticketId,
                                                          dt_inscricao = t.dt_inscricao,
                                                          score1Brasil = -10
                                                      }).ToList()
                                                    ).ToList();

            return result.ToList();
        }

        public override Repository getRepository(Object id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class ListViewResumoGerencial : ListViewRepository<ResumoGerencialViewModel, ApplicationContext>
    {
        #region Métodos da classe ListViewRepository
        public override IEnumerable<ResumoGerencialViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            IEnumerable<ResumoGerencial1ViewModel> r1 = (from t in db.Tickets
                                                         where "1|2".Contains(t.Situacao)
                                                         group t by t.ticketId.Substring(0, 2) into T
                                                         orderby T.Count()
                                                         select new ResumoGerencial1ViewModel()
                                                         {
                                                             loja = T.Key,
                                                             qte_palpites = T.Count()
                                                         }).ToList();

            IEnumerable<ResumoGerencial2ViewModel> r2 = (from t in
                                                            (from t1 in db.Tickets
                                                             group t1 by new { loja = t1.ticketId.Substring(0, 2), clienteId = t1.clienteId } into T1
                                                             select new
                                                             {
                                                                 loja = T1.Key.loja,
                                                                 clienteId = T1.Key.clienteId
                                                             })
                                                        group t by t.loja into T
                                                        orderby T.Count()
                                                        select new ResumoGerencial2ViewModel()
                                                        {
                                                            loja = T.Key,
                                                            qte_cadastros = T.Count()
                                                        }).ToList();

            IEnumerable<int> exp = db.Ticket_Expurgos.Select(info => info.clienteId).ToList();

            IEnumerable<ResumoGerencial3ViewModel> r3 = (from c in db.Clientes
                                                        join t in db.Tickets on c.clienteId equals t.clienteId into T
                                                        from t in T.DefaultIfEmpty()
                                                        where t.ticketId == null && !exp.Contains(c.clienteId)
                                                        select new ResumoGerencial3ViewModel()
                                                        {
                                                            nome = c.nome,
                                                            email = c.email,
                                                            dt_cadastro = c.dt_cadastro
                                                        }).ToList();

            IEnumerable<ResumoGerencial4ViewModel> r4 = (from t in db.Tickets
                                                         group t by new
                                                         {
                                                             ano = SqlFunctions.DatePart("year", t.dt_inscricao),
                                                             mes = SqlFunctions.DatePart("month", t.dt_inscricao),
                                                             dia = SqlFunctions.DatePart("day", t.dt_inscricao)
                                                         } into T
                                                         select new ResumoGerencial4ViewModel()
                                                         {
                                                             dia = T.Key.dia.Value,
                                                             mes = T.Key.mes.Value,
                                                             ano = T.Key.ano.Value,
                                                             qte_palpites = T.Count()
                                                         }).ToList();


            ResumoGerencial5ViewModel r5 = new ResumoGerencial5ViewModel()
            {
                total_dias = r4.Count(), // (from t in db.Tickets group t by t.dt_inscricao into T select T.Key).Count(),
                total_cadastros = db.Clientes.Count(),
                total_palpites = db.Tickets.Count(),
                total_palpites_aprovados = db.Tickets.Where(info => info.Situacao == "2").Count(),
                total_palpites_rejeitados = db.Tickets.Where(info => info.Situacao == "3").Count(),
                total_palpites_pendentes = db.Tickets.Where(info => info.Situacao == "1").Count(),
            };

            r5.media_diaria_palpite = r5.total_palpites / r5.total_dias;


            ResumoGerencialViewModel r = new ResumoGerencialViewModel()
            {
                resumo1 = r1.ToList(),
                resumo2 = r2.ToList(),
                resumo3 = r3.ToList(),
                resumo4 = r4.ToList(),
                resumo5 = r5
            };

            IList<ResumoGerencialViewModel> result = new List<ResumoGerencialViewModel>();

            result.Add(r);

            return result;
        }

        public IEnumerable<ResumoGerencialViewModel> Bind(ApplicationContext db, int? index, int pageSize = 50, params object[] param )
        {
            this.db = db;
            return Bind(index, pageSize, param);
        }


        public override Repository getRepository(Object id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class ListViewParametros : ListViewRepository<ParametroViewModel, ApplicationContext>
    {
        #region Métodos da classe ListViewRepository
        public override IEnumerable<ParametroViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {

            IEnumerable<ParametroViewModel> Parametros = (from p in db.Parametros
                                                          select new ParametroViewModel()
                                                          {
                                                              paramId = p.paramId,
                                                              valor = p.valor
                                                          }).ToList();


            return Parametros;
        }


        public override Repository getRepository(Object id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}