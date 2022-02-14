using FluentValidation;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;

namespace OskalaAbo.Models
{
    [Validator(typeof(AAdressValidator))]
    public class AAdress
    {
        public long id { get; set; } = -1;
        [Required]
        public string adresse { get; set; } = "";

        public string adresse2 { get; set; } = "";

        [Required]
        public int postCode { get; set; } = 0;

        [Required]
        public string city { get; set; } = "";

        [Required]
        public string country { get; set; } = "";


        public ALongResponse Insert(AAdress adress, MySqlConnector connector = null)
        {
            ALongResponse result = new ALongResponse();
            result.comments = "insert in AAdress";

            bool withoutTransac = false;

            TextInfo ti = new CultureInfo("fr-FR", false).TextInfo;

            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {
                dic.Add("adress", adress.adresse);
                dic.Add("adress2", adress.adresse2);
                dic.Add("post_code", adress.postCode);
                dic.Add("city", ti.ToTitleCase(adress.city));
                dic.Add("country", ti.ToTitleCase(adress.country));

                if (connector == null)
                {
                    withoutTransac = true;
                    connector = new MySqlConnector(false);
                }

                result = connector.Insert("adresses", dic, true);
                result.errorCode = (int)HttpStatusCode.OK;

                if (withoutTransac) connector.CloseConnection();
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.errorMessage = ex.Message;
                result.errorCode = (int)HttpStatusCode.BadRequest;
            }

            return result;
        }
    }

    public class AAdressValidator : AbstractValidator<AAdress>
    {
        public AAdressValidator()
        {

            RuleFor( adress => adress.adresse).MinimumLength(2).WithMessage("L'adresse de l'entreprise est trop petit. (minimum : 2 caractères)");
            RuleFor(adress => adress.adresse).MaximumLength(100).WithMessage("L'adresse de l'entreprise est trop grande. (maximum : 100 caractères)");
            RuleFor(adress => adress.adresse).Matches("^[a-zA-Z0-9àáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð\\s, '-]*$").WithMessage("L'adresse n'est pas au bon format.");

            When(adress => adress.adresse2 != null, () => {
                RuleFor(adress => adress.adresse2).MinimumLength(2)
                    .WithMessage("Le complément d'adresse est trop petit. (minimum : 2 caractères)");
                RuleFor(adress => adress.adresse2).MaximumLength(100)
                    .WithMessage("Le complément d'adresse est trop grand. (maximum : 100 caractères)");
                RuleFor(adress => adress.adresse2).Matches("^[a-zA-Z0-9àáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð\\s, '-]*$")
                    .WithMessage("Le complément d'adresse n'est pas au bon format.");
            });

            RuleFor(adress => adress.postCode).InclusiveBetween(01000, 99999).WithMessage("Le code postal doit être un nombre compris entre 01 000 et 99 999.");
            RuleFor(adress => adress.postCode).ScalePrecision(0, 5).WithMessage("Le code postal n'est pas au bon format.(uniquement des chiffres)");

            RuleFor(adress => adress.city).MinimumLength(2).WithMessage("La ville est obligatoire. (minimum : 2 caractères))");
            RuleFor(adress => adress.city).MaximumLength(100).WithMessage("Le nom de la ville est trop grand. (maximum : 100 caractères))");
            RuleFor(adress => adress.city).Matches("^[a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð, .'-]+$").WithMessage("Le nom de la ville n'est pas au bon format.");

            RuleFor(adress => adress.country).MinimumLength(2).WithMessage("Le nom du pays est trop petit. (minimum : 2 caractères)");
            RuleFor(adress => adress.country).MaximumLength(50).WithMessage("Le nom du pays est trop grand. (maximum : 50 caractères)");
            RuleFor(adress => adress.country).Matches("^[a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð, .'-]+$").WithMessage("Le nom du pays n'est pas au bon format.");

        }

    }
}