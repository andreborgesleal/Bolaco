using System;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Security;
using App_Dominio.Repositories;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using App_Dominio.Models;
using App_Dominio.Component;
using System.Net.Mail;
using System.Collections.Generic;
using App_Dominio.Negocio;

namespace DWM.Models.Persistence
{
    public class AccountModel : ProcessContext<Cliente, RegisterViewModel, ApplicationContext>
    {
        private RegisterViewModel registerViewModel { get; set; }

        #region Métodos da classe CrudContext
        public override Cliente ExecProcess(RegisterViewModel value, Crud operation)
        {
            EmpresaSecurity<SecurityContext> empresaSecurity = new EmpresaSecurity<SecurityContext>();

            int _empresaId = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EMPRESA).valor);
            int _sistemaId = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.SISTEMA).valor);

            value.empresaId = _empresaId;

            ClienteModel clienteModel = new ClienteModel(db);
            clienteModel.seguranca_db = this.seguranca_db;

            #region validar inclusão
            value.mensagem = clienteModel.Validate(value, Crud.INCLUIR);

            if (value.mensagem.Code > 0)
                throw new App_DominioException(value.mensagem);
            #endregion

            // verifica se já existe o usuário cadastrado
            UsuarioRepository u = empresaSecurity.getUsuarioByLogin(value.email, value.empresaId);

            if (u == null)
            {
                #region Incluir o usuário
                UsuarioRepository usuarioRepository = new UsuarioRepository()
                {
                    login = value.email.ToLower(),
                    nome = value.nome.Trim().Length <= 40 ? value.nome.ToUpper() : value.nome.Substring(0, 40).ToUpper(),
                    empresaId = _empresaId,
                    dt_cadastro = DateTime.Now,
                    situacao = "A",
                    isAdmin = "N",
                    senha = value.senha,
                    uri = value.uri,
                    confirmacaoSenha = value.confirmacaoSenha
                };

                usuarioRepository = empresaSecurity.SetUsuario(usuarioRepository);
                if (usuarioRepository.mensagem.Code > 0)
                    throw new App_DominioException(usuarioRepository.mensagem);

                u = usuarioRepository;
                #endregion

                #region Incluir o usuário no grupo de usuários
                int _grupoId = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.GRUPO_USUARIO).valor);
                UsuarioGrupo usuarioGrupo = new UsuarioGrupo()
                {
                    usuarioId = usuarioRepository.usuarioId,
                    grupoId = _grupoId,
                    situacao = "A"
                };

                // ******** é aqui o erro
                if (usuarioGrupo.grupoId > 0)
                    seguranca_db.Set<UsuarioGrupo>().Add(usuarioGrupo);

                //seguranca_db.SaveChanges();
                #endregion
            }

            #region incluir o cliente

            string _url = value.uri;

            #region Mapear repository para entity
            value.usuarioId = u.usuarioId;
            Cliente entity = clienteModel.MapToEntity(value);
            #endregion

            entity = this.db.Set<Cliente>().Add(entity);
            ClienteViewModel clienteViewModel = clienteModel.MapToRepository(entity); // precisa mapear para recuperar o clienteID que foi incluído
            #endregion

            registerViewModel = value;

            return entity;
        }

        public override Validate AfterInsert(RegisterViewModel value)
        {
            #region Enviar e-mail ao cliente e à administração
            if (db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.HABILITA_EMAIL).valor == "S")
            {
                SendEmail sendMail = new SendEmail();

                int _sistemaId = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.SISTEMA).valor);
                string _email_admin = db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EMAIL_ADMIN).valor;

                Empresa empresa = seguranca_db.Empresas.Find(registerViewModel.empresaId);
                Sistema sistema = seguranca_db.Sistemas.Find(_sistemaId);

                MailAddress sender = new MailAddress(empresa.nome + " <" + empresa.email + ">");
                List<string> recipients = new List<string>();
                List<string> norte = new List<string>();

                recipients.Add(registerViewModel.nome + "<" + registerViewModel.email + ">");
                norte.Add(empresa.nome + " <" + empresa.email + ">");
                if (_email_admin != "")
                    norte.Add(_email_admin);

                string Subject = "Confirmação de cadastro no " + empresa.nome;
                string Text = "<p>Confirmação de cadastro</p>";
                string Html = "<p><span style=\"font-family: Verdana; font-size: larger; color: #656464\">" + sistema.descricao + "</span></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: xx-large; color: #3e5b33\">" + registerViewModel.nome.ToUpper() + "</span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">CPF: <b>" + Funcoes.FormataCPF(registerViewModel.cpf) + "</b></span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">E-mail: <b>" + registerViewModel.email + "</b></span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Telefone: <b>" + Funcoes.FormataTelefone(registerViewModel.telefone) + "</b></span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Essa é uma mensagem de confirmação de seu cadastro. Seu cadastro na promoção <b>Bolaaaaço da Norte Refrigeração</b> foi realizado com sucesso.</span></p>";

                string asterisco = "";
                for (int i = 1; i <= registerViewModel.senha.Length - 1; i++)
                    asterisco += "*";

                Html += "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Seu Login de acesso para dar palpites é: </span></p>" +
                        "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: xx-large; color: #3e5b33\">" + registerViewModel.email + "</span></p>" +
                        "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Sua senha informada no cadastro é: </span></p>" +
                        "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: xx-large; color: #3e5b33\">" + registerViewModel.senha.Substring(0, 1) + asterisco + "</span></p>" +
                        "<hr />";


                Html += "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Agora que o seu cadastro foi realizado, já é possível dar o seu palpite no resultado dos jogos do Brasil da primeira fase da copa, dar o seu palpite de quais seleções disputarão a grande final e concorrer a prêmios.</span></p>" +
                        "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Acesse o site da Norte Refrigeração no endereço <a href=\"www.norterefrigeracao.com.br\">www.norterefrigeracao.com.br</a> e clique no banner da promoção. Na mesma página que você realizou este cadastro clique no link \"Dê o seu Palpite\". </span></p>" +
                        "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Informe o seu login e senha para realizar o acesso na área de apostas e cadastrar o seu palpite. Nesta área você poderá:</span></p>" +
                        "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">- Cadastrar o seu palpite para os jogos do Brasil na primeira fase.</span></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">- Cadastrar o seu palpite referente as seleções que farão a grande final da copa 2014 e o placar do jogo.</span></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">- Consultar as estatíticas dos palpites dados por todos os participantes da promoção.</span></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">- Consultar todos os seus palpites. Quanto mais você comprar mais palpites poderá fazer e maiores as suas chances de ganhar prêmios.</span></p>" +
                        "<hr />" +
                        "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: xx-large; color: #3e5b33\">BOA SORTE !</span></p>" +
                        "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: x-small; color: #333333\">Este é um e-mail automático. Por favor não responda, pois ele não será lido.</span></p>" +
                        "<p>&nbsp;</p>" +
                        "<p>&nbsp;</p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Obrigado,</span></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Administração " + empresa.nome + "</span></p>";

                Validate result = sendMail.Send(sender, recipients, Html, Subject, Text, norte);
                if (result.Code > 0)
                    throw new App_DominioException(result);
            }
            #endregion

            return new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };
        }

        public override Cliente MapToEntity(RegisterViewModel value)
        {
            return new Cliente()
            {
                clienteId = value.clienteId
            };
        }

        public override RegisterViewModel MapToRepository(Cliente entity)
        {

            return new RegisterViewModel()
            {
                clienteId = entity.clienteId,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override Cliente Find(RegisterViewModel key)
        {
            return db.Clientes.Find(key.clienteId);
        }

        public override Validate Validate(RegisterViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };
           
            return value.mensagem;
        }
        #endregion
    }
}