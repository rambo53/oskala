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
    [Validator(typeof(ABaseCompanyValidator))]
    public class ABaseCompany
    {
        #region attributs classe mère
        public long Id { get; set; } = -1;

        [Required]
        public string companyName { get; set; } = "";

        [Required]
        public string legalStatus { get; set; } = "";

        [Required]
        public string siretCode { get; set; } = "";

        [Required]
        public AAdress adress { get; set; } = null;

        [Required]
        public List<ABaseContact> contacts { get; set; } = null;

        public long idMainContact { get; set; } = -1;

        #endregion

        #region Methode BDD classe mère


        /*Creation entreprise en bdd, ajoute main contact si liste contacts de company non vide
         * ainsi que le main contact au passage
         */

        public static ALongResponse Register(ABaseCompany company)
        {
            ALongResponse result = new ALongResponse();
            result.comments = "insert in ABaseCompany";

            //permet de formatter avec des majuscule debut de string

            TextInfo ti = new CultureInfo("fr-FR", false).TextInfo;

            try
            {
                //instancie connector avec une connection et une transaction

                var connector = new MySqlConnector(true);

                //pour chaque contact de la liste fait un insert

                foreach (ABaseContact contact in company.contacts)
                {
                    result = contact.Insert(contact,connector);
                    if (result.isSuccess)
                    {
                        contact.id = result.result;
                        if(company.idMainContact <= 0 && contact.isMain)
                        {
                            company.idMainContact = contact.id;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if(!result.isSuccess || company.idMainContact <= 0)
                {
                    return result;
                }
                //insert de l'adresse pour recuperer l'id
                result = company.adress.Insert(company.adress, connector);

                if (!result.isSuccess || result.result <= 0)
                {
                    return result;
                }

                company.adress.id = result.result;

                //valorisation du dictionnaire pour company

                var dic = new Dictionary<string, object>();

                dic.Add("nom_company", ti.ToTitleCase(company.companyName));
                dic.Add("date_crea", DateTime.Now);
                dic.Add("forme_juridique", company.legalStatus);
                dic.Add("siret", company.siretCode);
                dic.Add("main_contact_id", company.idMainContact);
                dic.Add("id_adresse", company.adress.id);

                //insert de company

                result = connector.Insert("company", dic, true);

                if (!result.isSuccess || result.result <= 0)
                {
                    return result;
                }

                company.Id = result.result;

                //insert dans table associative entre company et contact

                foreach (ABaseContact contact in company.contacts)
                {

                    if (result.isSuccess)
                    {
                        result = company.RegistertAssoId(company.Id, contact.id, connector);
                    }
                    else
                    {
                        break;
                    }
                }


                if (result.isSuccess)
                {
                    connector.Commit();
                }

                connector.CloseConnection();
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.errorMessage = ex.Message;
                result.errorCode = (int)HttpStatusCode.BadRequest;
            }          

            return result;
        }

        public static ALongResponse UpdateCompany( ABaseCompany company)
        {
            var result = new ALongResponse();

            return result;
        }

        public ALongResponse RegistertAssoId(long idCompany, long idContact, MySqlConnector connector)
        {
            ALongResponse result = new ALongResponse();
            result.comments = "insertAssoId dans ABaseCompany";

            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                dic.Add("id_company", idCompany);
                dic.Add("id_contact", idContact);

                if (connector == null)
                {
                    connector = new MySqlConnector(false);
                }

                result = connector.Insert("asso_contact_company", dic, false);
            }
            catch(Exception ex)
            {
                result.isSuccess = false;
                result.errorMessage = ex.Message;
                result.errorCode = (int)HttpStatusCode.BadRequest;
            }

            return result;
        }
        /*
    const string INSERT_COMPANY = @"INSERT INTO company VALUES(null,
                            @nom,
                            @pays_location,
                            @date_crea,
                            null,
                            @forme_juridique,
                            null,
                            @adresse,
                            @complement_adresse,
                            @ville,
                            @code_postal,
                            false,
                            null,
                            false,
                            null,
                            null,
                            @siret,
                            @id_contact)";
         * 
         * 
         * 
         * MySqlConnection connection = new SqlConnect().getConnection();

        using (connection)
        {
            connection.Open();

            //premiere lettre en majuscule

            TextInfo ti = new CultureInfo("fr-FR", false).TextInfo;

            var transaction = connection.BeginTransaction();

            var command = new MySqlCommand(INSERT_COMPANY, connection, transaction);

            try
            {

                command.Parameters.AddWithValue("@nom", company.companyName);
                command.Parameters.AddWithValue("@date_crea", DateTime.Now);
                command.Parameters.AddWithValue("@forme_juridique", company.legalStatus);
                command.Parameters.AddWithValue("@siret", company.siretCode);
                command.Parameters.AddWithValue("@id_contact", company.contacts.ElementAt(0).id);

                command.ExecuteNonQuery();

                if (command.LastInsertedId > 0)
                {
                    company.Id = (int)command.LastInsertedId;
                }

                if (company.contacts.Count > 0)
                {

                    foreach (AContact contact in company.contacts)
                    {
                        contact.companyId = company.Id;
                        AIntegerResponse mainContactId = contact.insertContact(contact, command);
                        company.updateMainContactId(mainContactId.result, company.Id, command);
                    }
                }

                command.Transaction.Commit();


                result.result = company.Id;
                result.isSuccess = true;
                result.errorCode = (int)HttpStatusCode.OK;
                result.errorMessage = "Nouveau contact créé.";

                Log.log(LogLevel.Info, HttpContext.Current.Request.Url.AbsoluteUri, result);

            }
            catch (MySqlException mse)
            {
                int codeError = mse.Number;
                result.isSuccess = false;
                result.errorCode = (int)HttpStatusCode.BadRequest;

                if (codeError == 1062)
                {
                    result.errorMessage = "Le numéro de siret est déjà enregistré en BDD";
                }

                Log.log(LogLevel.Warning, HttpContext.Current.Request.Url.AbsoluteUri, result);

            }
            catch (Exception e)
            {

                result.isSuccess = false;
                result.errorMessage = "Un problème est survenue lors de l'enregistrement de votre compte entreprise.";
                result.errorCode = (int)HttpStatusCode.InternalServerError;


                Log.log(LogLevel.Warning, HttpContext.Current.Request.Url.AbsoluteUri, result);

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

        return result;
    }*/



        const string UPDATE_COMPANY_CONTACT_ID = "UPDATE company SET main_contact_id = @contact_id WHERE id = @id_company";

        public AIntegerResponse updateMainContactId(int idContact, int idCompany, MySqlCommand command = null)
        {
            AIntegerResponse result = new AIntegerResponse();
            MySqlConnection connection = new SqlConnect().getConnection();

            if (command == null)
            {
                connection = new SqlConnect().getConnection();
                connection.Open();
                command = new MySqlCommand(UPDATE_COMPANY_CONTACT_ID, connection);
            }
            else
            {
                connection = command.Connection;
                command = new MySqlCommand(UPDATE_COMPANY_CONTACT_ID, command.Connection, command.Transaction);
            }

            try
            {

                command.Parameters.AddWithValue("@contact_id", idContact);
                command.Parameters.AddWithValue("@id_company", idCompany);

                command.ExecuteNonQuery();

                result.result = idContact;
                result.isSuccess = true;
                result.errorMessage = "Le changement de main contact a bien été enregistré.";

            }
            catch (Exception e)
            {
                result.isSuccess = false;
                result.errorMessage = "Un problème est survenue lors de la mise à jour du contact.";
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
        }


        const string GET_SIRET = "SELECT siret FROM company WHERE siret = @siret LIMIT 1";

        public AResponse getSiret(string siret, AResponse result)
        {
            //AIntegerResponse result = new AIntegerResponse();
            result.comments = "getSiret";

            MySqlConnection connection = new SqlConnect().getConnection();

            using (connection)
            {
                try
                {
                    connection.Open();

                    MySqlParameter[] command = new MySqlParameter[1];

                    command[0] = new MySqlParameter("@siret", siret);

                    DataSet ds = MySqlHelper.ExecuteDataset(connection, GET_SIRET, command);
                    DataTable table = ds.Tables[0];

                    if (table.Rows.Count > 0)
                    {
                        result.isSuccess = false;
                        result.errorMessage = "Ce numéro est déjà lié à un compte.";
                    }
                    else
                    {
                        result.errorMessage = "Ce numéro est libre.";
                        result.errorCode = (int)HttpStatusCode.OK;
                        result.isSuccess = true;
                    }
                }
                catch (Exception e)
                {
                    result.isSuccess = false;
                    result.errorMessage = "Problème connexion serveur.";
                    result.errorCode = (int)HttpStatusCode.InternalServerError;
                }
            }

            return result;
        }

        #endregion

    }

    #region validator ABaseCompany
    public class ABaseCompanyValidator : AbstractValidator<ABaseCompany>
    {
        public ABaseCompanyValidator()
        {
            RuleFor(company => company.companyName).MinimumLength(2)
                .WithMessage("Le nom de l'entreprise est trop petit. (minimum : 2 caractères)");
            RuleFor(company => company.companyName).MaximumLength(50)
                .WithMessage("Le nom de l'entreprise est trop grand. (maximum : 50 caractères)");

            RuleFor(company => company.legalStatus).MinimumLength(2).WithMessage("La forme juridique est obligatoire. (minimum : 2 caractères)");
            RuleFor(company => company.legalStatus).MaximumLength(4).WithMessage("La forme juridique est trop grande. (maximum : 4 caractères)");
            RuleFor(company => company.legalStatus).Matches("^[A-Z]+$").WithMessage("La forme juridique n'est pas au bon format.");

            RuleFor(company => company.siretCode).MinimumLength(14).WithMessage("Le numero de siret est obligatoire. (minimum : 14 caractères))");
            RuleFor(company => company.siretCode).MaximumLength(17).WithMessage("Le numero de siret est trop grand. (maximum : 17 caractères)");
            RuleFor(company => company.siretCode).Matches("^[0-9 ]+$").ScalePrecision(0, 14).WithMessage("Le numéro de siret n'est pas au bon format.(uniquement des chiffres)");

        }

    }

    #endregion

    
    public class ACompany : ABaseCompany
    {
        #region classe fille
        public string apeCode { get; set; } = "";
        public string tvaCode { get; set; } = "";
        public string webSite { get; set; } = "";
        public DateTime creationDate { get; set; } = DateTime.Now;
        public DateTime updateDate { get; set; } = DateTime.Now;
        public string logo { get; set; } = "";
        public bool active { get; set; } = false;
        public bool generalConditions { get; set; } = false;
        public int idAbonnement { get; set; } = -1;


        /*public AObjectTResponse<ACompany> getCompany(int id)
        {
            var result = new AObjectTResponse<ACompany>();
            result.comments = "getCompany";

            var connector = new MySqlConnector();

            try
            {
                connector.openConnnection();

                const string GET_BY_ID = "SELECT * FROM company WHERE id = @id LIMIT 1";
                var ds = connector.initializeCommand(GET_BY_ID);

                DataTable table = ds.Tables[0];

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        result.result = new ACompany();

                        foreach (DataColumn column in table.Columns)
                        {
                            Console.WriteLine(row[column]);
                        }
                    }

                    result.isSuccess = false;
                    result.errorMessage = "Ce numéro est déjà lié à un compte.";
                }
                else
                {
                    result.errorMessage = "Ce numéro est libre.";
                    result.errorCode = (int)HttpStatusCode.OK;
                    result.isSuccess = true;
                }
            }
            catch(Exception e)
            {

            }
            
            connector.CloseConnection();

            return result;
        }*/


        public ALongResponse updateCompany(ACompany company)
        {
            ALongResponse result = new ALongResponse();

            return result;
        }
        #endregion
    }

}