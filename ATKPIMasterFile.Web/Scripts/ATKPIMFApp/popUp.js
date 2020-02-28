atkpimfApp.popUp = (function () {

    var popUpTimer = null;

    var self = {};

    self.showPopupByMouseOver = function (selector, url, popUpType) {
        var delay = 350;
        popUpTimer = setTimeout(function () { self.showByUrl(selector, url, popUpType, true) }, delay);

    };

    self.popUpResetTimer = function () {
        clearTimeout(popUpTimer);
    };

    self.showByUrl = function (selector, url, popUpType, showBeforeSend) {
        var popUpHtml;
        atkpimfApp.asyncRequest({
            url: url,
            beforeSend: (showBeforeSend ? function (data) {
                $('body').on('mousewheel', atkpimfApp.preventBodyScrolling);
                self.show(selector, '<div class="main__popdown main__popdown-big" onmouseleave="atkpimfApp.popUp.hideBig(this);"><div class="shared__ajax-loader-src" style="display:block;margin:0 auto;padding-top:100px;"></div></div>', popUpType);
            } : null),
            success: function (data) {
                if (showBeforeSend) {
                    popUpHtml = self.popUpHtml(data.Html)
                    
                    $("#main__popup .main__popdown").html(popUpHtml);
                } else {
                    self.show(selector, data.Html, popUpType);
                }
            }
        });
    };

    self.popUpHtml = function (html) {
        var result;
        var emptyPopUpText;
        var emptyPopUpInner;
        //console.info(html);
        if (html === undefined) {
            emptyPopUpText = "Активных пользователей не найдено";
            emptyPopUpInner = $("<div>", { 'class': "empty-popup-inner" });
            emptyPopUpInnerText = $("<p>", { 'class': "empty-popup-inner-text", text: emptyPopUpText });
            emptyPopUpInner.append(emptyPopUpInnerText);
            result = emptyPopUpInner;
        }
        else {
            result = html;
        }
        
        return result;
    };

    self.show = function (selector, html, popUpType) {
        //if (viewModel.checkUser && !viewModel.checkUser())
        //    return;
        var adminMainPopup = $("#admin-main-popup");

        (adminMainPopup.length > 0) ? adminMainPopup.hide() : "";
        var popup = $("#main__popup");

        if (popup.length <= 0) {
            popup = $("<div>", {
                'id': "main__popup",
                'class': "main__popup"
            });
        }
        popup.hide();
        var mouseoverElement = $(selector);
        var container = mouseoverElement.parent();
        
        popup.html(html);
        popup.attr("data-pop-type", popUpType);
        

        switch (popUpType) {
            case "feed-reblog-popup":
                container.css({ "position": "relative" });
                container.append(popup);
                popup.show();
                popup.css({ opacity: 0, top: 35, left: -100, marginLeft: "auto" }).show().animate({ opacity: 1, top: 30 }, "fast");
                break;
            case "feed-photo-share-popup":
                container.append(popup);
                popup.show();
                popup.css({ opacity: 0, top: 40, left: 30, marginLeft: "auto" }).show().animate({ opacity: 1, top: 35 }, "fast");
                break;
            //case "reblog-fullview-popup":
            //    container.css({ "position": "relative" });
            //    container.append(popup);
            //    popup.show();
            //    popup.css({ opacity: 0, top: 67, left: -70, marginLeft: "auto" }).show().animate({ opacity: 1, top: 62 }, "fast");
            //    break;

            case "reblog-fullview-counter-popup":
                container.css({ "position": "relative" });
                container.append(popup);
                popup.show();
                popup.css({ opacity: 0, top: 47, left: -162, marginLeft: "50%" }).show().animate({ opacity: 1, top: 42 }, "fast");
                break;
            case "like-fullview-counter-popup":
                container.css({ "position": "relative" });
                container.append(popup);
                popup.show();
                popup.css({ opacity: 0, top: 47, left: -162, marginLeft: "50%" }).show().animate({ opacity: 1, top: 42 }, "fast");
                break;

        }

        popup.click(function (e) {
            e.stopPropagation();
        });

        mouseoverElement.click(function (e) {
            e.stopPropagation();
        });

        $(document).on("mousedown.popup", function (event) {
            switch (event.which) {
                case 1:
                    if (!$(event.target).closest("#main__popup").length) {
                        $(document).off("mousedown.popup");
                        self.hide();
                    }
                    break;
            }
        });
    }

    self.hideBig = function (popup) {
        $('body').off('mousewheel', atkpimfApp.preventBodyScrolling);

        $(popup).animate({ opacity: 0 }, "fast", null, function () {
            $(popup).hide();
        });
    };

    self.hide = function () {
        $('body').off('mousewheel', atkpimfApp.preventBodyScrolling);

        var popup = $("#main__popup");
        if (popup.length > 0) {
            popup.hide();
        }
    };

    return self;
})();


var atkpimfApp = atkpimfApp || {};
atkpimfApp.hover = {};

atkpimfApp.hover = (function () {
    var self = this;
    self.show = function (element) {
        $(element).find("[data-hover]").show();
    };

    self.hide = function (element) {
        $(element).find("[data-hover]").hide();
    };
    return self;
})();


var atkpimfApp = atkpimfApp || {};
atkpimfApp.photoEdit = {};
atkpimfApp.photoEdit = (function () {
    var self = this;
    self.removePhoto = function (element) {
        $(element).closest("[data-remove]").remove();
    };
    return self;
})();