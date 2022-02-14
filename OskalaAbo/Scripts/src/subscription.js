/*function getSubcription(model) {

    console.log("get ok");

    var template = $(".offre-model");

    //itération sur mes objets reçus

    $(model).each(function (i, obj) {

        var tempClone = template.clone();
        tempClone.data("obj", obj);

        //$(tempClone).find('fieldset').attr("class", "").addClass(obj.offerThemeClass + " fieldset-blue rounded formule m-2");

        if (obj.isSpecial) {

            $(tempClone).find("#special").removeClass("special-visi");

        }

        bindSubscriptionItems(tempClone, obj);

        $("#div-offre").append(tempClone);

        tempClone.removeClass("hidden");

    });

    // Methode pour alimenter ma div présentant le détails de chaque offres

    $('.details-model').click(function () {

        let objSelect = $(this).closest(".offre-model").data("obj");

        bindSubscriptionItems('#details', objSelect);

        $('#details').removeClass("hidden");
    });


    // Methode de redirection

    $(".clickBtn").click(function (e) {

        e.preventDefault();
        const id = $(this).attr("data-bind-id");

        window.location = $(this).attr("href") + id;
    })


    // listener on click pour le switch mensuel annuel

    $("#switch").click(function () {

        let isChecked = $(this).prop("checked");

        if (isChecked == true) {
            $("#year").addClass("text-primary").removeClass("opa-switch");
            $("#month").removeClass("text-primary").addClass("opa-switch");
        }
        else {
            $("#year").removeClass("text-primary").addClass("opa-switch");
            $("#month").addClass("text-primary").removeClass("opa-switch");
        }
    })
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
}*/



$(document).ready(function () {

    //var model = JSON.parse('@Html.Raw(Json.Encode(Model))');





    var obj1 = {
        id: 1,
        name: 'Pinhead',
        isSpecial: false,
        specialDesc: "Offre du moment",
        cBenefits: ["Planning", "Télégestion"],
        cItems: ["option 1", "option 2", "option 3"],
        mensualPrice: 29,
        description: "No tears, please. It's a waste of good suffering...",
        avatar: "https://fr.techtribune.net/wp-content/uploads/2021/08/3867530-hellraiser-1021x580.jpg",
        offerThemeClass: "light-green-500",
    };

    var obj2 = {
        id: 2,
        name: 'Vador',
        isSpecial: true,
        specialDesc: "Offre du moment",
        cBenefits: ["Planning", "Télégestion"],
        cItems: ["option 1", "option 2", "option 3", "option 4"],
        mensualPrice: 39,
        description: "Offre parfaite pour gérer vos troupes à travers la galaxie.",
        avatar: "https://fr.web.img4.acsta.net/r_654_368/newsv7/20/09/24/15/36/1415597.jpg",
        offerThemeClass: "amber-500",
    };

    var model = new Array();
    model.push(obj1);
    model.push(obj2);

    var template = $(".offre-model");

    //itération sur mes objets reçus

    $(model).each(function (i, obj) {

        var tempClone = template.clone();
        tempClone.data("obj", obj);

        $(tempClone).find('fieldset').attr("class", "").addClass(obj.offerThemeClass + " fieldset-blue rounded formule m-2");

        $(tempClone).find('[data-bind-prop="avatar"]').attr("src", obj.avatar);

        if (obj.isSpecial) {

            $(tempClone).find('[data-bind-prop="specialDesc"]').removeClass("hidden");

        }

        bindSubscriptionItems(tempClone, obj);

        tempClone.removeClass("hidden");

        $("#div-offre").append(tempClone);

    });

    $('.details-model').click(function () {

        let objSelect = $(this).closest(".offre-model").data("obj");

        $('#details').find('fieldset').attr("class", "").addClass(objSelect.offerThemeClass + " fieldset-blue rounded m-2");

        bindSubscriptionItems('#details', objSelect);

        $('#details').removeClass("hidden");
    });
})

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
