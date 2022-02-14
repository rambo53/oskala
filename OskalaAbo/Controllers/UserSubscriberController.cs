using Newtonsoft.Json;
using OskalaAbo.Models;
using OskalaAbo.Models.data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace OskalaAbo.Controllers
{
    [Authorize]
    [RoutePrefix("api/userSubscriber")]
    public class UserSubscriberController : ApiController
    {

        [Route("getUserSubscriber")]
        [HttpGet]
        public async Task<AObjectTResponse<UserSubscriber>> getUserSubscriber(int id)  
        {

            AObjectTResponse<UserSubscriber> result = new AObjectTResponse<UserSubscriber>();
            
            return result;
        }


        [Route("sendMail")]
        [HttpPost]
        public async Task<AIntegerResponse> sendMail(Mail mail)
        {
            AIntegerResponse result = new AIntegerResponse();

            try
            {
                if (ModelState.IsValid && mail != null)
                {
                    UserSubscriber userSubscriber = new UserSubscriber(); //voir récupération user courant
                    var envoieOk = mail.verifdroit(userSubscriber);

                    if (envoieOk) 
                    {
                        var responseReset = userSubscriber.getUserByMailReset(mail.to);
                        result = userSubscriber.analyseResponse(responseReset, mail,2);
                    }
                    else
                    {
                        result.isSuccess = false;
                        result.errorMessage = "Vous n'avez pas les droits nécessaires.";
                    }

                }
                else
                {
                    result.isSuccess = false;

                    string messages = string.Join("; ", ModelState.Values
                                  .SelectMany(x => x.Errors)
                                  .Select(x => x.ErrorMessage));

                    result.errorMessage = messages;
                    result.errorCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (Exception e)
            {
                result.isSuccess = false;
                result.errorMessage = e.Message;
                result.errorCode = (int)HttpStatusCode.InternalServerError;
            }
            
            return result;
        }



        [Route("updateUserSubscriber")]
        [HttpPost]
        public ABooleanResponse updateUserSubscriber(ResetPasswordModel rpm)  
        {
            ABooleanResponse result = new ABooleanResponse();

            try
            {
                if (ModelState.IsValid && rpm != null) {

                    //requête update password user

                    result.isSuccess = true;
                }
                else
                {
                    result.isSuccess = false;

                    string messages = string.Join("; ", ModelState.Values
                                  .SelectMany(x => x.Errors)
                                  .Select(x => x.ErrorMessage));

                    result.errorMessage = messages;
                    result.errorCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (Exception e)
            {
                result.errorMessage = e.Message;
                result.errorCode = (int)HttpStatusCode.InternalServerError;
            }
            
            return result;
        }
        

        [Route("createUserSubscriber")]
        [HttpPost]
        public AIntegerResponse createUserSubscriber(UserSubscriber userSubscriber)
        {

             AIntegerResponse result = new AIntegerResponse();

             try
             {
                 if (ModelState.IsValid && userSubscriber!=null)
                 {
                    int idUser = userSubscriber.createUser(userSubscriber).result;

                    AObjectTResponse<Mail> mail = new Mail().getTemplateMailByID(4);
                    result = mail.result.analyseResponse(mail, userSubscriber);

                 }
                 else
                 {
                    result.isSuccess = false;

                    string messages = string.Join("; ", ModelState.Values
                                  .SelectMany(x => x.Errors)
                                  .Select(x => x.ErrorMessage));

                    result.errorMessage = messages;
                    result.errorCode = (int)HttpStatusCode.BadRequest;
                 }
             }
             catch (Exception e)
             {
                 result.errorMessage = e.Message;
                 result.errorCode = (int)HttpStatusCode.InternalServerError;
             }

            return result;
        }


        [Route("validSiret")]
        [HttpGet]
        public async Task<AResponse> validSiret(string siret)
        {

            AResponse result = new AResponse();

            if (siret != null && siret.Length >= 14 && Regex.IsMatch(siret, "^[0-9 ]+$"))
            {
                ABaseCompany company = new ABaseCompany();
                result = company.getSiret(siret, result);
            }
            else
            {
                result.isSuccess = false;
                result.errorMessage = "Le numéro de siret n'est pas valide.";
            }


            return result;
        }

        /// <summary>Vérifie la présence du mail renseigné en bdd</summary>
        [Route("validMail")]
        [HttpGet]
        public async Task<AResponse> validMail(string mail)
        {
            AResponse result = new AResponse();

            try
            {
                MailAddress m = new MailAddress(mail);
                ABaseContact contact = new ABaseContact();
                result = contact.getMail(mail);
            }
            catch (FormatException)
            {
                result.isSuccess = false;
                result.errorMessage = "L'adresse mail n'est pas au bon format.";
                result.errorCode = (int)HttpStatusCode.BadRequest;
            }


            return result;
        }




[Route("registerContact")]
        [HttpPost]
        public async Task<ALongResponse> SaveContact(ABaseCompany company)
        {

            ALongResponse result = new ALongResponse();

            if (ModelState.IsValid)
            {
                if (company.Id < 0)
                {
                    result = ABaseCompany.Register(company);
                }
                else
                {
                    result = ABaseCompany.UpdateCompany(company);
                }
                
            }
            else
            {
                result.isSuccess = false;

                string messages = string.Join("; ", ModelState.Values
                              .SelectMany(x => x.Errors)
                              .Select(x => x.ErrorMessage));

                result.errorMessage = messages;
                result.errorCode = (int)HttpStatusCode.BadRequest;
            }


            return result;
        }


        /*        if (mail != null && mail.Length <= 50 && mailValid)
{
    ABaseContact contact = new ABaseContact();
result = contact.getMail(mail);
}
else
{
    result.isSuccess = false;
    result.errorMessage = "L'adresse mail n'est pas au bon format.";
}*/
    }
}
