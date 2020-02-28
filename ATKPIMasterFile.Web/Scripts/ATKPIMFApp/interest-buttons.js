atkpimfApp.ChangeSubscribeToInterest = function (element, userId, interestId) {
    var clickedElement = $(element);
    var spinner = $("<span>").addClass("icon button-loading-icon");
    clickedElement.children("[data-subspan]").hide();
    clickedElement.append(spinner);
    clickedElement.prop("disabled", true);
    var url;
    if (clickedElement.attr('data-subscribeInterest') == 'true') {
        url = atkpimfApp.data.urlTmpl.InterestUnsubscribe;
    } else {
        url = atkpimfApp.data.urlTmpl.InterestSubscribe;
    }

    atkpimfApp.asyncRequest({
        url: url,
        type: "POST",
        data: {
            userId: userId,
            interestId: interestId
        },
        complete: function () {
            spinner.remove();
            clickedElement.children("[data-subspan]").show();
            clickedElement.prop("disabled", false);
        },
        success: function (result) {
            var data = result.CustomData;

            if (interestId == 100) {
                $('[data-sublb' + userId + ']').each(function () {

                    var isSub = false;
                    for (var i = 0; i < this.attributes.length; i++)
                        if (this.attributes[i].name.indexOf("data-sublb") > -1) {
                            isSub = $('[data-subbt' + this.attributes[i].name.replace("data-sublb", "") + '=' + this.attributes[i].value + ']').attr("data-subscribeInterest") == "true";
                            break;
                        }

                    var label = $(this);
                    var count = parseInt(label.text());
                    if (isSub != data.subscribe)
                        label.text(data.subscribe ? (count + 1) : (count == 0 ? 0 : (count - 1)));
                });
            } else
                $('[data-sublb' + userId + '=' + interestId + ']').text(data.subsciptions);


            var button = interestId == 100 ? $('[data-subbt' + userId + ']') : $('[data-subbt' + userId + '=' + interestId + ']');
            var isFirstSubscription = false;
            
            if (data.subscribe) {
                button.attr('data-subscribeInterest', 'true');
                button.children("[data-subspan=icon]").removeClass("shared__icon-plus-medium").addClass("shared__icon-minus-medium");
                button.children("[data-subspan=text]").text(atkpimfApp.data.localization["Unfollow"]);
                button.children("[data-subspan=textuser]").text(atkpimfApp.data.localization["UnfollowUser"]);
                if (button.length == 1) { //чекбокс у первой подписки
                    if ($("#ch" + userId + '-' + interestId).addClass("selected").length == 1) {
                        isFirstSubscription = true;
                        button.removeClass("button-gold-to-gold").addClass("button-light-grey");
                    }
                }

                if (isFirstSubscription) {
                    atkpimfApp.analytics.addEventToQueue(atkpimfApp.analytics.eventType.interestSubscription);
                }
                else {
                    atkpimfApp.analytics.trackEvent(atkpimfApp.analytics.eventType.interestSubscription);
                }
            }
            else {
                button.attr('data-subscribeInterest', 'false');
                button.children("[data-subspan=icon]").removeClass("shared__icon-minus-medium").addClass("shared__icon-plus-medium");
                button.children("[data-subspan=text]").text(atkpimfApp.data.localization["Follow"]);
                button.children("[data-subspan=textuser]").text(atkpimfApp.data.localization["FollowUser"]);
                if (button.length == 1) { //чекбокс у первой подписки
                    if ($("#ch" + userId + '-' + interestId).removeClass("selected").length == 1)
                        button.removeClass("button-light-grey").addClass("button-gold-to-gold");
                }
            }

            if (atkpimfApp.fn.subscribeSucess)
                atkpimfApp.fn.subscribeSucess(data, $('[data-subscribeInterest=true]').length);
            clickedElement.prop("disabled", false);

        }
    });
};

atkpimfApp.ChangeLikeInterestItem = function (element, userId, itemId) {
    if (atkpimfApp.viewModel.MyUser().IsAffiliated && atkpimfApp.viewModel.MyUser().Sex == 1 && !atkpimfApp.viewModel.MyUser().IsPremium) {
        atkpimfApp.modal.showTopByUrl(atkpimfApp.data.urlTmpl.BuyClubCard, null, false, true);

        return;
    }
    
    if ($(element).prop("disabled"))
        return;
    $(element).prop("disabled", true);

    //var buttonFeedBig = $('.feed-item-big__action-button[data-likeItemButton=true][data-likeItemId=' + itemId + ']');

    var url;
    if ($(element).attr('data-likeItem') == 'true') {
        url = atkpimfApp.data.urlTmpl.InterestItemUnlikeInterestItem;
        //buttonFeedBig.attr('data-likeItem', 'false').removeClass('selected');
    } else {
        url = atkpimfApp.data.urlTmpl.InterestItemLikeInterestItem;
        //buttonFeedBig.attr('data-likeItem', 'true').addClass('selected');
    }

    atkpimfApp.asyncRequest({
        url: url,
        type: "POST",
        data: {
            userId: userId,
            itemId: itemId
        },
        success: function (result) {

            var data = result.CustomData;
            var buttonFeed = $('.shared__button-feed[data-likeItemButton=true][data-likeItemId=' + itemId + ']');
            var buttonFullview = $('.button-like-fullview[data-likeItemButton=true][data-likeItemId=' + itemId + ']');

            buttonFeed.prop("disabled", false).empty();
            buttonFullview.prop("disabled", false).empty();
            //buttonFeedBig.prop("disabled", false).empty();

            if (data.like) {
                buttonFullview.attr('data-likeItem', 'true');
                buttonFeed.attr('data-likeItem', 'true');
                //buttonFeedBig.attr('data-likeItem', 'true').addClass('selected');

                buttonFullview.html(atkpimfApp.data.localization.Dislike);

                if (buttonFeed.hasClass('right-button-small'))
                    buttonFeed.append($("<span>").addClass("shared__icon shared__icon-dislike-small"));
                else
                    buttonFeed.html(atkpimfApp.data.localization.Dislike);

                atkpimfApp.analytics.trackEvent(atkpimfApp.analytics.eventType.like);
            } else {
                //right-button-small

                buttonFullview.attr('data-likeItem', 'false');
                buttonFeed.attr('data-likeItem', 'false');
                //buttonFeedBig.attr('data-likeItem', 'false').removeClass('selected');

                buttonFullview.append($("<span>").addClass("icon shared__icon-like-medium"))
                    .append($("<span>").css("margin-left", "5px").text(atkpimfApp.data.localization.Like));
                buttonFeed.append($("<span>").addClass("shared__icon shared__icon-like-small"));
                if (!buttonFeed.hasClass('right-button-small'))
                    buttonFeed.append($("<span>").css("margin-left", "3px").text(atkpimfApp.data.localization.Like));
            }

            $('[data-likeItemDisplay=true][data-likeItemId=' + itemId + ']').html(data.likes);
        }
    });
};

atkpimfApp.ReblogPhotoSuccessUpdate = function (data) {
    $('[data-reblogDisplay=true][data-reblogInterestItemId=' + data.interestItemId + ']').html(data.reblogCount);
    atkpimfApp.analytics.trackEvent(atkpimfApp.analytics.eventType.reblog)/*mixTrack('Reblog', 'Reblog')*/;
};

atkpimfApp.fn.createRequest = function (select, text) {
    atkpimfApp.asyncRequest({
        url: atkpimfApp.data.urlTmpl.InterestAdd,
        type: "POST",
        data: { "name": text },
        success: function (result) {
            if (result.Status == atkpimfApp.statusCodes.ok) {
                if (document.getElementById("repost-submit-button")) {
                    $("#repost-submit-button").removeAttr("disabled");
                }
                if (document.getElementById("add-text-submit-button")) {
                    $("#add-text-submit-button").removeAttr("disabled");
                }
                if (document.getElementById("savePhotoButton")) {
                    $("#savePhotoButton").removeAttr("disabled");
                }
                $.fn.successVisual(select, result.CustomData.Name, result.CustomData.InterestId, result.CustomData.IsPersonal ? "1" : "");
            }
            else {
                atkpimfApp.popUp.hide();
                alert(result.ErrorMessage);
                //atkpimfApp.modal.show(result.ErorrMessage);
            }
        }
    });
};

atkpimfApp.RateItem = function (userId, itemId, rate) {
    atkpimfApp.asyncRequest({
        url: atkpimfApp.data.urlTmpl.InterestItemRate,
        type: "POST",
        data: {
            userId: userId,
            itemId: itemId,
            rate: rate
        }
    });
};

