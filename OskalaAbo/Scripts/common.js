//const jqueryValidate = require("./jquery.validate");

if (/(MSIE\ [0-9]{1})/i.test(navigator.userAgent) || (/(Trident\/[7]{1})/i.test(navigator.userAgent))) {
    alert("L'accès à ce portail est réservé à un navigateur moderne (Chrome/Firefox/Edge Chromium). Les versions antérieures d'Internet Explorer ne sont pas supportées.");
    window.location = "https://www.google.com/intl/fr_fr/chrome/";
}




function initValidator(selector, callback) {

    if (jQuery.validator) {

        $.extend($.validator.messages, {
            '*': "Valeur incorrecte",
            required: "Ce champ est obligatoire",
            url: "Adresse URL invalide",
            email: "Adresse email invalide",
            date: "Date invalide",
            dateISO: "Date invalide ( ISO ).",
            number: "Valeur numérique invalide",
            digits: "Chiffres uniquement",
            creditcard: "Numéro de carte incorrecte",
            equalTo: "Les 2 champs ne correspondent pas",
            accept: "Extension invalide",
            remote: "Ce champ nécessite d'être corrigé",
            maxlength: jQuery.validator.format("Ce champ ne doit pas dépasser {0} caractère(s)."),
            minlength: jQuery.validator.format("Ce champ nécessite {0} caractère(s) au minimum"),
            rangelength: jQuery.validator.format("Ce champ nécessite entre {0} and {1} caractère(s)"),
            range: jQuery.validator.format("La valeur doit être comprise entre {0} et {1}"),
            max: jQuery.validator.format("La valeur doit être inférieure ou égale à {0}"),
            min: jQuery.validator.format("La valeur doit être supérieure ou égale à {0}"),
        });

        $.validator.addMethod("pattern", function (value, element, param) {           

            switch ($(element).attr("pattern")) {
                case "alpha": param = /^[a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð ,.'-]+$/u; break;
                case "digits": param = /^[0-9 ]+$/; break;
                case "alphaDigits": param = /^[a-zA-Z0-9àáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð\s, '-]*$/u; break;
                case "phone": param = /^(?:(?:\+|00)33[\s.-]{0,3}(?:\(0\)[\s.-]{0,3})?|0)[1-9](?:(?:[\s.-]?\d{2}){4}|\d{2}(?:[\s.-]?\d{3}){2})$/u; break;
                case "ape": param = /^[0-9]{4}[A-Za-z]$/; break;
            }

            if (this.optional(element)) {
                return true;
            }

            return param.test(value);
        }, "Ce champ n\'a pas un format valide.");


        $(selector).validate({

            rules: {

                "confirmPassword": {
                    equalTo: "#password"
                },
            },
            lang: 'fr',
            errorElement: 'span',
            errorClass: 'help-block',
            highlight: function (element, errorClass, validClass) {
                $(element).removeClass("success").addClass("error animate__headShake");
            },
            unhighlight: function (element, errorClass, validClass) {
                $(element).removeClass("error animate__headShake").addClass("success");
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

                let indexStep = parseInt($(form).data("current-step")) || 0;

                switch (indexStep) {
                    case 1:
                        Swal.fire({
                            title: 'Vérification des informations.',
                            showConfirmButton: false
                        })

                        validateMail($("#mail").val(), function (result) {

                            if (result.isSuccess) {
                                Swal.close();
                                $(form).find(".step-" + indexStep).addClass("hidden");
                                $("#step-progress").find(".step-progress-" + indexStep).removeClass("bg-step-progress text-light").addClass("border border-step-progress text-step-progress");
                                indexStep++;
                                $(form).find(".step-" + indexStep).removeClass("hidden");
                                $("#step-progress").find(".step-progress-" + indexStep).addClass("bg-step-progress text-light").removeClass("text-step-progress");
                                $(form).data("current-step", indexStep);
                            }
                        });        
                        break;
                    case 2:
                        Swal.fire({
                            title: 'Vérification des informations.',
                            showConfirmButton: false
                        })

                        validateSiret($("#numeroSiret").val(), function (result) {

                            if (result.isSuccess) {
                                Swal.close();
                                $(form).find(".step-" + indexStep).addClass("hidden");
                                $("#step-progress").find(".step-progress-" + indexStep).removeClass("bg-step-progress text-light").addClass("border border-step-progress text-step-progress");
                                indexStep++;
                                $(form).find(".step-" + indexStep).removeClass("hidden");
                                $("#step-progress").find(".step-progress-" + indexStep).addClass("bg-step-progress text-light").removeClass("text-step-progress");
                                $(form).data("current-step", indexStep);
                            }
                        });
                        break;
                    case 3:
                        if (callback && typeof (callback) === "function") { callback(form); }
                        break;
                }

            },
            invalidHandler: function (event, validator) {
                var errors = validator.numberOfInvalids();

            },
            onkeyup: false,

        })
      
    }
}

function checkPassword(password, eyeLogo) {

    //checkbox pour afficher mdp saisi

    let isVisible = false;

    eyeLogo.on('click', function () {

        if (!isVisible) {
            password.prop('type', 'text');
            $(this).find("i").addClass("fa-eye").removeClass("fa-eye-slash");
            isVisible = true;
        }
        else {
            password.prop('type', 'password');
            $(this).find("i").addClass("fa-eye-slash").removeClass("fa-eye");
            isVisible = false;
        }
    })

    //validation password suffisament complexe

    const regMin = /[a-zÀ-ȕ]/;
    const regMaj = /[A-Z]/;
    const regDigit = /[0-9]/;
    const regChar = /[_,-,.,=,?,!,*,+,-,\/,@,%,$,#,:,;]/;
    const minSize = 5;


    password.on('input', function () {

        let valueInput = password.val();
        let nbValid = 0;

        if (valueInput.match(regMin)) { nbValid++; };
        if (valueInput.match(regMaj)) { nbValid++; };
        if (valueInput.match(regDigit)) { nbValid++; };
        if (valueInput.match(regChar)) { nbValid++; };

        if (valueInput.length >= minSize) { nbValid++; };

        passwordValidation(nbValid, valueInput.length);

    });

    //modif affichage niveau sécurité password

    function passwordValidation(nbValid, pwdLenght) {
        const pwdsecurity = $("#colorPassword");
        const minValid = 3;
        const maxValid = 5;

        pwdsecurity.removeClass();

        if (nbValid > minValid && nbValid < maxValid && pwdLenght >= minSize) {
            pwdsecurity.addClass("bg-warning text-white");
            pwdsecurity.text("moyen");
        }
        else if (nbValid === maxValid) {
            pwdsecurity.addClass("bg-success text-white");
            pwdsecurity.text("fort");
        }
        else {
            pwdsecurity.addClass("bg-danger text-white");
            pwdsecurity.text("faible");
        }

    };
}


function maskFormat() {


    $(".mask-tel-fr").mask('00 00 00 00 00');
    $(".mask-postCode").mask("00000");
    $(".mask-siret").mask("000 000 000 00000");

}




/*Ajax*/
function postMethod(url, args, callback) {

    var postparams = {
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: url,
    };
    
    if (args) {
        postparams.data = JSON.stringify(args);
    }

    postparams.success = function (data) { if (callback && typeof (callback) === "function") { callback(data); } };
    return $.ajax(postparams);
}


function getMethod(url, args, callback, useJson,t) {

    useJson = useJson | false;

    var getparams = {
        type: "GET",
        timeout: t || 30000,
        crossDomain: true,
        cache: false,
        url: url,
    };

    if (useJson) {
        getparams.contentType = "application/json;charset=utf-8";
        getparams.dataType = "json";
        if (args) {
            getparams.data = JSON.stringify(args);
        }
    } else {
        getparams.contentType = 'application/x-www-form-urlencoded';
        if (args) {
            if (Object.prototype.toString.call(args) == '[object String]') {
                getparams.data = args; //encodeURIComponent(args);
            } else {
                getparams.data = $.param(args);
            }

        }
    }

    getparams.success = function (data) { if (callback && typeof (callback) === "function") { callback(data); } };
    //getparams.error = function (XMLHttpRequest, textStatus, errorThrown) { trapAjaxErrors(XMLHttpRequest, textStatus, errorThrown, callback); };
    // getparams.error = function (XMLHttpRequest, textStatus, errorThrown) { if (callback && typeof (callback) === "function") { callback({ isSuccess: false, errorMessage: errorThrown + " " + textStatus }); } };

    return $.ajax(getparams);

}


function parseError(XMLHttpRequest, textStatus, errorThrown) {
    alert(textStatus + ' ' + errorThrown + ' code:' + XMLHttpRequest.status);
    if (XMLHttpRequest.status == 403) {
        //redirectToLogin();
    }
}

function trapAjaxErrors(XMLHttpRequest, textStatus, errorThrow, callback) {
    console.error("Ajax " + textStatus + " > " + errorThrow);
    if (callback && typeof (callback) === "function") {
        callback(textStatus);
    }
}



/*BS*/
function showBsDialog(id, title, msg, options, callback) {
    var $dialog = $("#" + id);
    var settings = $.extend({
        cancelable: false,
        dialogSize: 'm', //xs,sm,m,lg
        dialogType: '', //success,info,warning,danger,striped
        dialogInput: 1, //0,1,2,3,4
        dialogResult: 0, //0,1,2
        iconClass: '', //fa fa-spinner fa-spin
        autoClose: 0, //10000
        delay: 0,
        checkfrm: "",
        effect: 'animated bounceIn'
    }, options);

    hideBsModal(id);

    if (settings.delay > 0) {
        setTimeout(function () {
            settings.delay = 0;
            console.log("Auto Delay");
            return showBsDialog(id, title, msg, settings, callback);
        }, settings.delay);
        return;
    }

    var timer;


    $dialog.find('.modal-dialog').attr('class', 'modal-dialog').addClass('modal-' + settings.dialogSize);
    if (title) { $dialog.find('.modal-title').text(title); }
    if (msg) { $dialog.find('.modal-body p:first').html(msg); }

    if (settings.iconClass) {
        $dialog.find(".modal-title i").attr('class', settings.iconClass).show();
    } else {
        $dialog.find(".modal-title i").hide();
    }

    if (!settings.cancelable) {
        $dialog.find('.modal-header .close').hide();
        $dialog.find('.modal-footer .btn-cancel').hide();
    } else {
        $dialog.find('.modal-header .close').show();
        $dialog.find('.modal-footer .btn-cancel').show();
    }

    if (settings.checkfrm) {
        $dialog.find('.modal-footer .btn-ok').removeAttr("data-dismiss");
    }


    switch (parseInt(settings.dialogInput)) {
        case 0://Info
            $dialog.find(".modal-icon").attr("src", "../media/dlg_info.png");
            break;
        case 1://Confirmation
            $dialog.find(".modal-icon").attr("src", "../media/confirmx48.png");
            break;
        case 2://Question
            $dialog.find(".modal-icon").attr("src", "../media/confirmx48.png");
            break;
        case 3://Warning
            $dialog.find(".modal-icon").attr("src", "../media/warningx48.png");
            break;
        case 4://Erreur
            $dialog.find(".modal-icon").attr("src", "../media/failurex48.png");
            break;
        case 5://Succès
            $dialog.find(".modal-icon").attr("src", "../media/successx48.png");
            break;
        default:
            $dialog.find(".modal-icon").attr("src", "../media/failurex48.png");

    }
    switch (parseInt(settings.dialogResult)) {
        case 0: //Ok
            $dialog.find(".modal-footer .btn-ok").show();
            $dialog.find(".modal-footer .btn-yes").hide();
            $dialog.find(".modal-footer .btn-no").hide();
            $dialog.find(".modal-footer .btn-cancel").hide();
            if (settings.autoClose === 0 && typeof callback !== 'function') {
                console.log("Forced Close");
            }

            break;

        case 1: //yesNO
            $dialog.find(".modal-footer .btn-ok").hide();
            $dialog.find(".modal-footer .btn-yes").show();
            $dialog.find(".modal-footer .btn-no").show();
            $dialog.find(".modal-footer .btn-cancel").hide();
            break;

        case 2: //OkCancel
            $dialog.find(".modal-footer .btn-ok").show();
            $dialog.find(".modal-footer .btn-yes").hide();
            $dialog.find(".modal-footer .btn-no").hide();
            $dialog.find(".modal-footer .btn-cancel").show();
            break;

    }

    if (settings.dialogResult === 0 && settings.autoClose) {
        timer = setTimeout(function () {
            console.log("Auto Close Set");
            $dialog.modal('hide');
            if (typeof callback === 'function') {
                callback(-1);
            }

        }, settings.autoClose);

    }

    $dialog.find('.modal-footer button').off("click");

    $(".modal-backdrop").remove();


    if (typeof callback === 'function') {
        $dialog.find('.modal-footer button').click(function (e) {
            if (timer) { clearTimeout(timer); }
            if (settings.checkfrm != "" && parseInt($(this).attr("data-result")) == 1) {
                $("#" + settings.checkfrm).data("callback", callback);
                $("#" + settings.checkfrm).submit();
                //callback($(this).attr("data-result"));

            } else {
                hideBsModal(id); ////CHCK
                callback($(this).attr("data-result"));
            }

        });
    } else {
        $dialog.find('.modal-footer button').click(function () {
            if (timer) { clearTimeout(timer); }
            /* $dialog.modal('hide');*/
            // $(this).attr("data-dismiss","modal");
            // $(this).attr(data-target="#" + id);
        });

    }


    $dialog.modal('show');


}

function hideBsModal(id) {
    $("#" + id).modal('hide');
    //$(".modal-backdrop").removeClass("in"); //CHCK
    $(".modal-backdrop").remove();
}




/*Fichiers*/


function bytesToSize(bytes, precision) {
    var kilobyte = 1024;
    var megabyte = kilobyte * 1024;
    var gigabyte = megabyte * 1024;
    var terabyte = gigabyte * 1024;

    if (bytes == 0) return '';

    if ((bytes > 0) && (bytes < kilobyte)) {
        return bytes + ' octet(s)';

    } else if ((bytes >= kilobyte) && (bytes < megabyte)) {
        return (bytes / kilobyte).toFixed(precision) + ' Ko';

    } else if ((bytes >= megabyte) && (bytes < gigabyte)) {
        return (bytes / megabyte).toFixed(precision) + ' Mo';

    } else if ((bytes >= gigabyte) && (bytes < terabyte)) {
        return (bytes / gigabyte).toFixed(precision) + ' Go';

    } else if (bytes >= terabyte) {
        return (bytes / terabyte).toFixed(precision) + ' To';

    } else {
        return bytes + ' B';
    }
}

function getTimeStamp() {
    if (window.performance.now) {
        return window.performance.now();
    } else {
        return Date.now(); //new Date().getTime();
    }
}

/*Forms*/

function clearFormValidator(domID, forceReset) {
    forceReset = forceReset || 0;
    if (domID) {
        $("#" + domID).find("select").each(function () {
            $(this).val($(this).find('option:selected').val()).trigger('chosen:updated');
        });
        if ($("#" + domID).hasClass("data-reset") || forceReset) {
            $("#" + domID)[0].reset();
        }
        $("#" + domID).validate().resetForm();
        $("#" + domID + " .form-control").removeClass("error success");
        $("#" + domID + " .help-block").remove();
    }

}

function scrollToId(id, delay) {
    delay = delay || 500;
    $('html, body').animate({ scrollTop: $("#" + id).offset().top }, delay);
}


function lockForm(domID, active, reset) {
    if (reset) {
        clearFormValidator(domID, true);
        $("#" + domID + " .note-editor").prev().summernote("code", "");
        $("#" + domID + " .cdatepicker").datepicker('clearDates');
    }
    if (active) {
        $("#" + domID + " button:not(.locked)").prop("disabled", false).removeClass("disabled");
        $("#" + domID).find(':input').not(".locked").prop('disabled', false);

        $("#" + domID + " .note-editor").prev().summernote("enable");
    } else {
        $("#" + domID + " button:not(.locked)").prop("disabled", true).addClass("disabled");
        $("#" + domID).find(':input').not(".locked").prop('disabled', true);

        $("#" + domID + " .note-editor").prev().summernote("disable");
    }

    $("#" + domID + " select").trigger("chosen:updated");
}




function loadCssFile(url, successCallback, failureCallback) {

    $("<link/>", {
        rel: "stylesheet",
        type: "text/css",
        href: url
    }).appendTo("head");

}


/*Formattage*/

function capitalize(s) {
    if (typeof s === "string" && s.length >= 2) return s.charAt(0).toUpperCase() + s.slice(1);
    return s;
}

function parseBool(value) {
    if (typeof value === 'boolean') return value;
    if (typeof value === 'string' && value.toLowerCase() == 'true') return true;
    if (typeof value === 'string' && value.toLowerCase() == 'false') return false;
    if (typeof value === 'string' && value.toLowerCase() == 'yes') return true;
    if (typeof value === 'string' && value.toLowerCase() == 'no') return false;

    return (parseInt(value) > 0);
}


function isInt(value) {
    return !isNaN(value) && parseInt(value) == value;
}

function getDecimal(n, digits) {
    if (!n || isNaN(n)) return 0;
    if (!digits) digits = 2;
    var pown = Math.pow(10, digits);
    var roundedNumber = Math.round(parseFloat(n) * pown) / pown;
    return roundedNumber;

    //return parseFloat(n).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
}


function formatIfNecessaryDecimal(n) {
    if (isInt(n)) return n;
    return n.toFixed(2);
}


function formatDecimal(n) {
    return n.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
}

function formatDecimalAndPad3(n, units) {
    var formatn = formatDecimal(n)
    // if (n < Math.pow(n, units - 1)) {
    if (n < 10) { formatn = "00" + formatn; return formatn; }
    if (n < 100) { formatn = "0" + formatn; return formatn; }
    return formatn;
}

//Formatte un chiffre sur 2 numériques
function pad2(number) {
    return (number < 10 ? '0' : '') + number
}

function stringFormat() {
    var s = arguments[0];
    for (var i = 0; i < arguments.length - 1; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, arguments[i + 1]);
    }

    return s;
}

function encodeEntities(value) {
    var NON_ALPHANUMERIC_REGEXP = /([^\#-~| |!])/g;
    var SURROGATE_PAIR_REGEXP = /[\uD800-\uDBFF][\uDC00-\uDFFF]/g;
    return value.
        replace(/&/g, '&amp;').
        replace(SURROGATE_PAIR_REGEXP, function (value) {
            var hi = value.charCodeAt(0);
            var low = value.charCodeAt(1);
            return '&#' + (((hi - 0xD800) * 0x400) + (low - 0xDC00) + 0x10000) + ';';
        }).
        replace(NON_ALPHANUMERIC_REGEXP, function (value) {
            return '&#' + value.charCodeAt(0) + ';';
        }).
        replace(/</g, '&lt;').
        replace(/>/g, '&gt;');
}

function decodeEntities(encodedString) {
    var textArea = document.createElement('textarea');
    textArea.innerHTML = encodedString;
    return textArea.value;
}

function decodeBlock(value) {
    return $("<div>" + value + "</div>").text();
}


/*URL*/

function getAbsolutePath() {
    var loc = window.location;
    var pathName = loc.pathname.substring(0, loc.pathname.lastIndexOf('/') + 1);
    return loc.href.substring(0, loc.href.length - ((loc.pathname + loc.search + loc.hash).length - pathName.length));
}


/*Query*/
function queryStringToJSON(qs) {
    qs = qs || location.search.slice(1);

    var pairs = qs.split('&');
    var result = {};
    pairs.forEach(function (pair) {
        var pair = pair.split('=');
        var key = pair[0];
        var value = decodeURIComponent(pair[1] || '');

        if (result[key]) {
            if (Object.prototype.toString.call(result[key]) === '[object Array]') {
                result[key].push(value);
            } else {
                result[key] = [result[key], value];
            }
        } else {
            result[key] = value;
        }
    });

    return JSON.parse(JSON.stringify(result));
}

function JObjetToFormData(o) {
    if (!o) return;

    var form_data = new FormData();

    for (var key in login) {
        form_data.append(key, o[key]);
    }
    return form_data;
}


function getQueryVariable(variable) {
    var query = window.location.search.substring(1);
    var vars = query.split('&');
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split('=');
        if (decodeURIComponent(pair[0]) == variable) {
            return decodeURIComponent(pair[1]);
        }
    }
    console.log('Query variable %s not found', variable);
}



/*Dates*/

//Convertit une chaine en date et horaire
function stringToDate(s) {
    s = s.replace(/[/]/g, "-");
    s = s.split(/[-: ]/);
    var ndate = new Date(s[2], s[1] - 1, s[0], s[3], s[4], 0);
    return new Date(ndate.valueOf() - ndate.getTimezoneOffset() * 60000);
}

function restStringToDate(s) {
    var ndate = new Date(s.replace(
    /^(\d{4})(\d\d)(\d\d)(\d\d)(\d\d)$/,
    '$4:$5 $2/$3/$1'
));
    return ndate;
    //return new Date(ndate.valueOf() - ndate.getTimezoneOffset() * 60000);
}

function stringToDayDate(s) {
    s = s.replace(/[/]/g, "-");
    //alert("stringToDate" + s);
    s = s.split(/[-: ]/);
    var ndate = new Date(s[2], s[1] - 1, s[0], 0, 0, 0, 0);
    return new Date(ndate.valueOf() - ndate.getTimezoneOffset() * 60000);
}


function resultStringToDate(s) {
    if (s) {
        return new Date(parseInt(s.substr(6)));
    }
}

//Compare 2 date
function dateEquals(sd, ed) {
    if (sd && ed) {
        sd.setHours(0, 0, 0, 0);
        ed.setHours(0, 0, 0, 0);
        return sd.getTime() === ed.getTime();
    }
}


function isBlank(str) {
    return (!str || /^\s*$/.test(str));
}

function isValidDate(aDate, src) {
    if (Object.prototype.toString.call(aDate) === "[object Date]") {
        if (isNaN(aDate.getTime())) {  // d.valueOf() could also work
            return false;
        }
        else {
            if (src) {
                var comp = src.split('/');
                if (src.length < 10 || comp.length < 3) return false;
                var d = parseInt(comp[0], 10);
                var m = parseInt(comp[1], 10);
                var y = parseInt(comp[2], 10);
                return (aDate.getFullYear() == y && aDate.getMonth() + 1 == m && aDate.getDate() == d);
            } else {
                return true;
            }
        }
    }
    else {
        return false;
    }
}

//formatte une date  en dd/mm/yyyy hh:MM
function formatDateTime(aDate, noEpoch) {
    if (typeof noEpoch !== 'undefined' && parseBool(noEpoch)) {
        //var epoch = stripDate(aDate).getTime();
        //if (epoch <= 0) return "";
        if (aDate.getFullYear() == 1970) return "";
    }
    var yyyy = aDate.getFullYear().toString();
    var mm = (aDate.getMonth() + 1).toString(); // getMonth() is zero-based
    var dd = aDate.getDate().toString();
    var hh = aDate.getHours().toString();
    var MM = aDate.getMinutes().toString();
    return (dd[1] ? dd : "0" + dd[0]) + "/" + (mm[1] ? mm : "0" + mm[0]) + "/" + yyyy + " " + (hh[1] ? hh : "0" + hh[0]) + ":" + (MM[1] ? MM : "0" + MM[0]); // padding
}

function formatDateDMY(aDate, noEpoch) {
    if (parseBool(noEpoch)) {
        /*var epoch = stripDate(aDate).getTime();
        if (epoch <= 0) return "";*/
        if (aDate.getFullYear() == 1970) return "";
    }

    var yyyy = aDate.getFullYear().toString();
    var mm = (aDate.getMonth() + 1).toString(); // getMonth() is zero-based
    var dd = aDate.getDate().toString();
    return (dd[1] ? dd : "0" + dd[0]) + "/" + (mm[1] ? mm : "0" + mm[0]) + "/" + yyyy; // padding
}





/*Couleurs*/


function rainbow(numOfSteps, step) {
    // This function generates vibrant, "evenly spaced" colours (i.e. no clustering). This is ideal for creating easily distinguishable vibrant markers in Google Maps and other apps.
    var r, g, b;
    var h = step / numOfSteps;
    var i = ~~(h * 6);
    var f = h * 6 - i;
    var q = 1 - f;
    switch (i % 6) {
        case 0: r = 1; g = f; b = 0; break;
        case 1: r = q; g = 1; b = 0; break;
        case 2: r = 0; g = 1; b = f; break;
        case 3: r = 0; g = q; b = 1; break;
        case 4: r = f; g = 0; b = 1; break;
        case 5: r = 1; g = 0; b = q; break;
    }
    var c = "#" + ("00" + (~ ~(r * 255)).toString(16)).slice(-2) + ("00" + (~ ~(g * 255)).toString(16)).slice(-2) + ("00" + (~ ~(b * 255)).toString(16)).slice(-2);
    return (c);
}


function rainbowRGBA(numOfSteps, step, opacity) {
    var rgba = "rgba(" + hexToRgb(rainbow(numOfSteps, step)) + "," + opacity + ")";
    return rgba;
}


function hexToRgb(hex) {
    if (hex.substring(0, 1) === "#") { hex = hex.substring(1); }
    var bigint = parseInt(hex, 16);
    var r = (bigint >> 16) & 255;
    var g = (bigint >> 8) & 255;
    var b = bigint & 255;
    return r + "," + g + "," + b;
}



function getRandColor(brightness) {
    //6 levels of brightness from 0 to 5, 0 being the darkest
    var rgb = [Math.random() * 256, Math.random() * 256, Math.random() * 256];
    var mix = [brightness * 51, brightness * 51, brightness * 51]; //51 => 255/5
    var mixedrgb = [rgb[0] + mix[0], rgb[1] + mix[1], rgb[2] + mix[2]].map(function (x) { return Math.round(x / 2.0) })
    return "rgb(" + mixedrgb.join(",") + ")";
}
