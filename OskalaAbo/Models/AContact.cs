using FluentValidation;
using FluentValidation.Attributes;
using Grpc.Core.Logging;
using MySql.Data.MySqlClient;
using OskalaAbo.Models.data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;

namespace OskalaAbo.Models
{
    [Validator(typeof(ABaseContactValidator))]
    public class ABaseContact
    {
        public long id { get; set; } = -1;

        [Required]
        public string title { get; set; } = "";

        [Required]
        public string lastName { get; set; } = "";

        [Required]
        public string firstName { get; set; } = "";

        [Required]
        public string mobilPhone { get; set; } = "";

        [Required]
        public string mail { get; set; } = "";

        [Required]
        public string password { get; set; } = "";

        [Required]
        public bool isMain { get; set; } = false;

        public DateTime creationDate { get; set; } = DateTime.Now;

        public string language { get; set; } = "FR";


        #region Methode BDD

        public ALongResponse Insert(ABaseContact contact, MySqlConnector connector = null)
        {
            ALongResponse result = new ALongResponse();
            result.comments = "insert in ABaseContact";

            TextInfo ti = new CultureInfo("fr-FR", false).TextInfo;

            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {
                dic.Add("nom_contact", ti.ToTitleCase(contact.lastName));
                dic.Add("prenom", ti.ToTitleCase(contact.firstName));
                dic.Add("titre", ti.ToTitleCase(contact.title));
                dic.Add("telephone_portable", contact.mobilPhone);
                dic.Add("mail", contact.mail);
                dic.Add("date_crea", DateTime.Now);
                dic.Add("is_main", contact.isMain);
                dic.Add("langue", ti.ToUpper(contact.language));
                dic.Add("password", contact.password);

                if (connector == null)
                {
                    connector = new MySqlConnector(false);

                }
                result = connector.Insert("contact", dic, true);

            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.errorMessage = ex.Message;
                result.errorCode = (int)HttpStatusCode.BadRequest;
            }

            return result;
        }





        const string GET_MAIL = "SELECT mail FROM contact WHERE mail = @mail LIMIT 1";

        public AResponse getMail(string mail)
        {
            AResponse result = new AResponse();
            result.comments = "getMail";

            var connector = new MySqlConnector(false);

            //using (connection)
            //{
                try
                {
                //connection.Open();
                var connection = connector.GetConnection();
                connection.Open();
                    MySqlParameter[] command = new MySqlParameter[1];

                    command[0] = new MySqlParameter("@mail", mail);

                    DataSet ds = MySqlHelper.ExecuteDataset(connection, GET_MAIL, command);
                    DataTable table = ds.Tables[0];

                    if (table.Rows.Count > 0)
                    {
                        result.isSuccess = false;
                        result.errorMessage = "Cette adresse mail est déjà utilisé.";
                        result.errorCode = (int)HttpStatusCode.BadRequest;
                    }
                    else
                    {
                        result.isSuccess = true;
                        result.errorCode = (int)HttpStatusCode.OK;
                    }
                }
                catch (Exception e)
                {
                    result.isSuccess = false;
                    result.errorCode = (int)HttpStatusCode.InternalServerError;
                    result.errorMessage = "Problème connexion serveur.";
                }
            //}

            return result;
        }

        #endregion


    }

    public class ABaseContactValidator : AbstractValidator<ABaseContact>
    {
        public ABaseContactValidator()
        {
            RuleFor(contact => contact.title).MinimumLength(2).WithMessage("Le titre est obligatoire. (minimum : 2 caractères)");
            RuleFor(contact => contact.title).MaximumLength(5).WithMessage("Le titre est trop grand. (maximum : 5 caractères)");
            RuleFor(contact => contact.title).Matches("^[A-Za-z]+$").WithMessage("Le titre n'est pas au bon format.");

            RuleFor(contact => contact.lastName).MinimumLength(2).WithMessage("Le nom est obligatoire. (minimum : 2 caractères)");
            RuleFor(contact => contact.lastName).MaximumLength(50).WithMessage("Le nom est trop grand. (maximum : 50 caractères)");
            RuleFor(contact => contact.lastName).Matches("^[a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð, .'-]+$").WithMessage("Le nom n'est pas au bon format.");

            RuleFor(contact => contact.firstName).MinimumLength(2).WithMessage("Le prénom est obligatoire. (minimum : 2 caractères)");
            RuleFor(contact => contact.firstName).MaximumLength(50).WithMessage("Le prénom est trop grand. (maximum : 50 caractères)");
            RuleFor(contact => contact.firstName).Matches("^[a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð, .'-]+$").WithMessage("Le prénom n'est pas au bon format.");

            RuleFor(contact => contact.mobilPhone).MinimumLength(2).WithMessage("Le portable est obligatoire. (minimum : 2 caractères)");
            RuleFor(contact => contact.mobilPhone).MaximumLength(50).WithMessage("Le portable est trop grand. (maximum : 50 caractères)");
            RuleFor(contact => contact.mobilPhone).Matches("^(?:(?:\\+|00)33[\\s.-]{0,3}(?:\\(0\\)[\\s.-]{0,3})?|0)[1-9](?:(?:[\\s.-]?\\d{2}){4}|\\d{2}(?:[\\s.-]?\\d{3}){2})$").WithMessage("Le portable n'est pas au bon format.");

            RuleFor(contact => contact.mail).MinimumLength(2).WithMessage("Le mail est obligatoire. (minimum : 2 caractères)");
            RuleFor(contact => contact.mail).MaximumLength(50).WithMessage("Le mail est trop grand. (maximum : 50 caractères)");
            RuleFor(contact => contact.mail).EmailAddress().WithMessage("Le mail n'est pas au bon format.");

            RuleFor(contact => contact.password).MinimumLength(5).WithMessage("Le mot de passe est trop petit. (minimum : 5 caractères)");
            RuleFor(contact => contact.password).MaximumLength(32).WithMessage("Le mot de passe est trop grand. (maximum : 32 caractères)");

            RuleFor(contact => contact.isMain).Must(contact => contact == false || contact == true).WithMessage("La valeur de main contact doit être vrai ou faux.");

        }

    }

    public class AContact : ABaseContact
    {
        public string role { get; set; } = "";
        public string phone { get; set; } = "";
        public string photo { get; set; } = "";
        public DateTime updateDate { get; set; } = DateTime.Now;
        public string contactType { get; set; } = "";
        public bool isActive { get; set; } = false;
        public string addType { get; set; } = "";


        [RegularExpression(@"^(?i)(true|false)$",
            ErrorMessage = "La validation des mails n'est pas au bon format.")]
        public bool okMail { get; set; } = false;


        #region Methode BDD

       
        /*const string INSERT_CONTACT = @"INSERT INTO contact VALUES(null, 
                                @nom,
                                @prenom,
                                @company_name,
                                null,
                                null,
                                @telephone_portable,
                                @mail,
                                @password,
                                null,
                                @adresse,
                                @complement_adresse,
                                @code_postal,
                                @ville,
                                @date_crea,
                                null,
                                @is_main,
                                @titre,
                                null,
                                false,
                                null,
                                @langue,
                                false,
                                @company_id)";

        public AIntegerResponse insertContact(AContact contact, MySqlCommand command = null)
        {
            AIntegerResponse result = new AIntegerResponse();
            result.comments = "insertContact";

            MySqlConnection connection = null;

            if (command == null)
            {
                connection = new SqlConnect().getConnection();
                connection.Open();
                command = new MySqlCommand(INSERT_CONTACT, connection);
            }
            else
            {
                connection = command.Connection;
                command = new MySqlCommand(INSERT_CONTACT, command.Connection, command.Transaction);
            }


            //premiere lettre en majuscule

            TextInfo ti = new CultureInfo("fr-FR", false).TextInfo;


            try
            {
                command.Parameters.AddWithValue("@nom", ti.ToTitleCase(contact.lastName));
                command.Parameters.AddWithValue("@prenom", ti.ToTitleCase(contact.firstName));
                command.Parameters.AddWithValue("@company_name", contact.companyName);
                command.Parameters.AddWithValue("@telephone_portable", contact.mobilPhone);
                command.Parameters.AddWithValue("@mail", contact.mail);
                command.Parameters.AddWithValue("@password", contact.password);
                command.Parameters.AddWithValue("@adresse", contact.adresse);
                command.Parameters.AddWithValue("@complement_adresse", contact.adresse2);
                command.Parameters.AddWithValue("@code_postal", contact.postCode);
                command.Parameters.AddWithValue("@ville", ti.ToTitleCase(contact.city));
                command.Parameters.AddWithValue("@date_crea", DateTime.Now);
                command.Parameters.AddWithValue("@is_main", contact.isMain);
                command.Parameters.AddWithValue("@titre", contact.title);
                command.Parameters.AddWithValue("@langue", contact.language);
                command.Parameters.AddWithValue("@company_id", contact.companyId);

                command.ExecuteNonQuery();

                if (command.LastInsertedId > 0)
                {
                    contact.id = (int)command.LastInsertedId;
                }

                result.result = contact.id;
                result.isSuccess = true;
                result.errorCode = (int)HttpStatusCode.OK;

            }
            catch (MySqlException mse)
            {
                int codeError = mse.Number;
                result.isSuccess = false;
                result.errorCode = (int)HttpStatusCode.BadRequest;

                if (codeError == 1062)
                {
                    result.errorMessage = "L'adresse mail est déjà enregistré en BDD";
                }

                Log.log(LogLevel.Warning, HttpContext.Current.Request.Url.AbsoluteUri, result);

            }
            catch (Exception e)
            {
                result.errorCode = (int)HttpStatusCode.InternalServerError;
                result.isSuccess = false;
                result.errorMessage = "Un problème est survenue lors de l'enregistrement de votre compte utilisateur.";

                Log.log(LogLevel.Warning, HttpContext.Current.Request.Url.AbsoluteUri, result);

                if (command.Transaction != null)
                {
                    try
                    {
                        command.Transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        Console.WriteLine("  Message: {0}", ex2.Message);
                    }
                }

            }
            finally
            {
                if (command.Connection == null)
                {
                    connection.Close();
                }
            }

            return result;
        }*/

        #endregion

    }
}