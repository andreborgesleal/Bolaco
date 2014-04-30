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
            return new Cliente();
            //EmpresaSecurity<SecurityContext> empresaSecurity = new EmpresaSecurity<SecurityContext>();

            //int _empresaId = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EMPRESA).valor);
            //int _sistemaId = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.SISTEMA).valor);

            //value.empresaId = _empresaId;

            //ClienteModel clienteModel = new ClienteModel(db);
            //clienteModel.seguranca_db = this.seguranca_db;

            //#region validar inclusão
            //value.mensagem = clienteModel.Validate(value, Crud.INCLUIR);

            //if (value.mensagem.Code > 0)
            //    throw new App_DominioException(value.mensagem);
            //#endregion

            //// verifica se já existe o usuário cadastrado
            //UsuarioRepository u = empresaSecurity.getUsuarioByLogin(value.email1, value.empresaId);

            //if (u == null)
            //{
            //    #region Verifica se o código de barras informado pelo usuário é igual ao existente na tabela Unidade
            //    string _situacao = db.Unidades.Where(info => info.torreId == value.torreId && info.unidadeId == value.unidadeId && 
            //                                            info.codigoBarra == value.codigo_barra.Replace(".", "").Replace(" ", "").Replace("-","")).Count() > 0 ? "A" : "D";
            //    #endregion

            //    #region Incluir o usuário
            //    UsuarioRepository usuarioRepository = new UsuarioRepository()
            //    {
            //        login = value.email1.ToLower(),
            //        nome = value.nome.Trim().Length <= 40 ? value.nome.ToUpper() : value.nome.Substring(0, 40).ToUpper(),
            //        empresaId = _empresaId,
            //        dt_cadastro = DateTime.Now,
            //        situacao = _situacao,
            //        isAdmin = "N",
            //        senha = value.senha,
            //        uri = value.uri,
            //        confirmacaoSenha = value.confirmacaoSenha
            //    };

            //    usuarioRepository = empresaSecurity.SetUsuario(usuarioRepository);
            //    if (usuarioRepository.mensagem.Code > 0)
            //        throw new App_DominioException(usuarioRepository.mensagem);

            //    u = usuarioRepository;
            //    #endregion

            //    #region Incluir o usuário no grupo de usuários
            //    int _grupoId = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.GRUPO_USUARIO).valor);
            //    UsuarioGrupo usuarioGrupo = new UsuarioGrupo()
            //    {
            //        usuarioId = usuarioRepository.usuarioId,
            //        grupoId = _grupoId,
            //        situacao = "A"
            //    };

            //    // ******** é aqui o erro
            //    if (usuarioGrupo.grupoId > 0)
            //        seguranca_db.Set<UsuarioGrupo>().Add(usuarioGrupo);

            //    //seguranca_db.SaveChanges();
            //    #endregion
            //}

            //#region incluir o condômino

            //string _url = value.uri;

            //#region Mapear repository para entity
            //Cliente entity = clienteModel.MapToEntity(value);
            //#endregion

            //entity = this.db.Set<Cliente>().Add(entity);
            //ClienteViewModel clienteViewModel = clienteModel.MapToRepository(entity); // precisa mapear para recuperar o clienteID que foi incluído
            //#endregion

            //#region Incluir o proprietário
            //if (value.ind_proprietario == "S")
            //{
            //    string _nome_conjuge = null;
            //    if (value.Dependentes.Where(info => info.tx_relacao_cliente == "Cônjuge").Count() > 0)
            //        _nome_conjuge = value.Dependentes.Where(info => info.tx_relacao_cliente == "Cônjuge").FirstOrDefault().nome;

            //    ProprietarioModel proprietarioModel = new ProprietarioModel(db);
            //    ProprietarioViewModel proprietarioViewModel = new ProprietarioViewModel()
            //    {
            //        torreId = entity.torreId,
            //        unidadeId = entity.unidadeId,
            //        dt_inicio = entity.dt_inicio,
            //        dt_fim = entity.dt_fim,
            //        nome = entity.nome.Length > 50 ? entity.nome.Substring(0,50) : entity.nome,
            //        ind_tipo_pessoa = entity.cpf_cnpj.Trim().Length == 11 ? "F" : "J",
            //        cpf_cnpj = entity.cpf_cnpj,
            //        email = entity.email1,
            //        ind_est_civil = entity.ind_est_civil,
            //        tel_contato1 = entity.telParticular1,
            //        tel_contato2 = entity.telParticular2,
            //        nome_conjuge = _nome_conjuge,
            //        endereco = entity.enderecoCom,
            //        complemento = entity.complementoEndCom,
            //        cidadeId = entity.cidadeComId,
            //        uf = entity.ufCom,
            //        cep = entity.cepCom
            //    };

            //    #region validar inclusão
            //    value.mensagem = proprietarioModel.Validate(proprietarioViewModel, Crud.INCLUIR);
            //    if (value.mensagem.Code > 0)
            //        throw new App_DominioException(value.mensagem);
            //    #endregion

            //    #region Mapear repository para entity
            //    Proprietario proprietario = proprietarioModel.MapToEntity(proprietarioViewModel);
            //    #endregion

            //    this.db.Set<Proprietario>().Add(proprietario);
            //    proprietarioViewModel = proprietarioModel.MapToRepository(proprietario); // precisa mapear para recuperar o clienteID que foi incluído
            //}
            //#endregion

            //#region Incluir o chamado para a administração
            //chamadoViewModel = new ChamadoViewModel()
            //{
            //    clienteId = entity.clienteId,
            //    areaAtendimentoId = _areaAtendimentoId,
            //    dt_chamado = DateTime.Now,
            //    assunto = "Solicitação de ativação do usuário " + (entity.nome.Length >= 15 ? entity.nome.Substring(0, 15) : entity.nome.Substring(0, entity.nome.Length)),
            //    situacao = "A"
            //};
            //chamadoViewModel.mensagemOriginal = "<h4>Validação do cadastro do condômino - Novo usuário</h4>";
            //chamadoViewModel.mensagemOriginal += "<hr>";
            //chamadoViewModel.mensagemOriginal += "<p><b>Nome Condômino: </b>" + entity.nome.ToUpper() + "</p>";
            //chamadoViewModel.mensagemOriginal += "<p><b>Login: </b>" + entity.email1 + "</p>";
            //chamadoViewModel.mensagemOriginal += "<p><b>CPF: </b>" + Funcoes.FormataCPF(entity.cpf_cnpj) + "</p>";
            //chamadoViewModel.mensagemOriginal += "<p><b>Unidade: </b>" + entity.torreId + "-" + entity.unidadeId.ToString() + "</p>";
            //chamadoViewModel.mensagemOriginal += "<hr>";
            //chamadoViewModel.uri = value.uri;

            //ChamadoModel chamadoModel = new ChamadoModel();
            //chamadoModel.db = this.db;
            //Chamado chamado = chamadoModel.ExecProcess(chamadoViewModel, Crud.INCLUIR);
            //#endregion

            //registerViewModel = value;
            //registerViewModel.usuarioId = entity.usuarioId;
            //registerViewModel.empresaId = _empresaId;

            //return entity;
        }

        public override Validate AfterInsert(RegisterViewModel value)
        {
            //EmpresaSecurity<SecurityContext> empresaSecurity = new EmpresaSecurity<SecurityContext>();

            //int _sistemaId = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.SISTEMA).valor);
            //int _areaAtendimentoId = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.AREA_ATENDIMENTO).valor);

            //#region Inserir os alertas para os usuários da secretaria
            //AreaAtendimento ae = (from aa in db.AreaAtendimentos
            //                      where aa.areaAtendimentoId == _areaAtendimentoId
            //                      select aa).FirstOrDefault();

            //int?[] _usuarioId = { ae.usuario1Id, ae.usuario2Id };

            //// obtêm o chamadoId
            //int? _chamadoId = (from cham in db.Chamados.AsEnumerable()
            //                   where cham.clienteId == value.clienteId
            //                   select cham.chamadoId).LastOrDefault();

            //for (int i = 0; i <= _usuarioId.Where(info => info.HasValue).Count() - 1; i++)
            //{
            //    AlertaRepository alerta = new AlertaRepository()
            //    {
            //        usuarioId = _usuarioId[i].Value,
            //        sistemaId = _sistemaId,
            //        dt_emissao = DateTime.Now,
            //        linkText = "<span class=\"label label-warning\">Novo Usuário</span>",
            //        url = "../Atendimento/Create?chamadoId=" + _chamadoId.ToString() + "&fluxo=2",
            //        mensagemAlerta = "<b>" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "h</b><p>" + chamadoViewModel.assunto + "</p>"
            //    };

            //    alerta.uri = value.uri;

            //    AlertaRepository r = empresaSecurity.InsertAlerta(alerta);
            //    if (r.mensagem.Code > 0)
            //        throw new DbUpdateException(r.mensagem.Message);
            //}
            //#endregion

            //#region Enviar e-mail ao condômino e à administração
            //if (db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.HABILITA_EMAIL).valor == "S")
            //{
            //    SendEmail sendMail = new SendEmail();

            //    Empresa empresa = seguranca_db.Empresas.Find(registerViewModel.empresaId);
            //    Sistema sistema = seguranca_db.Sistemas.Find(_sistemaId);

            //    MailAddress sender = new MailAddress(empresa.nome + " <" + empresa.email + ">");
            //    List<string> recipients = new List<string>();
            //    List<string> sindico = new List<string>();

            //    recipients.Add(registerViewModel.nome + "<" + registerViewModel.email1 + ">");
            //    sindico.Add(empresa.nome + " <" + empresa.email + ">");
            //    sindico.Add(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EMAIL_SINDICO).valor);

            //    string documento = registerViewModel.ind_proprietario == "S" ? "Título de propriedade" : "contrato de locação";
            //    string Subject = "Confirmação de cadastro no " + empresa.nome;
            //    string Text = "<p>Confirmação de cadastro</p>";
            //    string Html = "<p><span style=\"font-family: Verdana; font-size: larger; color: #656464\">" + sistema.descricao + "</span></p>" +
            //                  "<p><span style=\"font-family: Verdana; font-size: xx-large; color: #0094ff\">" + registerViewModel.nome.ToUpper() + "</span></p>" +
            //                  "<p></p>" +
            //                  "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Torre: <b>" + registerViewModel.torreId + "</b></span></p>" +
            //                  "<p></p>" +
            //                  "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Unidade: <b>" + registerViewModel.unidadeId.ToString() + "</b></span></p>" +
            //                  "<p></p>" +
            //                  "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Essa é uma mensagem de confirmação de seu cadastro. Seu registro no Sistema Administrativo do " + empresa.nome + " foi realizado com sucesso.</span></p>";

            //    if (!registerViewModel.usuarioId.HasValue)
            //        Html += "<p></p>" +
            //                "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Entretanto, sua conta está provisoriamente <b>DESATIVADA</b>.</span></p>";
            //    else
            //    {
            //        string asterisco = "";
            //        for (int i = 1; i <= registerViewModel.senha.Length - 1; i++)
            //            asterisco += "*";

            //        Html += "<p></p>" +
            //                "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Seu Login de acesso é: </span></p>" +
            //                "<p></p>" +
            //                "<p><span style=\"font-family: Verdana; font-size: xx-large; color: #0094ff\">" + registerViewModel.email1 + "</span></p>" +
            //                "<p></p>" +
            //                "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Sua senha informada no cadastro é: </span></p>" +
            //                "<p></p>" +
            //                "<p><span style=\"font-family: Verdana; font-size: xx-large; color: #0094ff\">" + registerViewModel.senha.Substring(0, 1) + asterisco + "</span></p>" +
            //                "<hr />";
            //    }

            //    Html += "<p></p>" +
            //            "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Solicitamos que entre em contato com a administração do condomínio o mais breve, de posse do </span><span style=\"font-family: Verdana; font-size: small; background-color: #fff1a5; color: #000\"><b>" + documento + "</b></span><span style=\"font-family: Verdana; font-size: small; color: #000\"> para que possamos regularizar seu cadastro e ativar em definitivo seu acesso ao sistema.</span></p>" +
            //            "<p></p>" +
            //            "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Através do sistema o condômino poderá:</span></p>" +
            //            "<p></p>" +
            //            "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">- Consultar os documentos e comunicados oficiais do condomínio postados pelo síndico.</span></p>" +
            //            "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">- Consultar os comunicados específicos destinados a sua torre.</span></p>" +
            //            "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">- Consultar os comunicados dos grupos a que você pertence: Academia, Tênis, etc.</span></p>" +
            //            "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">- Abrir chamados à administração como por exemplo solicitar a reserva da churrasqueira ou fazer o registro de uma ocorrência.</span></p>" +
            //            "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">- Atualizar seu cadastro (dependentes, veículos, funcionários, etc).</span></p>" +
            //            "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">- Informar à portaria um prestador ou um convidado que fará acesso em um determinado dia e horário a sua unidade.</span></p>" +
            //            "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">- Receber mensagens e alertas personalizados.</span></p>" +
            //            "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">- Consultar seu histórico de notificações.</span></p>" +
            //            "<hr />" +
            //            "<p></p>" +
            //            "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Além desses recursos, estaremos implementando outras novidades. Aguarde !</span></p>" +
            //            "<p>&nbsp;</p>" +
            //            "<p>&nbsp;</p>" +
            //            "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Obrigado,</span></p>" +
            //            "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Administração " + empresa.nome + "</span></p>";

            //    Validate result = sendMail.Send(sender, recipients, Html, Subject, Text, sindico);
            //    if (result.Code > 0)
            //        throw new App_DominioException(result);
            //}
            //#endregion

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