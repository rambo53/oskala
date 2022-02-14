
    function validateMail() {

        $('#formResetPasswordMail').validate({

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

                const mailUser = {
                    to: $("#mailClient").val().trim(),
                };

                Swal.fire('Vérification de vos droits...');

                postMethod("../api/userSubscriber/sendMail", mailUser, function (result) {

                    if (result.isSuccess) {
                        Swal.fire(
                            result.comments,
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
        })
    }

function validateNewPassword(model) {

    //affichage info du modèle

    $("#mailUser").text(model.uemail);

    //validation complexité et affichage password dans script commons

    checkPassword($("#newPassword"), $('#affichePassword'));


    $.validator.addMethod("sameValue", function (value, element, params) {
        return this.optional(element) || $("#newPassword").val() === $("#confirmNewPassword").val();
    }, 'Les deux champs doivent être identiques.');


    $('#formResetPassword').validate({
        rules: {
            "confirm_new_password": {
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
        submitHandler: function (form) {

            if (model) {
                model.pwd = $("#newPassword").val().trim();

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
        }
    })

}

