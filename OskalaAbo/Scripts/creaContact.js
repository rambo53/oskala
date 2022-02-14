
function validateMail(mail, callback) {


    return getMethod("../Login/validateMail?mail=" + mail, null, function (result) {

        if (result.errorCode == 400 || result.errorCode == 500) {

            Swal.fire({
                icon: 'error',
                title: result.errorMessage,
                text: '',
            });

        }

        if (callback && typeof (callback) === "function") { callback(result); }
    })

}

function validateSiret(siret, callback) {


    return getMethod("../Login/validateSiret?siret=" + siret, null, function (result) {

        if (result.errorCode == 400 || result.errorCode == 500) {

            Swal.fire({
                icon: 'error',
                title: result.errorMessage,
                text: '',
            });

        }

        if (callback && typeof (callback) === "function") { callback(result); }
    })

}



function validateFormCreaContact(form) {

    let adress = {
        adresse: $("#adresseEntreprise").val().trim(),
        adresse2: $("#complementAdresseEntreprise").val().trim(),
        postCode: parseInt($("#codePostalEntreprise").val()),
        city: $("#villeEntreprise").val().trim(),
        country: $("#pays").val().trim(),
    }

        let contact = {
            title: $(form).find("input[name=titre]:checked").val().trim(),
            lastName: $("#contactNom").val().trim(),
            firstName: $("#contactPrenom").val().trim(),
            mobilPhone: $("#telephonePortable").val().trim(),
            mail: $("#mail").val().trim(),
            password: $("#password").val().trim(),
            companyName: $("#entrepriseNom").val().trim(),
            AAdress: adress,
            isMain: true,
        }

        var tabContact = new Array();
        tabContact.push(contact);

        var tabAbonnement = new Array();
        tabAbonnement.push($("#aboId").attr("value"));

        let company = {
            companyName: $("#entrepriseNom").val().trim(),
            AAdress: adress,
            legalStatus: $("#formeJuridique").val(),
            siretCode: $("#numeroSiret").val().trim(),
            CContacts: tabContact,
            CAbonnement: tabAbonnement,
        };


        Swal.fire('Enregistrement de votre compte...');


    postMethod("../Login/RegisterContact", company, function (result) {

            if (result.isSuccess) {
                Swal.fire(
                    'Votre compte est enregistré.',
                    '',
                    'success'
                );
            }
            else {
                Swal.fire({
                    icon: 'error',
                    title: result.errorMessage,
                    text: '',
                });
            }

        });

}

function bindSubscriptionItems(selecteur, obj) {
    //itération sur mes data bind prop dans mon objet cloné

    $(selecteur).find('[data-bind-prop]').each(function (j, el) {

        let target = "";
        let prop = obj[$(this).attr('data-bind-prop')];

        //itération sur ma prop si c'est une collection

        if ($.isArray(prop)) {

            $(el).parent().find(".clone").remove();

            $(prop).each(function (k, tabEl) {


                let newEl = $(el).clone();
                newEl.addClass("clone");
                newEl.text("- " + tabEl);

                $(el).parent().append(newEl);
            })

        }
        else {

            target = $(el).attr('data-bind-src') || "";

            switch (target) {
                case "": $(this).text(prop); break;
                case "text": $(this).text(prop); break;
                case "html": $(this).html(prop); break;
                case "class": $(this).addClass(prop); break;
                case "tooltip": $(this).addClass(prop); break;

                default: $(this).attr(target, prop); break;
            }
        }
    });
}



