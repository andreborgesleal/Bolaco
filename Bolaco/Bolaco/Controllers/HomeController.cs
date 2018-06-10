using App_Dominio.Contratos;
using App_Dominio.Controllers;
using App_Dominio.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Security;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Bolaco.Controllers
{
    public class HomeController : SuperController
    {

        #region Inheritance
        public override int _sistema_id() { return (int)DWM.Models.Enumeracoes.Sistema.BOLACO; }

        public override string getListName()
        {
            return "Página Inicial";
        }

        public override ActionResult List(int? index, int? PageSize, string descricao = null)
        {
            throw new NotImplementedException();
        }
        #endregion


        [AuthorizeFilter]
        public ActionResult Index()
        {
            if (ViewBag.ValidateRequest)
            {
                TicketModel model = new TicketModel();

                TicketViewModel repository = model.CreateRepository();

                return View(repository);
            }
            else
                return View();
        }

        [HttpPost]
        [AuthorizeFilter]
        public ActionResult Index(TicketViewModel value, FormCollection collection)
        {
            if (ViewBag.ValidateRequest)
            {
                if (ModelState.IsValid)
                    try
                    {
                        //value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

                        #region BeforeCreate
                        value.Situacao = "1";
                        #endregion

                        TicketModel model = new TicketModel();

                        value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

                        value = model.SaveAll(value, Crud.INCLUIR);
                        if (value.mensagem.Code > 0)
                            throw new App_DominioException(value.mensagem);

                        Attention("Seu palpite foi incluído com sucesso e será validado pela administração conforme regras da campanha. Uma notificação de confirmação do palpite foi enviada para o seu e-mail.");
                        return RedirectToAction("Index", "Home");
                    }
                    catch (App_DominioException ex)
                    {
                        TicketModel model = new TicketModel();
                        value = model.CreateRepository(Request);

                        ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                        if (ex.Result.MessageType == MsgType.ERROR)
                            Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                        else
                            Attention(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    }
                    catch (Exception ex)
                    {
                        TicketModel model = new TicketModel();
                        value = model.CreateRepository(Request);

                        App_DominioException.saveError(ex, GetType().FullName);
                        ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                        Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    }
                else
                {
                    TicketModel model = new TicketModel();
                    value = model.CreateRepository(Request);

                    value.mensagem = new Validate()
                    {
                        Code = 999,
                        Message = MensagemPadrao.Message(999).ToString(),
                        MessageBase = ModelState.Values.Where(erro => erro.Errors.Count > 0).First().Errors[0].ErrorMessage
                    };

                    if (value.mensagem.MessageBase.Contains("is not valid for Dt.Compra."))
                        value.mensagem.MessageBase = "Data da compra inválida";

                    ModelState.AddModelError("", value.mensagem.Message); // mensagem amigável ao usuário
                    Attention(value.mensagem.MessageBase);
                }

                return View(value);
            }
            else
                return null;
        }

        public ActionResult _Estatistica()
        {
            ListViewEstatistica ListEstatistica = new ListViewEstatistica();
            IList<EstatisticaViewModel> results = (IList<EstatisticaViewModel>)ListEstatistica.ListRepository(0, 100);
            return View(results);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Bolaço da Norte Refrigeração.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "DWM Sisteams";

            return View();
        }

        public ActionResult _Error()
        {
            return View();
        }

        [AuthorizeFilter]
        public ActionResult ResumoGerencial()
        {
            if (ViewBag.ValidateRequest)
            {
                ListViewResumoGerencial resumo = new ListViewResumoGerencial();

                ResumoGerencialViewModel repository = ((IEnumerable<ResumoGerencialViewModel>)resumo.ListRepository(null)).FirstOrDefault();

                return View(repository);
            }
            else
                return View();

        }

        #region Alerta - segurança
        public ActionResult ReadAlert(int? alertaId)
        {
            try
            {
                EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();
                if (alertaId.HasValue && alertaId > 0)
                    security.ReadAlert(alertaId.Value);
            }
            catch
            {
                return null;
            }

            return null;
        }
        #endregion
    }
}