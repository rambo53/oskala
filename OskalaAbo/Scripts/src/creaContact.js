
function validateMail(mail, callback) {


    return getMethod("../Login/validMail?mail=" + mail, null, function (result) {

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


    return getMethod("../Login/validSiret?siret=" + siret, null, function (result) {

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
        city: $("#companyCity").val().trim(),
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
            isMain: true,
        }

        let tabContact = new Array();
        tabContact.push(contact);

        let company = {
            companyName: $("#entrepriseNom").val().trim(),
            legalStatus: $("#formeJuridique").val(),
            siretCode: $("#numeroSiret").val().trim(),
            adress: adress,
            contacts: tabContact,
        };


    Swal.fire({
        title: 'Enregistrement de votre compte...',
        showConfirmButton: false
    })


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



