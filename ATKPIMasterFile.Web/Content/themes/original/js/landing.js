
atkpimfApp.fn.detectBandwidth = function () {
    var connection = navigator.connection || navigator.mozConnection || navigator.webkitConnection;
    var bandwidthChange;

    if (connection) {
        bandwidthChange = function () {
            var highBandwidth = (!connection.metered && connection.bandwidth > 2);
            console.info("Connection speed: " + connection.bandwidth);
            if (highBandwidth) {
                document.body.classList.contains("low-bandwidth") ? document.body.classList.remove("low-bandwidth") : "";
            }
            else {
                document.body.classList.contains("low-bandwidth") ? "" : document.body.classList.add("low-bandwidth");
            }
        };
        connection.addEventListener("change", bandwidthChange);
        bandwidthChange();
    }


};

atkpimfApp.fn.switchLng = function (lng, fakeLng) {
    if (fakeLng) {
        localStorage.setItem("language", fakeLng);
    }

    document.location.href = "/" + lng;
};

atkpimfApp.fn.lngSave = function () {
    var noLanguage = localStorage.getItem("language") == null ? true : false;
    if (noLanguage) {

    }
    
};

//atkpimfApp.fn.switchLngClick = function(that) {
//    if ($(that).hasClass("lang-switch_closed")) {
//        $(that).removeClass("lang-switch_closed").addClass("lang-switch_opened");
//    }
//    else {
//        $(that).removeClass("lang-switch_opened").addClass("lang-switch_closed");
//    }
//}

atkpimfApp.fn.switchLngHover = function (that) {
    document.getElementById('language-switcher').setAttribute("data-state", "active");
}

atkpimfApp.fn.switchLngOut = function (that) {
    document.getElementById('language-switcher').setAttribute("data-state", "");
}

atkpimfApp.registerInputBlur = function (element) {
    var closeButton = element.parentElement.lastElementChild;
    if (closeButton !== null) {
        element.value == "" ? closeButton.setAttribute("data-hidden", "true") : closeButton.setAttribute("data-hidden", "false");
    }
};

atkpimfApp.registerInputFocus = function (element) {
    var closeButton = element.parentElement.lastElementChild;
    element.value == "" ? closeButton.setAttribute("data-hidden", "true") : closeButton.setAttribute("data-hidden", "false");
};

atkpimfApp.registerClearInput = function (element) {
    var closeButton = element.parentElement;
    var input = closeButton.parentElement.firstElementChild;
    input.value = "";
    closeButton === null ? "" : closeButton.setAttribute("data-hidden", "true");
};


atkpimfApp.validateInputValue = function (element, type) {
    validationCheck = $(element).parent(".landing__input-block").children(".input-block__validation-check");
    validationValue = $(element).val();
    re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    if (((type == "text") && (validationValue != "")) || ((type == "email") && (re.test(validationValue))) || ((type == "integer") && (validationValue!="") && (validationValue % 1 === 0))) {
        validationCheck.fadeIn("fast");
    } else {
        validationCheck.fadeOut("fast");
    }
};

