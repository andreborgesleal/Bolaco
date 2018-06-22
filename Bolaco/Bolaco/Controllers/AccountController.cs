using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using App_Dominio.Controllers;
using App_Dominio.Security;
using App_Dominio.Entidades;
using System.Data.Entity.Validation;
using App_Dominio.Contratos;
using App_Dominio.Enumeracoes;
using DWM.Models;
using DWM.Models.Persistence;
using System.Collections.Generic;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using System.Data.Entity;
using App_Dominio.Component;
using System.Net.Mail;

namespace Bolaco.Controllers
{
    [Authorize]
    public class AccountController : SuperController
    {
        #region Inheritance
        public override int _sistema_id() { return (int)DWM.Models.Enumeracoes.Sistema.BOLACO; }

        public override string getListName()
        {
            return "Login";
        }

        public override ActionResult List(int? index, int? pageSize = 40, string descricao = null)
        {
            throw new NotImplementedException();
        }
        #endregion

        [AllowAnonymous]
        public ActionResult Index()
        {
            EmpresaSecurity<App_DominioContext> login = new EmpresaSecurity<App_DominioContext>();
            if (System.Web.HttpContext.Current != null)
                login.EncerrarSessao(System.Web.HttpContext.Current.Session.SessionID);

            ListViewGanhadores list = new ListViewGanhadores();
            IEnumerable<TicketViewModel> Ganhadores = (IEnumerable<TicketViewModel>)list.ListRepository(0, 200);
            ViewBag.Ganhadores = Ganhadores;

            ListViewParametros _parametros = new ListViewParametros();
            IEnumerable<ParametroViewModel> Parametros = (IEnumerable<ParametroViewModel>)_parametros.ListRepository(0, 200);
            ViewBag.Parametros = Parametros;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(RegisterViewModel value, FormCollection collection)
        {
            if (ModelState.IsValid)
                try
                {
                    //value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

                    #region BeforeCreate

                    #endregion

                    AccountModel model = new AccountModel();

                    value.uri = "Account/Index"; // this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

                    RegisterViewModel r = new RegisterViewModel()
                    {
                        email = value.email,
                        senha = value.senha
                    };

                    value = model.SaveAll(value, Crud.INCLUIR);
                    if (value.mensagem.Code > 0)
                        throw new App_DominioException(value.mensagem);

                    EmpresaSecurity<App_DominioContext> security = new EmpresaSecurity<App_DominioContext>();

                    security.EncerrarSessao(System.Web.HttpContext.Current.Session.SessionID);

                    // verifica se é cliente ou se é membro da administração
                    //ClienteModel clienteModel = new ClienteModel();
                    //int? clienteId = clienteModel.getClienteByLogin(value.email, security);

                    //if (clienteId.HasValue && clienteId.Value < 0)
                    //    throw new DbEntityValidationException();

                    #region Autorizar
                    Validate result = security.Autorizar(r.email, r.senha, _sistema_id(), value.clienteId.ToString());
                    if (result.Code > 0)
                        throw new ArgumentException(result.Message);
                    #endregion

                    string sessaoId = result.Field;

                    Success("Cadastro incluído com sucesso. É preciso dar o seu palpite p/ concorrer à promoção. Só o cadastro não é suficiente. Preencha o formulário abaixo e dê o seu palpite.");

                    return RedirectToAction("index", "Home");
                    //return RedirectToAction("Login", "Account");
                }
                catch (App_DominioException ex)
                {
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    if (ex.Result.MessageType == MsgType.ERROR)
                        Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    else
                        Attention(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                catch (Exception ex)
                {
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
            else
            {
                value.mensagem = new Validate()
                {
                    Code = 999,
                    Message = MensagemPadrao.Message(999).ToString(),
                    MessageBase = ModelState.Values.Where(erro => erro.Errors.Count > 0).First().Errors[0].ErrorMessage
                };
                ModelState.AddModelError("", value.mensagem.Message); // mensagem amigável ao usuário
                Attention(value.mensagem.MessageBase);
            }

            ListViewGanhadores list = new ListViewGanhadores();
            IEnumerable<TicketViewModel> Ganhadores = (IEnumerable<TicketViewModel>)list.ListRepository(0, 200);
            ViewBag.Ganhadores = Ganhadores;

            ListViewParametros _parametros = new ListViewParametros();
            IEnumerable<ParametroViewModel> Parametros = (IEnumerable<ParametroViewModel>)_parametros.ListRepository(0, 200);
            ViewBag.Parametros = Parametros;

            return View(value);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                EmpresaSecurity<App_DominioContext> security = new EmpresaSecurity<App_DominioContext>();
                try
                {
                    // verifica se é cliente ou se é membro da administração
                    ClienteModel clienteModel = new ClienteModel();
                    int? clienteId = clienteModel.getClienteByLogin(model.UserName, security);

                    if (clienteId.HasValue && clienteId.Value < 0)
                        throw new DbEntityValidationException();

                    #region Autorizar
                    Validate result = security.Autorizar(model.UserName, model.Password, _sistema_id(), clienteId.HasValue ? clienteId.ToString() : "0");
                    if (result.Code > 0)
                        throw new ArgumentException(result.Message);
                    #endregion

                    string sessaoId = result.Field;

                    return RedirectToAction("index", "Home");
                }
                catch (ArgumentException ex)
                {
                    Error(ex.Message);
                }
                catch (App_DominioException ex)
                {
                    Error("Erro na autorização de acesso. Favor entre em contato com o administrador do sistema");
                }
                catch (DbEntityValidationException ex)
                {
                    Error("Não foi possível autorizar o seu acesso. Favor entre em contato com o administrador do sistema");
                }
                catch (Exception ex)
                {
                    Error("Erro na autorização de acesso. Favor entre em contato com o administrador do sistema");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [AllowAnonymous]
        public ActionResult Forgot(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Forgot(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                EmpresaSecurity<App_DominioContext> security = new EmpresaSecurity<App_DominioContext>();
                try
                {
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        using (SecurityContext seguranca = new SecurityContext())
                        {
                            #region grava a nova senha no cadastro do usuário
                            Usuario usuario = seguranca.Usuarios.Where(info => info.empresaId == 4 && info.login == model.UserName).FirstOrDefault();
                            if (usuario == null || usuario.login == null || usuario.login == "")
                                throw new ArgumentException("E-mail não cadastrado");

                            Usuario u = seguranca.Usuarios.Find(usuario.usuarioId);

                            Random r = new Random(System.DateTime.Now.Millisecond);
                            string senha = r.Next(100000, 999999).ToString();

                            usuario.senha = security.Criptografar(senha);

                            seguranca.Entry(usuario).State = EntityState.Modified;
                            seguranca.SaveChanges();
                            #endregion

                            #region enviar a nova senha por e-mail
                            if (db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.HABILITA_EMAIL).valor == "S")
                            {
                                SendEmail sendMail = new SendEmail();

                                int _sistemaId = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.SISTEMA).valor);
                                string _email_admin = db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EMAIL_ADMIN).valor;

                                Empresa empresa = seguranca.Empresas.Find(4);
                                Sistema sistema = seguranca.Sistemas.Find(_sistemaId);

                                MailAddress sender = new MailAddress(empresa.nome + " <" + empresa.email + ">");
                                List<string> recipients = new List<string>();
                                List<string> norte = new List<string>();

                                recipients.Add(u.nome + "<" + u.login + ">");
                                norte.Add(empresa.nome + " <" + empresa.email + ">");
                                if (_email_admin != "")
                                    norte.Add(_email_admin);

                                string Subject = "[Bolaaaço 2018] Alteração de Senha ";
                                string Text = "<p>[Bolaaaço 2018] Alteração de Senha</p>";
                                string Html = "<p><span style=\"font-family: Verdana; font-size: x-large; font-weight: bold; color: #3e5b33\">" + sistema.descricao + "</span></p>" +
                                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Essa é uma mensagem de alteração de senha. Seu cadastro na campanha <b>Bolaaaço Norte Refrigeração 2018</b> foi alterado para uma nova senha.</span></p>" +
                                              "<table style=\"width: 95%; border: 0px solid #fff\">" +
                                              "<tr>" +
                                              "<td style=\"width: 55%\">" +
                                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Seu Login de acesso para dar palpites é: </span><span style=\"font-family: Verdana; font-size: large; margin-left: 20px; color: #3e5b33\"><b>" + u.login + "</b></span></p>" +
                                              "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">A nova senha gerada para o seu cadastro é: </span><span style=\"font-family: Verdana; font-size: large; margin-left: 20px; color: #3e5b33\"><b>" + senha + "</b></span></p>" +
                                              "<p></p>";

                                Html += "</td>" +
                                        "<td style=\"width: 45%; vertical-align: top; float: right; padding-right: 27px\"><img src=\"http://bolaco2018.azurewebsites.net/Content/images/selocircular.png\"></td>" +
                                        "</tr>" +
                                        "</table>" +
                                        "<hr />";

                                Html += "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Agora que a sua senha já foi atualizada, já é possível dar os seus palpites no resultado dos jogos do Brasil da primeira fase da copa e de quais seleções disputarão a grande final.</span></p>" +
                                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Acesse <a href=\"http://bolaco2018.azurewebsites.net/Account/Login\">Bolaaaaço Norte Refrigeração 2018</a> e dê os seus palpites.</span></p>" +
                                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Informe o seu login e a nova senha para realizar o acesso na área de apostas e cadastrar os seus palpite. Nesta área você poderá:</span></p>" +
                                        "<ul>" +
                                        "<li><span style=\"font-family: Verdana; font-size: small; color: #000\">Cadastrar os seus palpites para os jogos do Brasil na primeira fase.</span></li>" +
                                        "<li><span style=\"font-family: Verdana; font-size: small; color: #000\">Cadastrar o seu palpite para o resultado dos jogos do Brasil nas OITAVAS DE FINA, QUARTAS DE FINAL e SEMIFINAL (caso seja classificado).</span></li>" +
                                        "<li><span style=\"font-family: Verdana; font-size: small; color: #000\">Cadastrar o seu palpite referente as seleções que farão a grande final da Copa e o placar do jogo.</span></li>" +
                                        "<li><span style=\"font-family: Verdana; font-size: small; color: #000\">Consultar as estatíticas dos palpites dados por todos os participantes da campanha.</span></li>" +
                                        "<li><span style=\"font-family: Verdana; font-size: small; color: #000\">Consultar todos os seus palpites. Quanto mais você comprar mais palpites poderá fazer.</span></li>" +
                                        "</ul>" +
                                        "<hr />" +
                                        "<p><span style=\"font-family: Verdana; font-size: large; color: #3e5b33\"><b>BOA SORTE !</b></span></p>" +
                                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Cordialmente,</span></p>" +
                                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Administração " + empresa.nome + "</span></p>" +
                                        "<p><span style=\"font-family: Verdana; font-size: x-small; color: #333333\">Este é um e-mail automático. Por favor não responda, pois ele não será lido.</span></p>" +
                                        "</div>";

                                Validate result1 = sendMail.Send(sender, recipients, Html, Subject, Text, norte);
                                if (result1.Code > 0)
                                {
                                    result1.MessageBase = "Sua senha foi atualizada com sucesso mas não foi possível enviar seu e-mail de confirmação. Favor aguardar alguns instantes e refaça a operação.";
                                    throw new App_DominioException(result1);
                                }

                                Success("Uma nova senha foi gerada e enviada para o seu e-mail.");
                            }
                            #endregion
                        }
                    }
                    return RedirectToAction("Login", "Account");
                }
                catch (ArgumentException ex)
                {
                    Error(ex.Message);
                }
                catch (App_DominioException ex)
                {
                    Error("Erro na alteração da senha. Favor entre em contato com o administrador do sistema");
                }
                catch (DbEntityValidationException ex)
                {
                    Error("Não foi possível alterar a sua senha. Favor entre em contato com o administrador do sistema");
                }
                catch (Exception ex)
                {
                    Error("Erro na geração da senha. Favor entre em contato com o administrador do sistema");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            System.Web.HttpContext web = System.Web.HttpContext.Current;
            new EmpresaSecurity<App_DominioContext>().EncerrarSessao(web.Session.SessionID);

            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public ActionResult _Regras()
        {
            return View();
        }

        //[AllowAnonymous]
        //public ActionResult Tabelaaaco()
        //{
        //    return View();
        //}

    }
}