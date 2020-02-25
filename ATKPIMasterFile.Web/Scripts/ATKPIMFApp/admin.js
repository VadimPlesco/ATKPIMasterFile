$(function () {
    $(".main__menu-logo")
    .attr("onclick", "kinkyApp.modal.showByUrl('/ctrl/admin/ModalMenu');")
    .children("span")
    .css("background", "none")
    .html("<span style='color:pink'>Kinky</span><span style='color:white'>Admin</span>");

    +function adminUserModerate() {
        $(document).on("mouseenter", ".shared__user-avatar", function (e) {
            var avatar = $(this);
            var adminElement;
            var url = avatar.attr("href") || avatar.data('profile-url');

            if (url === null || url === undefined) {
                return;
            }

            if (avatar.parent('[data-admin-user="container"]').length <= 0) {
                avatar.wrap('<div data-admin-user="container" class="admin-user-moderate-container"></div>');
            }

            adminElement = $("<button>", { 'class': "shared__button-feed theme-item__button admin-button-photo-user", text: "Модерация", "data-user-moderate-button": "" });
            avatar.parent().append(adminElement);

            adminElement.on("click", function (event) {
                kinkyApp.modal.showByUrl('/ctrl/admin/ModerateUser' + url.replace('/u', ''));
            });
        });

        $(document).on("mouseleave", '[data-admin-user="container"]', function (event) {
            var parent = $(this).parent();
            parent.find("[data-user-moderate-button]").remove();
        });
    }();

    +function adminProfileAvatarModerate() {
        /* Show admin button over the folder in user profile */
        $(document).on("mouseenter", "[data-userid]", function (event) {
            var adminElement;
            var adminElementClass;
            var itemIdElement = $(this).closest("[data-userid]");;
            adminElementClass = (itemIdElement.hasClass("image-folder-top-inner") ? "admin-button-folder-inside" : "admin-button-folder-profile");
            adminElement = $("<button>", { 'class': "shared__button-feed theme-item__button" + " " + adminElementClass, text: "Модерация", "data-user-moderate-button": "" });
            itemIdElement.append(adminElement);
            adminElement.on("click", function () {
                var itemIdValue = itemIdElement.attr("data-userid").split('-');
                kinkyApp.modal.showByUrl('/ctrl/admin/ModerateUser/' + itemIdValue[0]);

            });
        });

        /* Hide admin button over the folder in user profile */
        $(document).on("mouseleave", "[data-userid]", function (event) {
            $(this).find("[data-user-moderate-button]").remove();
        });
    }();

    +function adminFeedModerate() {
        /* Show admin button over the photo in feed */
        $(document).on("mouseenter", "[data-itemId]", function (event) {
            var adminElement = $("<button>", { 'class': "shared__button-feed theme-item__button admin-button-photo-feed", text: "Модерация", "data-feed-moderate-button": "" });
            var itemIdElement = $(this).closest("[data-itemId]");
            itemIdElement.append(adminElement);

            if (itemIdElement.find("[type=checkbox]").length == 0) {
                var itemIdValue = itemIdElement.attr("data-itemId");
                var adminElementImgcheckbox = $("<input>", { type: "checkbox", id: "admin-checkbox-" + itemIdValue, 'class': "admin-checkbox", "data-selectImgcheckbox": itemIdValue, "data-feed-moderate-checkbox": "" });
                var adminElementImgcheckboxLabel = $("<label>", { 'class': "admin-checkbox-label", "for": "admin-checkbox-" + itemIdValue, "data-feed-moderate-label": "" });
                itemIdElement.append(adminElementImgcheckbox);
                itemIdElement.append(adminElementImgcheckboxLabel);

                adminElementImgcheckbox.on("change", function () {
                    CheckStatus();
                });
            }

            adminElement.on("click", function () {
                var itemIdValue = itemIdElement.attr("data-itemId");//.split('-');
                //kinkyApp.modal.showByUrl('/ctrl/admin/ModerateInterestItems?userId=' + itemIdValue[0] + '&interestItemId=' + itemIdValue[1]);
                kinkyApp.modal.showByUrl('/ctrl/admin/ModerateInterestItems?itemsIdValue=' + itemIdValue);
            });
        });

        /* Hide admin button over the photo in feed */
        $(document).on("mouseleave", "[data-itemId]", function (event) {
            $(this).closest("[data-itemId]").find("[data-feed-moderate-button]").remove();
            var el = $(this).closest("[data-itemId]").find("[data-feed-moderate-checkbox]");
            if (!el.is(':checked')) {
                el.remove();
                $(this).closest("[data-itemId]").find("[data-feed-moderate-label]").remove();
            }
        });
    }();


    +function adminProfileFolderModerate() {
        /* Show admin button over the folder in user profile */
        $(document).on("mouseenter", "[data-interestid]", function (event) {
            var adminElement;
            var adminElementClass;
            var itemIdElement = $(this);
            adminElementClass = (itemIdElement.hasClass("image-folder-top-inner") ? "admin-button-folder-inside" : "admin-button-folder-profile");
            adminElement = $("<button>", { 'class': "shared__button-feed theme-item__button" + " " + adminElementClass, text: "Модерация", "data-folder-moderate-button": "" });
            itemIdElement.append(adminElement);
            adminElement.on("click", function () {
                var itemIdValue = itemIdElement.attr("data-interestid").split('-');
                kinkyApp.modal.showByUrl('/ctrl/admin/ModerateInterest?userId=' + itemIdValue[0] + '&interestId=' + itemIdValue[1]);
                //alert(itemIdValue);
            });
        });

        /* Hide admin button over the folder in user profile */
        $(document).on("mouseleave", "[data-interestid]", function (event) {
            $(this).find("[data-folder-moderate-button]").remove();
        });
    }();

    +function adminFullviewFolderModerate() {
        /* Show admin button over the folder in user profile */
        $(document).on("mouseenter", "[data-fullviewInterestid]", function (event) {
            var adminElement;
            var adminElementClass;
            var itemIdElement = $(this).closest("[data-fullviewInterestid]");
            adminElementClass = "admin-button-fullview-folder";
            adminElement = $("<button>", { 'class': "shared__button-feed theme-item__button" + " " + adminElementClass, text: "Модерация", "data-fullview-folder-moderate-button": "" });
            itemIdElement.append(adminElement);
            adminElement.on("click", function () {
                var itemIdValue = itemIdElement.attr("data-fullviewInterestid").split('-');
                kinkyApp.modal.showByUrl('/ctrl/admin/ModerateInterest?userId=' + itemIdValue[0] + '&interestId=' + itemIdValue[1]);

            });
        });

        /* Hide admin button over the folder in user profile */
        $(document).on("mouseleave", "[data-fullviewInterestid]", function (event) {
            $(this).closest("[data-fullviewInterestid]").find("[data-fullview-folder-moderate-button]").remove();
        });
    }();

    function ShowAdminPopupByUrl(selector, url, type) {
        kinkyApp.asyncRequest({
            url: url,
            success: function (data) { ShowAdminPopup(selector, data.Html, type); }
        });
    }

    function ShowAdminPopup(selector, html, type) {
        var popup = $("#main__popup");
        var adminMainPopup = $("#admin-main-popup");
        (popup.length > 0) ? popup.remove() : "";

        if (adminMainPopup.length > 0) {
            adminMainPopup.remove();
        }
        adminMainPopup = $("<div>", {
            'id': "admin-main-popup",
            'class': "main__popup"
        }).hide();

        var button = $(selector);



        var position = button.position();
        var container = button.parent();

        var shiftLeft = (button.outerWidth()) / 2 - 134;
        var shiftTop = position.top + button.outerHeight() + 15;
        adminMainPopup.css({ "top": shiftTop, "left": shiftLeft });
        adminMainPopup.html(html);
        container.append(adminMainPopup);
        adminMainPopup.attr("data-pop-type", type);


        if (adminMainPopup.parents("#photoDetailwraper").length > 0) {
            adminMainPopup.css({ opacity: 0, top: 75, marginLeft: 10 }).show().animate({ opacity: 1, top: 70 }, "fast");
        }
        else if (adminMainPopup.parents(".online-user-block-item__online-user-header").length > 0) {
            adminMainPopup.css({ opacity: 0, top: 65 }).show().animate({ opacity: 1, top: 60 }, "fast");
        }
        else if (adminMainPopup.parent(".feed-avatar-container").length > 0) {
            adminMainPopup.css({ opacity: 0, top: 75, marginLeft: 20 }).show().animate({ opacity: 1, top: 70 }, "fast");
        }
        else if (adminMainPopup.parent(".repost-avatar-header").length > 0) {
            adminMainPopup.css({ opacity: 0, top: 70, marginLeft: 0 }).show().animate({ opacity: 1, top: 65 }, "fast");
        }
        else if (adminMainPopup.parent(".avatar-folder").length > 0) {
            adminMainPopup.css({ opacity: 0, top: 70, marginLeft: -125 }).css("left", "auto").show().animate({ opacity: 1, top: 65 }, "fast");
        }
        else if (adminMainPopup.parent(".user-avatar_center-block").length > 0) {
            adminMainPopup.css({ opacity: 0, top: 70, marginLeft: -125 });
        }
        else if (adminMainPopup.parent(".status__message-block").length > 0) {
            adminMainPopup.css({ opacity: 0, left: -125, top: 60, marginLeft: 0 }).show().animate({ opacity: 1, top: 55 }, "fast");
        }



        else {
            adminMainPopup.css({ opacity: 0, top: 40, marginLeft: 0 }).show().animate({ opacity: 1, top: 35 }, "fast");
        }

        adminMainPopup.click(function (e) {
            e.stopPropagation();
        });

        button.click(function (e) {
            e.stopPropagation();
        });

        $(document).on("mousedown.adminPopupCloseOut", function (event) {
            switch (event.which) {
                case 1:
                    if (!$(event.target).closest("#admin-main-popup").length) {
                        $(document).off("mousedown.adminPopupCloseOut");
                        HideAdminPopup();
                    }
                    break;
            }
        });
    }

    function HideAdminPopup() {
        var adminMainPopup = $("#admin-main-popup");
        if (adminMainPopup.length > 0) {
            adminMainPopup.remove();
        }
    }

});

function CheckStatus() {
    var count = 0;
    var allUserIds = [];

    $("[data-selectImgcheckbox]").each(function (index, value) {
        var el = $(value);
        if (el.is(':checked')) {
            count++;

            var itemIdValue = el.attr('data-selectImgcheckbox').split('-');
            allUserIds.push(itemIdValue[0]);
        }
    });

    if (count == 0) {
        DelAdminPanel();
    }
    else if ($(".admin-panel").length == 0) {
        AddAdminPanel();
    }
    else if ($(".admin-panel").length > 0) {
        var isOne = isOnlyOneUser(allUserIds);
        AddRemoveAdminMoveButton(isOne);
    }
}

function isOnlyOneUser(arr) {
    var len = arr.length;
    var firstEl = arr[0];
    var c = 0;

    for (i = 0; i < len; i++) {
        if (arr[i] == firstEl)
            c++;
    }

    if (c == len)
        return true;
    else
        return false;
}

function AddAdminPanel() {
    var adminPanel = $("<div>", { 'class': "admin-panel" });

    var adminDeleteButton = $("<button>", { 'class': "admin-panel-button theme-item__button", text: "Модерировать выбранные" });
    adminPanel.append(adminDeleteButton);

    $("#body").append(adminPanel);

    adminDeleteButton.on("click", function () {
        //if (confirm('Вы действительно хотите удалить выбранные элементы?'))
            moderateDeletePhoto();
    });

    var adminMoveButton = $("<button>", { 'class': "admin-panel-button theme-item__button", "data-admin-move-button": "", text: "Перенести выбранные" });
    adminPanel.append(adminMoveButton);

    adminMoveButton.on("click", function () {
        moderateInterestItemFolder();
    });
}

function DelAdminPanel() {
    $("#body").find(".admin-panel").remove();
}

function AddRemoveAdminMoveButton(isMove) {
    var adminPanel = $("#body").find(".admin-panel");

    if (isMove == true && ($("[data-admin-move-button]").length == 0)) {
        var adminMoveButton = $("<button>", { 'class': "admin-panel-button theme-item__button", "data-admin-move-button": "", text: "Перенести выбранные" });
        adminPanel.append(adminMoveButton);

        adminMoveButton.on("click", function () {
            moderateInterestItemFolder();
        });
    }
    else if (isMove == false) {
        $("#body").find("[data-admin-move-button]").remove();
    }
}


function moderateDeletePhoto() {
    var itemsIdValue = "";

    $("[data-selectImgcheckbox]").each(function (index, value) {
        var el = $(value);
        if (el.is(':checked'))
            itemsIdValue += el.attr('data-selectImgcheckbox') + ";";
    });

    kinkyApp.modal.showByUrl('/ctrl/admin/ModerateInterestItems?itemsIdValue=' + itemsIdValue);

    //$.ajax({
    //    url: "/ctrl/Admin/DeleteInterestItems",
    //    type: 'POST',
    //    data: {
    //        value: str
    //    },
    //    success: function (data) {
    //        var arr = data.data;
    //        for (var i = 0; i < arr.length; i++) {
    //            $("[data-id='" + arr[i] + "']").remove();
    //        }
    //    }
    //});
}

function moderateInterestItemFolder() {
    var interestItemIds = "";
    var userId = "";

    $("[data-selectImgcheckbox]").each(function (index, value) {
        var el = $(value);
        if (el.is(':checked')) {
            var itemIdValue = el.attr('data-selectImgcheckbox').split('-');
            userId = itemIdValue[0];
            interestItemIds += itemIdValue[1] + ";";
        }
    });

    kinkyApp.modal.showByUrl('/ctrl/admin/ModerateInterestItemFolder?userIdStr=' + userId + '&interestItemIdsStr=' + interestItemIds);
}

