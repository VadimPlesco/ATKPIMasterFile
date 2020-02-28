atkpimfApp.fn.chatDialogsInit = function () {
    ko.applyBindings(atkpimfApp.viewModel, document.getElementById("chat-dialogs-modal"));

    $("#chat-dialogs-modal").draggable({ "handle": ".chat-dialogs-modal__modal-header", "containment": "window" });
    var loading = false;
    var needLoadMore = true;
    var cm = $(".chat-dialogs-modal__chat-dialogs-holder");
    var cp = $(".chat-dialogs__preloader");
    var sb = cm.mCustomScrollbar({
        scrollInertia:0,
        scrollButtons: { enable: true }, callbacks: {
            whileScrollingInterval: 400,
            whileScrolling: function () {
                if (mcs.draggerBottom < 150 && !loading && needLoadMore) {
                    loading = true;
                    cp.fadeIn("fast");
                    atkpimfApp.viewModel.getMoreContacts(function (contactCountLoaded) {
                        cp.fadeOut("fast");
                        var dragger = sb.find(".mCSB_dragger");
                        var container = sb.find(".mCSB_container");
                        var scrollBox = sb.find(".mCustomScrollBox");

                        if (container.height() + container.position().top > scrollBox.height()) dragger.css("top", parseInt(0 - container.position().top * scrollBox.height() / container.height()));

                        cm.mCustomScrollbar("update");
                        needLoadMore = contactCountLoaded > 0;
                        loading = false;
                    });
                }
            }
        }
    });
    $(".chat-dialogs-modal__chat-dialogs-holder").find(".mCSB_scrollTools").css({ "top": 5, "bottom": 8 });
};

atkpimfApp.fn.showChatDialogs = function () {


    var actualContacts = $(".chat-dialogs-floater__message-block").length;
    var visibleItemCount = (parseInt(($(window).height() - 64 * 2) / 64) - 1) < 6 ? 6 : (parseInt(($(window).height() - 64 * 2) / 64) - 1);
    var scrollHeight = visibleItemCount <= 10 ? visibleItemCount <= 3 ? 192 : (Math.min(visibleItemCount, actualContacts) * 64) : (Math.min(visibleItemCount, actualContacts) * 64);

    $(".chat-dialogs-modal__chat-dialogs-holder").height(scrollHeight);
    $(".chat-dialogs-modal").height(scrollHeight + 47);

    $(".chat-dialogs-modal").css({ "left": $(document).width()-480, "top": 74 });


    setTimeout(function () {
        $(".chat-dialogs-modal__chat-dialogs-holder").mCustomScrollbar("update");
        if ($(".mCustomScrollbar").children(".mCustomScrollBox").children(".mCSB_scrollTools").css("display") != "block") {
            $(".chat-dialogs-modal__chat-dialogs-holder").find(".mCS_no_scrollbar").css({ "margin-right": 0 });
        }
    }, 10);

    $("#chat-dialogs-modal")
    .on('mouseenter', function () {
        $('body').on('mousewheel', atkpimfApp.preventBodyScrolling);
    })
    .on('mouseleave', function () {
        $('body').off('mousewheel', atkpimfApp.preventBodyScrolling);
    });


    $("#chat-dialogs-modal").show();
};

atkpimfApp.fn.hideChatDialogs = function () {
    $("#chat-dialogs-modal").hide();
};