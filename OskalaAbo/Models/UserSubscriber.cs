using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using OskalaAbo.Models.data;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Data;
using System.Globalization;

namespace OskalaAbo.Models
{

    public class UserSubscriber : UserLogin
    {

        [MinLength(2, ErrorMessage = "Le prénom est obligatoire. (minimum : 2 caractères)")]
        [MaxLength(50, ErrorMessage = "Le prénom est trop grand. (maximum : 50 caractères)")]
        public string companyName { get; set; }

        [MinLength(2, ErrorMessage = "Le prénom est obligatoire. (minimum : 2 caractères)")]
        [MaxLength(50, ErrorMessage = "Le prénom est trop grand. (maximum : 50 caractères)")]
        [RegularExpression(@"^[a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð, .'-]+$",
            ErrorMessage = "Le prénom n'est pas au bon format.")]
        public string userFirstName { get; set; } = "";

        [Phone(ErrorMessage ="Le numéro n'est pas au bon format.")]
        public string userTelephone { get; set; } = "";

        public DateTime dateCreaCompte { get; set; }


        #region requete sql
        //////////////////////////////////////////////////////////////////// REQUETE SQL 



        const string GET_USER_BY_MAIL_RESET = "SELECT active FROM user WHERE mail = @mail LIMIT 1";

        public ABooleanResponse getUserByMailReset(string mail)
        {
            ABooleanResponse okEnvoi = new ABooleanResponse();
            MySqlConnection connection = new SqlConnect().getConnection();

            using (connection)
            {

                try
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(GET_USER_BY_MAIL_RESET, connection);

                    command.Parameters.AddWithValue("@mail",mail);

                    MySqlDataReader result = command.ExecuteReader();

                    


                        if (result.HasRows)
                    {
                   
                        if (result.Read())
                        {
                            okEnvoi.isSuccess = true;
                            okEnvoi.result = result.GetBoolean(0);
                        }

                    }
                    else
                    {
                        okEnvoi.errorMessage = "Pas de résultat pour cette adresse mail";
                    }

                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                }

            }

            return okEnvoi;
        }

        

        const string CREATE_USER = "INSERT INTO user VALUES(null, " +
                            "@first_name, " +
                            "@last_name, " +
                            "@company_name, " +
                            "@mail, " +
                            "@telephone, " +
                            "@password, " +
                            "@active," +
                            "null, "+
                            "@date_crea_compte)";


        public AIntegerResponse createUser(UserSubscriber userSubscriber)
        {
            AIntegerResponse userId = new AIntegerResponse();
            MySqlConnection connection = new SqlConnect().getConnection();

            using (connection)
            {

                try
                {
                    connection.Open();

                    //premiere lettre en majuscule
                    
                    TextInfo ti = new CultureInfo("fr-FR", false).TextInfo;

                    MySqlCommand command = new MySqlCommand(CREATE_USER, connection);

                    command.Parameters.AddWithValue("@first_name", ti.ToTitleCase(userSubscriber.userFirstName));

                    command.Parameters.AddWithValue("@last_name", ti.ToTitleCase(userSubscriber.userLastName));

                    command.Parameters.AddWithValue("@company_name", userSubscriber.companyName);

                    command.Parameters.AddWithValue("@mail", userSubscriber.userMail);

                    command.Parameters.AddWithValue("@telephone", userSubscriber.userTelephone);

                    command.Parameters.AddWithValue("@password", userSubscriber.userPassword);

                    command.Parameters.AddWithValue("@active", userSubscriber.isActive);

                    command.Parameters.AddWithValue("@date_crea_compte", DateTime.Now);

                    command.ExecuteNonQuery();

                    if (command.LastInsertedId > 0)
                    {
                        userId.result = (int)command.LastInsertedId;
                    }

                }
                catch (Exception e)
                {
                    userId.errorMessage = "Problème enregistrement";
                    Trace.WriteLine(e.Message);
                }

            }

            return userId;
        }



       

        #endregion

        public AIntegerResponse analyseResponse(ABooleanResponse responseData, Mail mail, int id)
        {
            AIntegerResponse result = new AIntegerResponse();

            if (responseData.result)
            {
                var mailRepoResponse = mail.getTemplateMailByID(id);
                result = mail.analyseResponse(mailRepoResponse, mail);
            }
            else if (responseData.isSuccess)
            {
                result.isSuccess = false;
                result.errorMessage = "Votre compte n'est plus actif.";
            }
            else
            {
                result.isSuccess = false;
                result.errorMessage = "Cette adresse mail n'est relié à aucun compte.";
            }

            return result;
        }

    }
}