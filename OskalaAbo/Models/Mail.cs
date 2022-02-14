using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Azure;
using static AJsonEmail;
using RestSharp;
using Newtonsoft.Json;
using OskalaAbo.Models.interfaces;
using System.Data;
using OskalaAbo.Models.data;
using MySql.Data.MySqlClient;

namespace OskalaAbo.Models
{
    public class Mail : Flow
    {
        public string from { get; set; }
        public string fromName { get; set; }
        public string to { get; set; }
        public string template { get; set; }
        public string password { get; set; }

        private string m_lastError;

        public Mail()
        {
            password = CloudConfigurationManager.GetSetting("emailPwd");
            fromName = CloudConfigurationManager.GetSetting("emailFromName");
            from = CloudConfigurationManager.GetSetting("emailFrom");
        }
        public bool send(string address, string subject, string body)
        {
            //voir log

            try
            {
                var client = new RestClient("https://api.sendgrid.com/v3/mail/send");
                client.UserAgent = "Oskala";
                client.AutomaticDecompression = true;
                client.ReadWriteTimeout = 60 * 1000;
                client.Timeout = 60 * 1000;


                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "Bearer " + password);

                

                AJsonEmailContent email = new AJsonEmailContent();
                // email.content.Add(New AJsonEmail.Content() With {.type = "text/plain", .value = "Test email"})
                email.content.Add(new AJsonEmail.Content() { type = "text/html", value = body });
                email.from.email = from;
                email.from.name = fromName;
                var adsTo = new List<To>();
                adsTo.Add(new To() { email = address, name = address });


                email.personalizations.Add(new AJsonEmail.Personalization() { to = adsTo, subject = subject });


                JsonSerializerSettings jsonserializer = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };


                Trace.WriteLine(JsonConvert.SerializeObject(email, jsonserializer));
                request.AddParameter("application/json", JsonConvert.SerializeObject(email, jsonserializer), ParameterType.RequestBody);
                // request.AddParameter("application/json", "{""personalizations"":[{""to"":[{""email"":""srastoll@softena.com"",""name"":""John Doe""}],""subject"":""Hello, World!""}],""content"": [{""type"": ""text/plain"", ""value"": ""Heya!""}],""from"":{""email"":""contact@softena.com"",""name"":""John Doe""},""reply_to"":{""email"":""noreply@softena.com"",""name"":""John Doe""}}", ParameterType.RequestBody)
                var reply = client.Execute(request);
                if (reply.IsSuccessful)
                    return true;
                else
                {
                    m_lastError = reply.Content;
                    Trace.WriteLine("KO" + reply.Content + " " + reply.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                m_lastError = ex.Message;
                return false;
            }
        }


        public bool verifdroit(UserSubscriber userSubscriber)
        {
            return true;
        }


        #region requêtes SQL


        const string GET_TEMPLATE_MAIL_BY_ID = "SELECT modele FROM template_mail WHERE id = @id LIMIT 1";

        public AObjectTResponse<Mail> getTemplateMailByID(int id)
        {
            var mail = new AObjectTResponse<Mail>();
            mail.result = new Mail();
            MySqlConnection connection = new SqlConnect().getConnection();

            using (connection)
            {

                try
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(GET_TEMPLATE_MAIL_BY_ID, connection);

                    MySqlParameter idParam = new MySqlParameter("@id", SqlDbType.Int) { Value = id };

                    command.Parameters.Add(idParam);

                    MySqlDataReader result = command.ExecuteReader();

                    if (result.HasRows)
                    {
                        if (result.Read())
                        {
                            mail.isSuccess = true;
                            mail.result.template = result.GetString(0);
                        }
                    }
                    else
                    {
                        Trace.WriteLine("pas de résultats");
                    }

                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                }

            }

            return mail;
        }

        #endregion

        public AIntegerResponse valoriseResponse(AObjectTResponse<Mail> mailTemplate, string adresseEnvoi)
        {
            AIntegerResponse result = new AIntegerResponse();

            if (mailTemplate.isSuccess)
            {
                var okEnvoi = mailTemplate.result.send(adresseEnvoi, "test", mailTemplate.result.template);

                if (okEnvoi)
                {
                    result.isSuccess = true;
                    result.comments = "Un mail vous à été envoyé.";
                }
                else
                {
                    result.isSuccess = false;
                    result.errorMessage = "Problème lors de l'envoi du mail.";
                }

            }
            else
            {
                result.isSuccess = false;
                result.errorMessage = "Le mail n'a pas pu être envoyé, le modèle n'existe pas.";
            }

            return result;
        }

        public AIntegerResponse analyseResponse(AObjectTResponse<Mail> mailTemplate, Mail mail)
        {
           return valoriseResponse(mailTemplate, mail.to);
        }

        public AIntegerResponse analyseResponse(AObjectTResponse<Mail> mailTemplate, UserSubscriber userSubscriber)
        {
            return valoriseResponse(mailTemplate, userSubscriber.userMail);
        }

    }
}