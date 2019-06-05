kinkyApp.fn.askedPhoto = function (userId, interestId, element) {
    kinkyApp.viewModel.sendAskPhoto(userId, interestId)
    // localStorage["askPhotoInf" + userId + "-" + interestId] = "asked";
    $("#feed-add-new-item").css("background", "none");
    $("#feed-add-new-text").text(kinkyApp.data.localization["Request_sent"]);
    element.removeAttribute("onclick");
    element.classList.contains("pointer") ? element.classList.remove("pointer") : "";
};

kinkyApp.fn.feedPhotoLoaded = function (img, type) {
    if (type == "nongif-background") {
        img.parentNode.className += img.parentNode.className ? ' fade-out' : 'fade-out';
    }
    else if (type == "gif-background") {
        var scriptTag = document.scripts[document.scripts.length - 1];
        var parentTag = scriptTag.parentNode;
        parentTag.className += parentTag.className ? ' fade-out' : 'fade-out';
    }
    else {
        img.className += img.className ? ' fade-in' : 'fade-in';
    }
    
};

kinkyApp.fn.feedGifIndicator = function (indicator) {
    var img;
    var data;
    indicator.classList.contains("playing") ? indicator.classList.remove("playing") : indicator.classList.add("playing");
    img = indicator.parentNode.querySelector('[data-imgfeedtype="gif"]');
    data = img.dataset;
    if (img.src == data.gifsrc) {
        img.src = "";
        img.src = data.jpgsrc;
    } else {
        img.src = "";
        img.src = data.gifsrc;
    };     
};