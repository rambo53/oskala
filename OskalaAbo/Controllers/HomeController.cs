using Newtonsoft.Json;
using OskalaAbo.Models;
using OskalaAbo.Models.data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace OskalaAbo.Controllers
{

    public class HomeController : Controller
    {

        [HttpGet]
        public ActionResult Index(UserBasic userBasic)
        {

            if (ModelState.IsValid && userBasic != null)
            {
                return View();
                /*if (!userBasic.isActive)
                {
                    AIntegerResponse newUserUpdateactive = userBasic.updateActiveNewUser(userBasic.Id);

                    if (newUserUpdateactive.result > 0)
                    {
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Login", "Login");
                    }
                }
                else
                {
                    return View();
                }*/
               
            }
            else
            {
                string messages = string.Join("; ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                return RedirectToAction("Login", "Login",messages);
            }

        }

        [HttpGet]
        public ActionResult RegisterSub()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Abonnement()
        {
            return View();
        }


        [HttpGet]
        public ActionResult createPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                return View("RegisterSub", model);
            }
            else
            {
                AResponse result = new AResponse();
                result.errorMessage = "retentez votre chance";
                return View("RegisterSub", result);
            }
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RegisterNewPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                return View("ResetPassword", model);
            }
            else
            {
                AResponse result = new AResponse();
                result.errorMessage = "retentez votre chance";
                return View("ResetPassword", result);
            }
            
        }


    }
}
