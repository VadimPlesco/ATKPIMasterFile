kinkyApp.fn.askedAvatar = function (clickedElement, userId, event) {
    if (event) {
        if (event.stopPropagation)
            event.stopPropagation();   // W3C model
        else
            event.cancelBubble = true; // IE model
    }
    kinkyApp.viewModel.sendAskAvatar(userId);
    //localStorage["avatar" + userId] = "asked";
    var button = $(clickedElement);
    button.addClass("plain-button")
        .text(kinkyApp.data.localization["Request_sent"])
        .prop("disabled", true)
        .css("font-weight", "bold");

};

kinkyApp.fn.removeClass = function (element, className) {
    console.info("removeClass");
    var element = $(element);
    element.removeClass(className);
};