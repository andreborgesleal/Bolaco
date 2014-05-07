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

                    value = model.SaveAll(value, Crud.INCLUIR);
                    if (value.mensagem.Code > 0)
                        throw new App_DominioException(value.mensagem);

                    Success("Registro incluído com sucesso");
                    return RedirectToAction("Login", "Account");
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

        //
        // POST: /Account/Register

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            System.Web.HttpContext web = System.Web.HttpContext.Current;
            new EmpresaSecurity<App_DominioContext>().EncerrarSessao(web.Session.SessionID);

            return RedirectToAction("Login", "Account");
        }
        
    }
}