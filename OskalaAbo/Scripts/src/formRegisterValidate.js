function createUser() {

        $.validator.addMethod("regexUserName", function (value, element, params) {
            return this.optional(element) || /^[a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð ,.'-]+$/u.test(value);
        }, 'Ce champ n\'a pas un format valide.');


        $.validator.addMethod("regexUserPhone", function (value, element, params) {
            return this.optional(element) || /^(?:(?:\+|00)33[\s.-]{0,3}(?:\(0\)[\s.-]{0,3})?|0)[1-9](?:(?:[\s.-]?\d{2}){4}|\d{2}(?:[\s.-]?\d{3}){2})$/u.test(value);
        }, 'Le numéro entré n\'est pas valide.');



        $('#formInscription').validate({

            rules: {
                "nom_client": {
                    "regexUserName": true
                },
                "prenom_client": {
                    "regexUserName": true
                },
                "tel_client": {
                    "regexUserPhone": true
                },
                /*"mot_de_passe": {
                    "regex":
                }*/
            },

            lang: 'fr',
            errorElement: 'span',
            errorClass: 'help-block',
            highlight: function (element, errorClass, validClass) {
                $(element).removeClass("success").addClass("error");
            },
            unhighlight: function (element, errorClass, validClass) {
                $(element).removeClass("error").addClass("success");
            },
            showErrors: function (errorMap, errorList) {
                this.defaultShowErrors();
            },
            errorPlacement: function (error, element) {
                if (element.parent('.input-group').length) {
                    error.insertAfter(element.parent());
                } else {
                    error.insertAfter(element);
                }
            },
            success: function (label) {

            },
            submitHandler: function (form) {
                // showBsProgress("progressModal", "Identification", "Connexion au compte en cours...", { dialogSize: 'm' });

                const newAccount = {
                    companyName: $("#nomSociete").val().trim(),
                    userLastName: $("#nomClient").val().trim(),
                    userFirstName: $("#prenomClient").val().trim(),
                    userMail: $("#mailClient").val().trim(),
                    userTelephone: $("#telClient").val().trim(),
                    userPassword: $("#motDePasse").val().trim(),
                };


                Swal.fire('Enregistrement de votre compte...');


                postMethod("../api/userSubscriber/createUserSubscriber", newAccount, function (result) {

                    if (result.isSuccess) {
                        Swal.fire(
                            'Votre compte est enregistré.',
                            'Un mail vous a été envoyé',
                            'success'
                        );
                    }
                    else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Une erreur s\'est produit durant l\'enregistrement de votre compte.',
                            text: result.errorMessage,
                        });
                    }

                });

            },
            invalidHandler: function (event, validator) {
                var errors = validator.numberOfInvalids();

            }
        })
}

function createPassword(model) {

    checkPassword($("#initializePassword"), $('#affichePassword'));

    let creaPassword = $("#initializePassword");
    let confirmCreaPassword = $("#confirmInitializePassword");
    
    $.validator.addMethod("sameValue", function (value, element, params) {
        return this.optional(element) || creaPassword.val() === confirmCreaPassword.val();
    }, 'Les deux champs doivent être identiques.');


    $('#formCreaPassword').validate({
        rules: {
            "confirm_initialize_password": {
                "sameValue": true
            },
        },

        lang: 'fr',
        errorElement: 'span',
        errorClass: 'help-block',
        highlight: function (element, errorClass, validClass) {
            $(element).removeClass("success").addClass("error");
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).removeClass("error").addClass("success");
        },
        showErrors: function (errorMap, errorList) {
            this.defaultShowErrors();
        },
        errorPlacement: function (error, element) {
            if (element.parent('.input-group').length) {
                error.insertAfter(element.parent());
            } else {
                error.insertAfter(element);
            }
        },
        success: function (label) {

        },
        /*submitHandler: function (form) {

            if (model) {
                model.pwd = creaPassword.val().trim();

                postMethod("../api/userSubscriber/updateUserSubscriber", model, function (result) {

                    if (!result.isSuccess) {
                        Swal.fire({
                            icon: 'error',
                            title: result.errorMessage,
                            text: '',
                        });
                    }

                });
            }
        }*/
    })
}
