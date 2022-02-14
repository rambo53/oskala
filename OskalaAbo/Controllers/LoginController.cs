using OskalaAbo.Models;
using OskalaAbo.Models.data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OskalaAbo.Controllers
{
    [RoutePrefix("Login")]
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult SubscriptionProd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Connect(UserLogin userLogin)
        {
            try
            {
                userLogin.userMail = Request["username"];
                userLogin.userPassword = Request["password"];

                if(ModelState.IsValid && userLogin != null)
                {
                    var oklog = userLogin.getUserByMailLogin(userLogin.userMail, userLogin.userPassword);

                    if (oklog.result.isActive)
                    {
                        return RedirectToAction("Index", "Home", oklog.result);
                    }
                    else if (!oklog.result.isActive)
                    {
                        AResponse response = new AResponse();
                        response.errorMessage = "Votre compte n'est plus actif";

                        return View("Login", response);
                    }
                    else
                    {
                        AResponse response = new AResponse();
                        response.errorMessage = "Les informations rentrées ne sont pas bonne ou ce compte n'existe pas.";

                        return View("Login", response);
                    }
                }
                else
                {
                    string messages = string.Join("; ", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));

                    AResponse response = new AResponse();
                    response.errorMessage = messages;

                    return RedirectToAction("Login", "Login", response);
                }

            }
            catch(Exception e)
            {
                AResponse response = new AResponse();
                response.errorMessage = e.Message;

                return View("Login",response);
            } 

        }

        [HttpGet]
        [Route("CreaContactProd")]
        public ActionResult CreaContactProd(int id = -1)
        {
            //verif id abo existe bdd
            //return crea avec abonnement
            //sinon retourne sur page abonnement avec message erreur
            AAbonnement abonnement = new AAbonnement();
            abonnement.id = 2;
            abonnement.name = "Vador";
            abonnement.description = "Offre parfaite pour gérer vos troupes à travers la galaxie.";
            abonnement.offerThemeClass = "amber-500";
            abonnement.freeTrial = true;

            return View(abonnement);
        }

        [HttpGet]
        [Route("validateMail")]
        public async Task<JsonResult> validateMail(string mail)
        {

            var api = new UserSubscriberController();
            var result = await api.validMail(mail);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [Route("validateSiret")]
        public async Task<JsonResult> validateSiret(string siret)
        {

            var api = new UserSubscriberController();
            var result = await api.validSiret(siret);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [Route("RegisterContact")]
        public async Task<JsonResult> RegisterContact(ABaseCompany company)
        {

            if (ModelState.IsValid)
            {

                var api = new UserSubscriberController();
                var result = await api.SaveContact(company);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = new AResponse();
                result.isSuccess = false;

                string messages = string.Join("; ", ModelState.Values
                              .SelectMany(x => x.Errors)
                              .Select(x => x.ErrorMessage));

                result.errorMessage = messages;

                return Json(result, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        [Route("Subscription")]
        public ActionResult Subscription(ABaseCompany company = null)
        {
            return View();
        }


        [HttpGet]
        [Route("TestForm")]
        public ActionResult TestForm()
        {
            return View();
        }

    }
}