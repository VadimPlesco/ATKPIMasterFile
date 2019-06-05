$().ready(function () {
    var checkstate = function () {
        if (kinkyApp.viewModel && kinkyApp.viewModel.MaxContactsMenuCount && $(".main__menu-logo").length > 0) kinkyApp.viewModel.MaxContactsMenuCount(Math.floor(($(".main__menu-logo").position().top - $(".menu-holder__messages-bar").position().top - 40) / 55));
    };

    checkstate();
    var timer;

    /*
    $(window).focus(function () {
        if ($(".chat-modal__chat-messages").length > 0) $(".chat-modal__chat-messages").each(function () {
            $(this).mCustomScrollbar("scrollTo", "bottom");
        });
    });
    */

    $(window).bind('resize', function (event) {
        if (this == event.target) { checkstate(); }

        $("#notify-holder").css({ "left": $(window).width() - 290 });
    });

    $("#user-profile-button").click(function () {
        var $this = $(".user-pad__profile-drop-down-menu");
        $("#user-profile-button").find(".shared__icon-down-arrow").toggleClass("shared__icon-up-arrow");
        if ($this.css("display") == "none") { $this.show(); } else { $this.hide(); }
        checkstate();
    });
});



kinkyApp.fn.datingClick = function (succesUrl, forbiddenUrl) {
    kinkyApp.updateElementById(succesUrl, 'body');
//    if (kinkyApp.viewModel.MyUser().CanSendMessages)
//        kinkyApp.updateElementById(succesUrl, 'body');
//    else
//        kinkyApp.modal.showByUrl(forbiddenUrl);
};
