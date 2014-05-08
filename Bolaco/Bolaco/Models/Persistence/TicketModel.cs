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
            value.dt_inscricao = DateTime.Now.AddHours(_fuso);
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
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Essa é uma mensagem de confirmação de seu palpite. Seu palpite na promoção <b>Bolaaaaço da Norte Refrigeração</b> foi realizado com sucesso.</span></p>" +
                              "<p></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: large; color: #000\">Número da Sorte: <b>" + value.ticketId + "</b></span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: large; color: #000\">Data e hora do palpite: <b>" + value.dt_inscricao.ToString("dd/MM/yyyy HH:mm") + " h.</b></span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Data da Compra: <b>" + value.dt_compra.ToString("dd/MM/yyyy") + "</b></span></p>" +
                              "<hr />" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Seu palpite:" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Brasil <b>" + value.score1Brasil.ToString() + "</b> X <b>" + value.score1Croacia.ToString() + "</b> Croácia</span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Brasil <b>" + value.score2Brasil.ToString() + "</b> X <b>" + value.score2Mexico.ToString() + "</b> México</span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Brasil <b>" + value.score3Brasil.ToString() + "</b> X <b>" + value.score3Camaroes.ToString() + "</b> Camarões</span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Final da Copa Fifa 2014:" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: large; color: #000\"><b>" + value.nome_selecao1Final + "  " + value.score1_final.ToString() + "</b> X <b>" + value.score2_final.ToString() + " " + value.nome_selecao2Final + "</b></span></p>";

                Html += "<p></p>" +
                        "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: xx-large; color: #3e5b33\"><b>BOA SORTE !</b></span></p>" +
                        "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: x-small; color: #333333\">Este é um e-mail automático. Por favor não responda, pois ele não será lido.</span></p>" +
                        "<p>&nbsp;</p>" +
                        "<p>&nbsp;</p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Cordialmente,</span></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Administração " + empresa.nome + "</span></p>";

                Validate result = sendMail.Send(sender, recipients, Html, Subject, Text, norte);
                if (result.Code > 0)
                    throw new App_DominioException(result);
            }
            #endregion

            return new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };
        }

        public override Ticket MapToEntity(TicketViewModel value)
        {
            return new Ticket()
            {
                ticketId = value.ticketId,
                dt_compra = value.dt_compra,
                clienteId = value.clienteViewModel.clienteId,
                dt_inscricao = value.dt_inscricao,
                score1Brasil = value.score1Brasil,
                score1Croacia = value.score1Croacia,
                score2Brasil = value.score2Brasil,
                score2Mexico = value.score2Mexico,
                score3Brasil = value.score3Brasil,
                score3Camaroes = value.score3Camaroes,
                selecao1Id_Final = value.selecao1Id_Final,
                selecao2Id_Final = value.selecao2Id_Final,
                score1_final = value.score1_final,
                score2_final = value.score2_final
            };
        }

        public override TicketViewModel MapToRepository(Ticket entity)
        {
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
                                        where sel.nome.ToUpper() == "CROÁCIA"
                                        select new SelecaoViewModel()
                                        {
                                            selecaoId = sel.selecaoId,
                                            nome = sel.nome,
                                            bandeira = sel.bandeira
                                        }).FirstOrDefault();

            SelecaoViewModel mexico = (from sel in db.Selecaos
                                       where sel.nome.ToUpper() == "MÉXICO"
                                       select new SelecaoViewModel()
                                       {
                                           selecaoId = sel.selecaoId,
                                           nome = sel.nome,
                                           bandeira = sel.bandeira
                                       }).FirstOrDefault();

            SelecaoViewModel camaroes = (from sel in db.Selecaos
                                         where sel.nome.ToUpper() == "CAMARÕES"
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
                value.mensagem.MessageBase = "Número da Sorte já existe em nossa base de dados.";
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
                                            where sel.nome.ToUpper() == "CROÁCIA"
                                            select new SelecaoViewModel()
                                            {
                                                selecaoId = sel.selecaoId,
                                                nome = sel.nome,
                                                bandeira = sel.bandeira
                                            }).FirstOrDefault();

                SelecaoViewModel mexico = (from sel in db.Selecaos
                                           where sel.nome.ToUpper() == "MÉXICO"
                                           select new SelecaoViewModel()
                                           {
                                               selecaoId = sel.selecaoId,
                                               nome = sel.nome,
                                               bandeira = sel.bandeira
                                           }).FirstOrDefault();

                SelecaoViewModel camaroes = (from sel in db.Selecaos
                                             where sel.nome.ToUpper() == "CAMARÕES"
                                             select new SelecaoViewModel()
                                             {
                                                 selecaoId = sel.selecaoId,
                                                 nome = sel.nome,
                                                 bandeira = sel.bandeira
                                             }).FirstOrDefault();

                return new TicketViewModel()
                {
                    dt_compra = DateTime.Today,
                    clienteViewModel = clienteViewModel,
                    bandeira_brasil = brasil.bandeira,
                    bandeira_croacia = croacia.bandeira,
                    bandeira_mexico = mexico.bandeira,
                    bandeira_camaroes = camaroes.bandeira,
                    Palpites = (from pal in db.Tickets
                                where pal.clienteId == _clienteId
                                orderby pal.dt_inscricao descending
                                select new TicketViewModel()
                                {
                                    ticketId = pal.ticketId,
                                    dt_inscricao = pal.dt_inscricao,
                                    dt_compra = pal.dt_compra,
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
                                                     where t.score1Brasil.HasValue
                                                     group t by new { t.score1Brasil, t.score1Croacia } into T
                                                     select new EstatisticaViewModel()
                                                     {
                                                         jogo = 1,
                                                         bandeira_selecao1 = _bandeira_brasil,
                                                         nome_selecao1 = "Brasil",
                                                         score_selecao1 = T.Key.score1Brasil.Value,
                                                         bandeira_selecao2 = _bandeira_croacia,
                                                         nome_selecao2 = "Croácia",
                                                         score_selecao2 = T.Key.score1Croacia.Value,
                                                         quantidade = T.Count(),
                                                         total = qte_tickets
                                                     };

            IEnumerable<EstatisticaViewModel> est2 = from t in db.Tickets
                                                     where t.score2Brasil.HasValue
                                                     group t by new { t.score2Brasil, t.score2Mexico } into T
                                                     select new EstatisticaViewModel()
                                                     {
                                                         jogo = 2,
                                                         bandeira_selecao1 = _bandeira_brasil,
                                                         nome_selecao1 = "Brasil",
                                                         score_selecao1 = T.Key.score2Brasil.Value,
                                                         bandeira_selecao2 = _bandeira_mexico,
                                                         nome_selecao2 = "México",
                                                         score_selecao2 = T.Key.score2Mexico.Value,
                                                         quantidade = T.Count(),
                                                         total = qte_tickets
                                                     };

            IEnumerable<EstatisticaViewModel> est3 = from t in db.Tickets
                                                     where t.score1Brasil.HasValue
                                                     group t by new { t.score3Brasil, t.score3Camaroes } into T
                                                     select new EstatisticaViewModel()
                                                     {
                                                         jogo = 3,
                                                         bandeira_selecao1 = _bandeira_brasil,
                                                         nome_selecao1 = "Brasil",
                                                         score_selecao1 = T.Key.score3Brasil.Value,
                                                         bandeira_selecao2 = _bandeira_camaroes,
                                                         nome_selecao2 = "Camarões",
                                                         score_selecao2 = T.Key.score3Camaroes.Value,
                                                         quantidade = T.Count(),
                                                         total = qte_tickets
                                                     };

            IEnumerable<EstatisticaViewModel> est4 = from t in db.Tickets
                                                     group t by new { t.selecao1Id_Final, t.selecao2Id_Final } into T
                                                     select new EstatisticaViewModel()
                                                     {
                                                         jogo = 4,
                                                         bandeira_selecao1 = (from s in db.Selecaos where s.selecaoId == T.Key.selecao1Id_Final select s.bandeira).FirstOrDefault(),
                                                         nome_selecao1 = (from s in db.Selecaos where s.selecaoId == T.Key.selecao1Id_Final select s.nome).FirstOrDefault(),
                                                         bandeira_selecao2 = (from s in db.Selecaos where s.selecaoId == T.Key.selecao2Id_Final select s.bandeira).FirstOrDefault(),
                                                         nome_selecao2 = (from s in db.Selecaos where s.selecaoId == T.Key.selecao2Id_Final select s.nome).FirstOrDefault(),
                                                         quantidade = T.Count(),
                                                         total = qte_tickets
                                                     };

            return (est1.OrderByDescending(info => info.quantidade).ToList().Union(est2.OrderByDescending(info => info.quantidade).ToList()).Union(est3.OrderByDescending(info => info.quantidade).ToList()).Union(est4.OrderByDescending(info => info.quantidade).ToList())).ToList();
        }


        public override Repository getRepository(Object id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}