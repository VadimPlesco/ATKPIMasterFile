$().ready(function () {
    var checkstate = function () {
        if (atkpimfApp.viewModel && atkpimfApp.viewModel.MaxContactsMenuCount && $(".main__menu-logo").length > 0) atkpimfApp.viewModel.MaxContactsMenuCount(Math.floor(($(".main__menu-logo").position().top - $(".menu-holder__messages-bar").position().top - 40) / 55));
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



atkpimfApp.fn.datingClick = function (succesUrl, forbiddenUrl) {
    atkpimfApp.updateElementById(succesUrl, 'body');
//    if (atkpimfApp.viewModel.MyUser().CanSendMessages)
//        atkpimfApp.updateElementById(succesUrl, 'body');
//    else
//        atkpimfApp.modal.showByUrl(forbiddenUrl);
};
