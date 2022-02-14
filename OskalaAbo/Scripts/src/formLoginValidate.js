$(document).ready(function () {

    initValidator();

    console.log("ok validate page")

    if (jQuery.validator) {



    }

    $('#frmUsrLogin').validate({


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
            const connect = {
                userMail: $("#usrLogin").val().trim(),
                userPassword: $("#usrPwd").val().trim(),
            };


            postMethod("../Login/Connect", connect, function (result) {

            });

        },
        invalidHandler: function (event, validator) {
            var errors = validator.numberOfInvalids();
        }*/
    });
})