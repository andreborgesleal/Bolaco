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

                string Subject = "Confirmação de cadastro na " + empresa.nome;
                string Text = "<p>Confirmação de cadastro</p>";
                string Html = "<p><span style=\"font-family: Verdana; font-size: x-large; font-weight: bold; color: #3e5b33\">" + sistema.descricao + "</span></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Essa é uma mensagem de confirmação de seu cadastro. Seu cadastro na promoção <b>Bolaaaaço da Norte Refrigeração</b> foi realizado com sucesso.</span></p>" +
                              "<table style=\"width: 95%; border: 0px solid #fff\">" +
                              "<tr>" +
                              "<td style=\"width: 55%\">" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Seus dados:" +
                              "<p><span style=\"font-family: Verdana; font-size: large; color: #3e5b33\">" + registerViewModel.nome.ToUpper() + "</span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">CPF: <b>" + Funcoes.FormataCPF(registerViewModel.cpf) + "</b></span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">E-mail: <b>" + registerViewModel.email + "</b></span></p>" +
                              "<p></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Telefone: <b>" + Funcoes.FormataTelefone(registerViewModel.telefone) + "</b></span></p>";

                if (registerViewModel.endereco != null && registerViewModel.endereco != "")
                {
                    Html += "<p></p>" +
                            "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Endereço: <b>" + registerViewModel.endereco.ToUpper() + "</b>&nbsp;</span><span style=\"font-family: Verdana; font-size: small; color: #000\"><b>" + registerViewModel.complemento.ToUpper() + "</b></span></p>" +
                            "<p></p>" +
                            "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Cidade: <b>" + registerViewModel.cidade.ToUpper() + "</b>-</span><span style=\"font-family: Verdana; font-size: small; color: #000\"><b>" + registerViewModel.uf.ToUpper() + "</b>&nbsp;&nbsp;</span><span style=\"font-family: Verdana; font-size: small; color: #000\">CEP: <b>" + Funcoes.FormataCep(registerViewModel.cep) + "</b></span></p>";
                }

                Html += "</td>" +
                        "<td style=\"width: 45%; vertical-align: top; float: right; padding-right: 27px\"><img src=\"http://bolaco.azurewebsites.net/Content/images/selocircular.png\"></td>" +
                        "</tr>" +
                        "</table>";

                Html += "<div style=\"width: 100%\"><p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Seu Login de acesso para dar palpites é: </span><span style=\"font-family: Verdana; font-size: large; margin-left: 20px; color: #3e5b33\"><b>" + registerViewModel.email + "</b></span></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Sua senha informada no cadastro é: </span><span style=\"font-family: Verdana; font-size: large; margin-left: 20px; color: #3e5b33\"><b>" + registerViewModel.senha + "</b></span></p>" +
                        "<hr />";

                Html += "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Agora que o seu cadastro foi realizado, já é possível dar os seus palpites no resultado dos jogos do Brasil da primeira fase da copa e de quais seleções disputarão a grande final.</span></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Acesse <a href=\"http://bolaco.azurewebsites.net/Account/Login\">Bolaaaaço da Norte Refrigeração</a> e dê os seus palpites.</span></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Informe o seu login e senha para realizar o acesso na área de apostas e cadastrar os seus palpite. Nesta área você poderá:</span></p>" +
                        "<ul>"+
                        "<li><span style=\"font-family: Verdana; font-size: small; color: #000\">Cadastrar os seus palpites para os jogos do Brasil na primeira fase.</span></li>" +
                        "<li><span style=\"font-family: Verdana; font-size: small; color: #000\">Cadastrar o seu palpite referente as seleções que farão a grande final da Copa 2014 e o placar do jogo.</span></li>" +
                        "<li><span style=\"font-family: Verdana; font-size: small; color: #000\">Consultar as estatíticas dos palpites dados por todos os participantes da promoção.</span></li>" +
                        "<li><span style=\"font-family: Verdana; font-size: small; color: #000\">Consultar todos os seus palpites. Quanto mais você comprar mais palpites poderá fazer.</span></li>" +
                        "</ul>"+
                        "<hr />" +
                        "<p><span style=\"font-family: Verdana; font-size: large; color: #3e5b33\"><b>BOA SORTE !</b></span></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Cordialmente,</span></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Administração " + empresa.nome + "</span></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: x-small; color: #333333\">Este é um e-mail automático. Por favor não responda, pois ele não será lido.</span></p>" +
                        "</div>";

                Validate result = sendMail.Send(sender, recipients, Html, Subject, Text, norte);
                if (result.Code > 0)
                {
                    result.MessageBase = "Seu cadastro foi realizado com sucesso mas não foi possível enviar seu e-mail de confirmação. Vá em \"Já sou Cadastrado\" para dar seus palpites e visualizar o seu cadastro.";
                    throw new App_DominioException(result);
                }
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