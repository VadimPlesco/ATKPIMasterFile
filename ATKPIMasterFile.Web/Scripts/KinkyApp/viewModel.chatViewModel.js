kinkyApp.ChatViewModel = function (parentViewModel, user) {
    user.chat = this;
    var self = this;

    this.isPromoVideo = ko.observable(false);

    this.currentBroadcastSessionId = '';
    this.initialLink = '';
    this.isPromoStub = ko.observable(false);
    this.promoVideoLink = ko.observable('');
    this.promoVideoLink.subscribe(function (newValue) {
        if (newValue != self.initialLink && newValue != '') {
            

            self.initialLink = newValue;
        }
    });
    var checkPromoVideo = function (buddyUser) {
        //debugger;
        if (buddyUser.PromoVideoLink && buddyUser.PromoVideoLink != self.promoVideoLink() && kinkyApp.viewModel.MyUser().IsAffiliated && !kinkyApp.viewModel.MyUser().IsPremium) {
            
            var seenLink = localStorage.getItem('promo-video-' + buddyUser.UserId, buddyUser.PromoVideoLink);
            console.log('check promo link', seenLink, buddyUser);
            
            if (seenLink != null) {
                self.isPromoVideo(false);
                self.isPromoStub(true);
            }
            else {
//                $.ajax({
//                    url: buddyUser.PromoVideoLink + '.mp4',
//                    type: 'HEAD',
//                    async: false,
//                    error: function () {
//                        console.log('there is no such file: ' + buddyUser.PromoVideoLink + '.mp4');
//                    },
//                    success: function () {
//                        console.log('file found: ' + buddyUser.PromoVideoLink + '.mp4');
                        self.isPromoVideo(true);
                        self.isPromoStub(false);
                        self.promoVideoLink(buddyUser.PromoVideoLink);
//                    }
//                });
            }
        }

        if (buddyUser.PromoStub && kinkyApp.viewModel.MyUser().IsAffiliated && !kinkyApp.viewModel.MyUser().IsPremium) {
            self.isPromoVideo(false);
            self.isPromoStub(true);
        }
    };

    
    
    this.ChatMessages = ko.observableArray();

    var buddyUserUpdateTime = new Date();

    this.showModelIsBusy = ko.observable(false);

    this.BuddyUser = ko.observable(user);
    checkPromoVideo(user);
    
    this.BuddyUser.subscribe(function (newValue) {

        buddyUserUpdateTime = new Date();

        if (self.Active()) {
            console.log('$-------------------------------------------------------------$');
            console.log(self.currentBroadcastSessionId);
            console.log(self);
            console.log('$-------------------------------------------------------------$');
            
            if (newValue.IsBroadcastingVideo) {
                if (self.currentBroadcastSessionId != '') {
                    self.watchVideoModel.connect(self.currentBroadcastSessionId/* || self.BuddyUser().BroadcastSessionId*/, '$');
                }
            }
            else {
                self.initBuddyUsersVideo();
            }
        }

        if (self.Active() && !newValue.IsBroadcastingVideo) {
            self.watchVideoModel.disconnect();
        }

        if (!self.privateVideoMode()) {
            self.showModelIsBusy(newValue.IsInPrivateChat);
        }
        //debugger;
    });
    
    this.Text = ko.observable("");

    this.Title = ko.observable("---");

    //this.watchVideoModal = new kinkyApp.WatchVideoModal(user.UserId);
    this.watchVideoModel = new kinkyApp.WatchVideoModel(this, user.UserId);

    this.bringToFront = function() {
        zIndexTop($("#chat" + self.BuddyUser().UserId));
    };

    this.Active = ko.observable(false);

    this.Active.subscribe(function (active) {

        if (active) {
            self.micIsOn(true);
            zIndexTop($("#chat" + self.BuddyUser().UserId));
            //debugger;
//            debugger;
            //self.sexyChatButtonText($('#sexy-chat-button-' + self.BuddyUser().UserId).data('stopped-text'));
            
            self.toggleSexyChat(parentViewModel.MyUser().SexyChatEnabled);

            var contact = ko.utils.arrayFirst(kinkyApp.viewModel.Contacts(), function (item) {
                return item.BuddyUserId == self.BuddyUser().UserId;
            });

            if (contact) {
                self.sexyChatStatus(contact.SexyChatState);

                if (contact.SexyChatState == null || contact.SexyChatState == 0) {
                    self.sexyChatButtonText($('#sexy-chat-button-' + self.BuddyUser().UserId).data('stopped-text'));
                }
            }
            else {
                self.sexyChatButtonText($('#sexy-chat-button-' + self.BuddyUser().UserId).data('stopped-text'));
            }

            if (self.sexyChatStatus() == 2) {
                self.showSexyChatIncomeRequest(true);
            }

            //if (self.BuddyUser().VideoChatStarted) {
            //    self.buddyUsersVideoSessionId(self.BuddyUser().VideoChatSessionId);
            //    self.buddyUsersSignature(self.BuddyUser().VideoChatSignature);
            //}

            self.initBuddyUsersVideo();
            self.privateVideoChatButtonText($('#private-video-chat-button-' + self.BuddyUser().UserId).data('stopped-text'));
            
            if (!self.privateVideoMode() && self.BuddyUser().IsInPrivateChat) {
                self.showModelIsBusy(self.BuddyUser().IsInPrivateChat);
            }

            if (self.isPromoVideo()) {
                $('#promo-video-' + self.BuddyUser().UserId)
                    .on('play', function () {
                        //                        debugger;
                        console.log('play');
                        //sendToServer(function(serverHub) {
                        //    serverHub.promoVideoWatched();
                        //});

                        localStorage.setItem('promo-video-' + self.BuddyUser().UserId, self.promoVideoLink());
                    })
                    .on('ended pause', function () {
                        //                        debugger;
                        console.log('ended');
                        self.isPromoVideo(false);
                        self.isPromoStub(true);
                    });
                setTimeout(function() {
                    $('#promo-video-' + self.BuddyUser().UserId)[0].play();
                }, 500);
            }
        }

        if (active && self.BuddyUser().IsBroadcastingVideo) {
            console.log('#-------------------------------------------------------------#');
            console.log(self.currentBroadcastSessionId);
            console.log(self);
            console.log('#-------------------------------------------------------------#');
            if (self.currentBroadcastSessionId != '') {
                self.watchVideoModel.connect(self.currentBroadcastSessionId/* || self.BuddyUser().BroadcastSessionId*/, '#');
            }
        }

        if (!active) {
//            debugger;
            self.watchVideoModel.disconnect();
            self.micIsOn(false);
            self.closeVideoChat(false);
            self.privateVideoMode(false);
            
            if (self.isPromoVideo()) {
                $('#promo-video-' + self.BuddyUser().UserId)[0].pause();
                self.isPromoStub(true);
            }
        }
    });


    this.saveSettings = function () {
        self.showSettingsPanel(false);
        
        sendToServer(function (serverHub) {
            serverHub.enableSexyChat(self.toggleSexyChat()).done(function (success) {
            });
        });
    };

    this.chatWindow = null;

    this.init = function () {

        var offset = parentViewModel.Chats.indexOf(self) * 5;

        var chatDiv = $("#chat" + self.BuddyUser().UserId);
        chatDiv.css("left", parseInt(chatDiv.css("left")) + offset);
        chatDiv.css("top", parseInt(chatDiv.css("top")) + offset);

        chatDiv.draggable({ "handle": ".chat-modal__chat-header", "containment": "document" });
        chatDiv.mousedown(function () {
            zIndexTop(this);
            if ((Date.now() - buddyUserUpdateTime) > 10000)
                parentViewModel.getUser(self.BuddyUser().UserId, null, true);
        });

        chatDiv
            .on('mouseenter', function () {
                $('body').on('mousewheel', kinkyApp.preventBodyScrolling);
            })
            .on('mouseleave', function () {
                $('body').off('mousewheel', kinkyApp.preventBodyScrolling);
            });

        self.chatWindow = chatDiv;

        var cm = $("#chat" + self.BuddyUser().UserId + " .chat-modal__chat-messages");
        var cp = $("#chat_preloader" + self.BuddyUser().UserId);
        var chint = $("#chat" + self.BuddyUser().UserId + " .chat-modal__chat-header .chat-header__user-info .chat-header__user-premium-v2");

        self.ct = $("#chat" + self.BuddyUser().UserId + " textarea.chat-v3-message-container__message-box");

        $("#chat" + self.BuddyUser().UserId + " li.chat-v3-smile-container__smile-item").on("click", function () {
            
            self.ct.insertAtCaret("*" + $(this).attr("data-text") + "*");
        });

        //$(chint).tooltip();

        setInterval(function () {
            $(chint).toggleClass('chat-header__user-premium-v2-span-hover');

        }, 5000);

        

        var sb = cm.mCustomScrollbar({
            scrollInertia: 0,
            scrollButtons: { enable: true }, callbacks: {
                whileScrollingInterval: 400,
                whileScrolling: function () {

                    console.log(self.lastMessageId);

                    if (mcs.top > -80 && self.lastMessageId) {

                        cp.removeClass("hidden");

                        

                        var t = "#chat_msg-" + self.lastMessageId;
                        var lastMsgId = self.lastMessageId;

                        self.lastMessageId = null;

                        if (lastMsgId)
                        sendToServer(function (serverHub) {
                            serverHub.getMoreChatEvents(self.BuddyUser().UserId, lastMsgId).done(function (result) {
                                if (result.length > 0) {
                                    self.putChatMessages(result, true);
                                    var tt = $(t).position().top;                             
                                    var container = sb.find(".mCSB_container");
                                    var dragger = sb.find(".mCSB_dragger");
                                    var draggerContainer = sb.find(".mCSB_draggerContainer");

                                    container.css("top", 0 - tt + parseInt(container.css("padding-top")));
                                    dragger.css("top", parseInt(tt * draggerContainer.height() / container.height()));
                                    cm.mCustomScrollbar("update");
                                }
                                cp.addClass("hidden");
                                self.lastMessageId = result.length == 30 ? result[0].Id : null;
                            });
                        });
                    }
                }
            }
        });

        if ($.fn.initChatInputMessage)
            $.fn.initChatInputMessage(self.BuddyUser().UserId);

        sendToServer(function (serverHub) {
            serverHub.getInitChatEvents(self.BuddyUser().UserId).done(function (result) {
                self.putChatMessages(result);
                self.lastMessageId = result.length == 30 ? result[0].Id : null;
                self.tryMarkAsRead();
            });
        });
    };

    var zIndexTop = function (z) {

        //console.log(z);
        //$(".chat-modal").css({ "z-index": 0 });
        $(".chat-v2-modal").css({ "z-index": 1 });
        $(".translation-notify-modal").css({ "z-index": 1 });
        $(z).css({ "z-index": 100 });
    };

    this.close = function (skipPrivateCheck) {
        //debugger;
        if (self.privateVideoMode() && !skipPrivateCheck) {
//            debugger;
            kinkyApp.modal.showTopByUrl(kinkyApp.data.urlTmpl.ModalBlockerConfirmCloseVideoChat + '?userId=' + self.BuddyUser().UserId + '&close=true');

            return;
        }
        
        self.watchVideoModel.disconnect();
        self.ShowUploadPhotoPanel(false);
        self.showSexyChatIncomeRequest(false);
        self.Active(false);
    };

    this.updateChatMessages = function (msgs) {
        try {
            for (var i = 0; i < msgs.length; i++) {
                var msg = ko.utils.arrayFirst(self.ChatMessages(), function (item) {
                    return msgs[i].Id == item.Id;
                });
                if (msg)
                    kinkyApp.mapAsObservable(msg, "ReadTS", self.formatDate(msgs[i].ReadTS, kinkyApp.data.localization["Not_read"], kinkyApp.data.localization["Read_in"] + " "));
            }
        }
        catch (ex) {
            alert("updateChatMessages error:" + ex);
        }
    };

    this.tryMarkAsRead = function () {
        try {
            if (self.Active()) {
                var result = self.ChatMessages();
                if (result.length > 0) {
                    var buddyUserId = self.BuddyUser().UserId;
                    var findContact = ko.utils.arrayFirst(parentViewModel.Contacts(), function (item) {
                        return buddyUserId === item.BuddyUserId;
                    });

                    var last = result[result.length - 1];

                    try {
                        if (self.Active()) {
                            $("#chat" + self.BuddyUser().UserId + " .chat-modal__chat-messages").mCustomScrollbar("update");
                            $("#chat" + self.BuddyUser().UserId + " .chat-modal__chat-messages").mCustomScrollbar("scrollTo", "bottom");
                        }
                    }
                    catch (ex) {
                        alert("mCustomScrollbar error:" + ex);
                    }

                    if (findContact != null && findContact.UnreadCount > 0) {

                        sendToServer(function (serverHub) {
                            serverHub.markAsRead(buddyUserId, last.Id).done(function (contact) {
                                parentViewModel.putContacts(contact);
                            });
                        });
                    }
                }
            }
        }
        catch (ex) {
            alert("tryMarkAsRead error:" + ex);
        }
    };

    this.parMsg = null;

    var canMessagesGroup = function (parentMsg, curMsg) {
        return (parentMsg != null
             && parentMsg.Type == 1
             && parentMsg.IsMy == curMsg.IsMy
             && curMsg.Type == 1
             && parentMsg.CreateTS.substring(0, 16) == curMsg.CreateTS.substring(0, 16)
             && parentMsg.Id != curMsg.Id
            );
    };

    //this.lastMessageBlockId = null;

    this.putChatMessages = function (messages, isHistory) {
        try {
            if (canMessagesGroup(self.parMsg, messages[0])) {
                messages[0].Text = kinkyApp.smileLinkFormatter.formatMessage(messages[0].Text);
                self.parMsg.Texts.push(messages[0].Text);
                return;
            }
            for (var i = 0; i < messages.length; i++) {
                self.ChatMessages.remove(function (item) { return item.Id == messages[i].Id; });
                kinkyApp.mapAsObservable(messages[i], "ReadTS", self.formatDate(messages[i].ReadTS, kinkyApp.data.localization["Not_read"], kinkyApp.data.localization["Read_in"] + " "));

                if (messages[i].Texts == null)
                    messages[i].Texts = ko.observableArray([]);

                messages[i].Text = kinkyApp.smileLinkFormatter.formatMessage(messages[i].Text);
                if (canMessagesGroup(self.parMsg, messages[i]))
                    self.parMsg.Texts.push(messages[i].Text);
                else {

                    self.ChatMessages.push(messages[i]);
                    self.parMsg = messages[i];
                }

            }
            self.ChatMessages.sort(function (left, right) { return left.Id.localeCompare(right.Id) * -1; });

            self.BuddyUserTyping(false);
        }
        catch (ex) {
            alert("putChatMessages error:" + ex);
        }
    };

    this.sendMessageByEnter = function (obj, event) {
        self.BeginTyping();

        var keyCode = (event.which ? event.which : event.keyCode);

        if (!event.ctrlKey && keyCode === 13) {
            self.sendMessage();
            return false;
        } else
            if (event.ctrlKey && keyCode === 13) {
                self.Text(self.Text() + '\n');
            }
        return true;
    };


    //-----------------------------------------------------------------Событие ввода теста
    //Отправка события ввода текса
    this.isMessageTyping = false;
    this.BeginTyping = function () {
        if (self.isMessageTyping == false) {

            sendToServer(function (serverHub) {
                serverHub.typeChatMessage(self.BuddyUser().UserId);
                self.isMessageTyping = true;
                setTimeout(function () {
                    self.isMessageTyping = false;
                }, 3000);
            });
        }
    };

    //получение события ввода теста
    this.IsBuddyUserTyping = ko.observable(false);

    this.isBuddyUserTypingTimer = null;
    this.BuddyUserTyping = function (val) {
        self.IsBuddyUserTyping(val);

        if (val) {
            if (self.isBuddyUserTypingTimer)
                window.clearTimeout(self.isBuddyUserTypingTimer);

            self.isBuddyUserTypingTimer = setTimeout(function () {
                self.isBuddyUserTypingTimer = null;
                self.IsBuddyUserTyping(false);
            }, 5000);
        }
    };
    //----------------------------------------------------------------------------------------
    //this.totalSec = 0;
    
    this.privateVideoChatTimerCallback = function () {
        if (self.showPrivateVideoChatInfoPanel() && self.privateVideoChatStartTime >= 0) {
            var totalSec = Math.floor((Date.now() - self.privateVideoChatStartTime) / 1000) - 1;
            if (totalSec < 0)
                return;
            var hours = Math.floor(totalSec / 3600);
            var minutes = Math.floor(totalSec % 3600 / 60);
            var seconds = Math.floor(totalSec % 3600 % 60);

            var result = (hours < 10 ? "0" + hours : hours) + ":" + (minutes < 10 ? "0" + minutes : minutes) + ":" + (seconds < 10 ? "0" + seconds : seconds);

            self.privateVideoChatTimerValue(result);

            var reward = (totalSec * 0.038).toFixed(2);
            var clientsCash = (self.clientsCashInitial() - (totalSec * 0.038)).toFixed(2);

            if (clientsCash < 0) {
                self.privateVideoChatClose();
                clientsCash = 0;
            }

            self.privateVideoChatReward({ integerPart: reward.split('.')[0], fractionPart: reward.split('.')[1] });
            self.clientsCash({ integerPart: clientsCash.split('.')[0], fractionPart: clientsCash.split('.')[1] });
        }
    };

    this.selectGift = function () {
        if (kinkyApp.viewModel.MyUser().IsAffiliated && kinkyApp.viewModel.MyUser().Sex == 1 && !kinkyApp.viewModel.MyUser().IsPremium) {
            kinkyApp.modal.showTopByUrl(kinkyApp.data.urlTmpl.BuyClubCard, null, false, true);

            return;
        }
        
        kinkyApp.modal.showByUrl(kinkyApp.data.urlTmpl.SelectGift + '?buddyUserId=' + self.BuddyUser().UserId);
    };

    this.toggleSexyChat = ko.observable(false);
    this.ShowBanConfirmationPanel = ko.observable(false);
    
    this.openBanPanel = function () {
        if (kinkyApp.viewModel.MyUser().IsAffiliated && kinkyApp.viewModel.MyUser().Sex == 1 && !kinkyApp.viewModel.MyUser().IsPremium) {
            kinkyApp.modal.showTopByUrl(kinkyApp.data.urlTmpl.BuyClubCard, null, false, true);

            return;
        }
        
        self.ShowBanConfirmationPanel(true);
    };

    this.ShowUnBanConfirmationPanel = ko.observable(false);

    this.ShowBanWaitPanel = ko.observable(false);

    this.showSettingsPanel = ko.observable(false);

    this.showDeleteCorrespondencePanel = ko.observable(false);

    this.showSexyChatIncomeRequest = ko.observable(false);

    this.privateVideoChatReward = ko.observable({integerPart: 0, fractionPart: '00'});

    this.showPrivateVideoChatInfoPanel = ko.observable(false);

    this.showPrivateVideoChatInfoPanel.subscribe(function (newValue) {
        if (newValue) {
            self.timer = window.setInterval(self.privateVideoChatTimerCallback, 300);
        }
        else {
            self.privateVideoChatStartTime = -1;
            self.privateVideoChatTimerValue("00:00:00");
            self.privateVideoChatReward({ integerPart: 0, fractionPart: 0 });
            window.clearInterval(self.timer);
        }
    });

    this.privateVideoChatTimerValue = ko.observable("00:00:00");

    this.sexyChatButtonText = ko.observable("");

    this.sexyChatState = ko.observable(0);

    this.sexyChatStatus = ko.observable(0);

    this.sexyChatStatus.subscribe(function (newValue) {
        self.showSexyChatInfoPanel(newValue == 4);
        self.showSexyChatIncomeRequest(newValue == 2);

        if (newValue == 3 || newValue == 4) {
            self.sexyChatButtonText($('#sexy-chat-button-' + self.BuddyUser().UserId).data('chatting-text'));
            self.sexyChatState(2);
        }

        if (newValue == 0) {
            self.sexyChatButtonText($('#sexy-chat-button-' + self.BuddyUser().UserId).data('stopped-text'));
            self.sexyChatState(0);
        }
    });

    this.tryShowDeleteCorrespondencePanel = function () {
        if (kinkyApp.viewModel.MyUser().IsAffiliated && kinkyApp.viewModel.MyUser().Sex == 1 && !kinkyApp.viewModel.MyUser().IsPremium) {
            kinkyApp.modal.showTopByUrl(kinkyApp.data.urlTmpl.BuyClubCard, null, false, true);

            return;
        }
        
        sendToServer(function (serverHub) {
            serverHub.checkUserIsPremium().done(function (isPremium) {
                if (!isPremium) {
                    kinkyApp.modal.showTopByUrl(kinkyApp.data.urlTmpl.DeleteMessagesPremiumBlocker);
                }
                else {
                    self.showDeleteCorrespondencePanel(true);
                }
            });
        });
    };

    this.sexyChatClickHandle = function () {
        if (kinkyApp.viewModel.MyUser().IsAffiliated && kinkyApp.viewModel.MyUser().Sex == 1 && !kinkyApp.viewModel.MyUser().IsPremium) {
            kinkyApp.modal.showTopByUrl(kinkyApp.data.urlTmpl.BuyClubCard, null, false, true);

            return;
        }

        if (self.sexyChatState() == 0) {
            self.sendSexyChatStartRequest();
        }
        else {
            self.sendSexyChatEndRequest();
        }
    };

    this.sendSexyChatStartRequest = function () {
        self.sentStopSexyChatRequest = false;

        sendToServer(function (serverHub) {
            serverHub.requestSexyChat(self.BuddyUser().UserId)
                .done(function (success) {
                    if (!success) {
                        self.sexyChatButtonText($('#sexy-chat-button-' + self.BuddyUser().UserId).data('stopped-text'));
                        self.sexyChatState(0);
                    }
                    else {
                        self.sexyChatButtonText($('#sexy-chat-button-' + self.BuddyUser().UserId).data('waiting-text'));
                        self.sexyChatState(1);
                    }
                });
        });
    };

    this.sentStopSexyChatRequest = false;
    this.sentAcceptSexyChatRequest = false;
    
    this.sendSexyChatEndRequest = function () {
        if (self.sentStopSexyChatRequest || self.sexyChatState() == 0) {
            return;
        }

        self.sentStopSexyChatRequest = true;
        sendToServer(function (serverHub) {
            serverHub.endSexyChat(self.BuddyUser().UserId)
                .done(function () {
                    self.sexyChatButtonText($('#sexy-chat-button-' + self.BuddyUser().UserId).data('stopped-text'));
                    self.sexyChatState(0);
                    self.sentStopSexyChatRequest = false;
                });
        });
    };

    this.acceptSexyChatRequest = function () {

        if (self.sentAcceptSexyChatRequest) return;

        self.sentAcceptSexyChatRequest = true;
        
        sendToServer(function (serverHub) {
            serverHub.confirmSexyChat(self.BuddyUser().UserId)
                .done(function (success) {
                    if (!success) return;
                    
                    self.sexyChatButtonText($('#sexy-chat-button-' + self.BuddyUser().UserId).data('chatting-text'));
                    self.sexyChatState(2);
                    self.sexyChatReward(0);
                    self.sexyChatStartTime = Date.now();
                    self.sentAcceptSexyChatRequest = false;
                });
        });
    };

    this.rejectSexyChatRequest = function () {
        sendToServer(function (serverHub) {
            serverHub.rejectSexyChat(self.BuddyUser().UserId)
                .done(function () {
                    self.sexyChatButtonText($('#sexy-chat-button-' + self.BuddyUser().UserId).data('stopped-text'));
                    self.sexyChatState(0);
                });
        });
    };

    this.Ban = function () {
        self.sendSexyChatEndRequest();
        self.ShowBanConfirmationPanel(false);
        self.ShowBanWaitPanel(true);
        sendToServer(function (serverHub) {
            serverHub.ban(self.BuddyUser().UserId, true).done(function (usr) {
                parentViewModel.putUser(usr);
                self.ShowBanWaitPanel(false);
            });
        });
    };

    this.UnBan = function () {
        self.ShowUnBanConfirmationPanel(false);
        self.ShowBanWaitPanel(true);
        sendToServer(function (serverHub) {
            serverHub.ban(self.BuddyUser().UserId, false).done(function (user) {
                parentViewModel.putUser(user);
                self.ShowBanWaitPanel(false);
            });
        });
    };

    this.deleteAllCorrespondence = function () {
        sendToServer(function (serverHub) {
            serverHub.deleteAllCorrespondence(self.BuddyUser().UserId);
        });
    };

    this.sendMessage = function () {
        if (kinkyApp.viewModel.MyUser().IsAffiliated && kinkyApp.viewModel.MyUser().Sex == 1 && !kinkyApp.viewModel.MyUser().IsPremium) {
            kinkyApp.modal.showTopByUrl(kinkyApp.data.urlTmpl.BuyClubCard, null, false, true);

            return;
        }

        self.Text($(self.ct).val());

        if (parentViewModel.checkUser()) {
            try {
                if (self.ShowUploadPhotoPanel()) {

                    $.ajax({
                        url: kinkyApp.data.urlTmpl.SendChatPhoto,
                        type: "POST",
                        data: {
                            UserId: parentViewModel.MyUser().UserId,
                            BuddyUserId: self.BuddyUser().UserId,
                            Diamonds: self.PhotoIsPrivate() ? self.PhotoPrice() : 0,
                            Value: self.SyncKey(),
                            Type: 4
                        },
                        xhrFields: {
                            withCredentials: true
                        }
                    }).done(function (result) {
                        if (result.messagesLimit) {
                            kinkyApp.modal.showTopByUrl(kinkyApp.data.urlTmpl.UnlimitedMessagesPremiumBlocker + "?time=");
                        }
                        else if (result.timeLeft) {
                            kinkyApp.modal.showTopByUrl(kinkyApp.data.urlTmpl.UnlimitedMessagesPremiumBlocker + "?time=" + result.timeLeft);
                        }
                    });

                    kinkyApp.analytics.trackEvent(kinkyApp.analytics.eventType.chatImageSendPc);

                    self.ShowUploadPhotoPanel(false);
                    self.PhotoId = 0;
                    return;
                }
                var temptext = self.Text()
                    .replace(/<\/?[^>]+>/gi, ' ')
                    //.replace(/(&nbsp;)*/g, "")
                    .trim();

                if (!temptext || temptext == '') {
                    //self.Text('<br/>');
                    return;
                }

                sendToServer(function (serverHub) {
                    serverHub.sendChatMessage({
                        UserId: parentViewModel.MyUser().UserId,
                        BuddyUserId: self.BuddyUser().UserId,
                        Text: temptext,
                        Type: 1
                    }).done(function (result) {
//                        debugger;
                        if (result.showPremiumBlocker) {
//                            debugger;
                            if (result.messagesLimit) {
                                kinkyApp.modal.showTopByUrl(kinkyApp.data.urlTmpl.UnlimitedMessagesPremiumBlocker + "?time=");
                            }
                            else if (result.timeLeft) {
                                kinkyApp.modal.showTopByUrl(kinkyApp.data.urlTmpl.UnlimitedMessagesPremiumBlocker + "?time=" + result.timeLeft);
                            }
                        }
                        else if (result.showClubCardBlocker) {
//                            debugger;
                            if (result.contactsLimit) {
                                kinkyApp.modal.showTopByUrl(kinkyApp.data.urlTmpl.ModalBlockerContactsLimit + "?time=" + result.timeLeft);
                            }
                        }
                    });
                });

                self.Text('');

                kinkyApp.analytics.trackEvent(kinkyApp.analytics.eventType.chatMessageSend);

            }
            catch (e) {
                alert("sendMessage error:" + e);
            }
        }
    };


    this.ShowUploadPhotoPanel = ko.observable(false);

    this.ShowSelectPhotoSendTypePanel = ko.observable(false);
    this.ShowPaymentSettings = ko.observable(false);
    this.showSettingsHint = ko.observable(false);

    this.PhotoUrl = ko.observable("");
    this.PhotoIsPrivate = ko.observable(false);
    this.PhotoPrice = ko.observable(1);
    this.SyncKey = ko.observable(0);

    this.selectPhotoSendType = function () {
        if (kinkyApp.viewModel.MyUser().IsAffiliated && kinkyApp.viewModel.MyUser().Sex == 1 && !kinkyApp.viewModel.MyUser().IsPremium) {
            kinkyApp.modal.showTopByUrl(kinkyApp.data.urlTmpl.BuyClubCard, null, false, true);

            return;
        }
        
        self.ShowSelectPhotoSendTypePanel(true);
    };

    this.paymentSettings = function () {
        self.ShowPaymentSettings(true);
    };

    this.settingsHintShow = function () {
        self.showSettingsHint(true);
    };

    this.settingsHintHide = function () {
        self.showSettingsHint(false);
    };

    this.sendPhoto = function () {
        var syncCustomData = null;
        try {
            self.SyncKey((new Date()).getTime());
            kinkyApp.UploadPhoto(kinkyApp.data.urlTmpl.InterestItemUploadPhoto + "?sk=" + self.SyncKey(),
                function (input) {
                    self.ShowBanWaitPanel(false);
                },
                null,
                function (input) {
                    self.PhotoIsPrivate(false);
                    self.PhotoPrice(1);
                    self.ShowSelectPhotoSendTypePanel(false);
                    self.ShowUploadPhotoPanel(true);
                    self.ShowBanWaitPanel(true);

                    if (input && input.files && input.files[0]) {
                        var reader = new FileReader();
                        reader.onload = function (e1) {
                            self.PhotoUrl(e1.target.result);
                        };
                        reader.readAsDataURL(input.files[0]);
                    }
                });
        }
        catch (e) {
            alert("sendMessage error:" + e);
        }
    };

    this.savePaymentSettings = function () {
        alert("Add save settings functionality");
        self.ShowPaymentSettings(false);
    };

    this.selectPhoto = function () {
        parentViewModel.sendPhotoToChatUserId = self.BuddyUser().UserId;
        kinkyApp.modal.showByUrl(kinkyApp.data.urlTmpl.InterestSelect);
        //alert('пока функционал не доступен :(');
        self.ShowSelectPhotoSendTypePanel(false);
        self.ShowUploadPhotoPanel(false);
    };

    this.navigateBack = function () {
        self.ShowUploadPhotoPanel(false);
        self.ShowSelectPhotoSendTypePanel(false);
        self.ShowPaymentSettings(false);
    };

    this.openGiftSelectionPanel = function (url) {
        return function () {
            kinkyApp.modal.showByUrl(url);
        };
    };

    this.sendGift = function (type, data) {
        try {
            sendToServer(function (serverHub) {
                serverHub.sendChatMessage({
                    UserId: parentViewModel.MyUser().UserId,
                    BuddyUserId: self.BuddyUser().UserId,
                    Type: type,
                    Value: data.Value,
                    GiftId: data.Id
                });
            });
        }
        catch (e) {
            alert("sendMoney error:" + e);
        }

    };

    this.myVideoStreamOpened = ko.observable(false);
    this.myVideoStreamOpened.subscribe(function(newValue) {
        if (parentViewModel.myVideoSessionsOpened == null) {
            parentViewModel.myVideoSessionsOpened = 0;
        }

        if (newValue) {
            parentViewModel.myVideoSessionsOpened += 1;
        }
        else {
            parentViewModel.myVideoSessionsOpened -= 1;
        }

        if (parentViewModel.myVideoSessionsOpened == 0) {
            parentViewModel.currentVideoSessionId = '';
            parentViewModel.signature = '';
        }
    });
    this.buddyUsersVideoStreamOpened = ko.observable(false);
    this.buddyUsersVideoSessionId = ko.observable('');
    this.buddyUsersSignature = ko.observable('');
    this.myVideoStreamObject = null;
    this.buddyUsersVideoStreamObject = null;
//    this.buddyUserIsInPrivateChat = ko.observable();
    
    this.myVideoStreamOpened(false);

    this.closeVideoChat = function (forcedClose) {
        //        debugger;
        if (self.privateVideoMode()) {
            self.privateVideoChatClose();
        }
        
        if (self.myVideoStreamOpened()) {
            self.closeMyVideo();
        }
        
        if (self.buddyUsersVideoStreamOpened()) {
            self.closeBuddyUsersVideo();
        }

        self.watchVideoModel.disconnect(forcedClose);
    };
    
    this.startVideoChat = function (byButton, successCallback) {
        //        debugger;
        if (self.isPromoStub()) {
            kinkyApp.modal.showByUrl('/ctrl/payment/premiumstatus');
            return;
        }
        
        var callback = successCallback || function () { };
        
        if (self.myVideoStreamOpened()) {
            if (byButton === true) {
                self.closeMyVideo();
            }
        }
        else {
            
            if (!kinkyApp.viewModel.MyUser().IsModel && !self.privateVideoMode()) {
                kinkyApp.modal.showTopByUrl(kinkyApp.data.urlTmpl.ModalBlockerVideoOnlyForPrivate);

                return;
            }
            
            if (parentViewModel.myVideoSessionsOpened > 0) {
                //debugger;
                self.initMyVideo(callback);
            }
            else {
                try {
                    kinkyApp.asyncRequest({
                        url: kinkyApp.data.urlTmpl.VideoChatStart,
                        type: "POST",
                        success: function(data) {
                            parentViewModel.currentVideoSessionId = data.sessionId;
                            parentViewModel.signature = data.signature;

                            self.initMyVideo(callback);
                        }
                    });
                }
                catch(error) {
                    console.error(error);
                }
            }
        }
    };

    this.initBuddyUsersVideo = function (forced) {
//        debugger;
        if ((!self.Active() || self.BuddyUser().Banned == 1 || self.buddyUsersVideoSessionId() == '' || self.buddyUsersVideoStreamOpened()) && !forced) {
            return;
        }

        console.log('initBuddyUsersVideo');
        console.log(self.BuddyUser());
        
        if (!self.BuddyUser().IsBroadcastingVideo && (self.BuddyUser().StreamOpenedFor == null || (self.BuddyUser().StreamOpenedFor != null && self.BuddyUser().StreamOpenedFor.indexOf(kinkyApp.viewModel.MyUser().UserId) < 0))) {
//            debugger;
            return;
        }
//        debugger;
        var videoContainerId = self.myVideoStreamOpened()
            ? 'small-video-stream-placeholder-' + parentViewModel.MyUser().UserId + '-' + self.BuddyUser().UserId
            : 'big-video-stream-placeholder-' + parentViewModel.MyUser().UserId + '-' + self.BuddyUser().UserId;
        var smallContainer = $('#small-video-stream-placeholder-' + parentViewModel.MyUser().UserId + '-' + self.BuddyUser().UserId).parent();
        var height = self.myVideoStreamOpened() ? 170 : 414; //414;
        var width = self.myVideoStreamOpened() ? 230 : 540;
        var videoId = 'video-stream-' + self.BuddyUser().UserId + '-' + parentViewModel.MyUser().UserId;
        var videoHandlerName = 'handleBuddyVideo_' + parentViewModel.MyUser().UserId + '_' + self.BuddyUser().UserId;

        kinkyApp.videoHandlers[videoHandlerName] = function(state) {
            var chat = ko.utils.arrayFirst(kinkyApp.viewModel.Chats(), function(item) {
                return item.BuddyUser().UserId == self.BuddyUser().UserId;
            });

            if (state == 10) {
                chat.buddyVideoInitialized = true;
                $(chat.buddyUsersVideoStreamObject).parent().removeClass('loading');
                console.debug('buddyVideoInitialized');
            }
            else if (state == 9 && (chat.buddyVideoInitialized == true || self.buddyUsersVideoSessionId() == self.currentBroadcastSessionId)) {
                //debugger;
                if (self.buddyUsersVideoSessionId() == self.currentBroadcastSessionId) {
                    self.currentBroadcastSessionId = '';
                }

//                if (self.privateVideoMode() && !kinkyApp.viewModel.MyUser().IsModel) {
//                    self.privateVideoChatClose();
//                }
//                else {
                    chat.closeBuddyUsersVideo();
//                }
            }
        };
        var flashvars = {
            __type: 'viewer',
            __rtmp: 'rtmp://88.198.39.25:1935/kinkylove',
            __stream: self.buddyUsersVideoSessionId(),
            __autoplay: '1',
            __log: '1',
            __sign: self.buddyUsersSignature(),
            __getParamFunJS: 'getVideoParamsHandler', //TODO: repair this shit
            __setStatusJS: 'kinkyApp.videoHandlers["' + videoHandlerName + '"]'
        };
        var params = {
            menu: 'true',
            allowFullscreen: 'true',
            wmode: 'transparent',
//            wmode: 'window',
            allowScriptAccess: 'sameDomain'
        };
        var attributes = { id: videoId, name: videoId };

        swfobject.embedSWF('/client.swf', videoContainerId, width, height, '11.0.0', '/expressInstall.swf', flashvars, params, attributes);
        self.buddyUsersVideoStreamObject = $('#' + videoId).length > 0 ? $('#' + videoId)[0] : null;
        self.buddyUsersVideoStreamOpened(true);

        if (!forced) {
            $(self.buddyUsersVideoStreamObject).parent().addClass('loading');
        }
        
        if (self.myVideoStreamOpened() && self.buddyUsersVideoStreamOpened()) {
            smallContainer.removeClass('hidden').removeClass('offline');
        }

        if (!self.myVideoStreamOpened() && self.buddyUsersVideoStreamOpened()) {
            smallContainer.removeClass('hidden').addClass('offline');
        }

        if (self.myVideoStreamOpened() && !self.buddyUsersVideoStreamOpened()) {
            smallContainer.addClass('hidden').removeClass('offline');
        }

        if (!smallContainer.prev().hasClass('parent')) {
            smallContainer.insertAfter(smallContainer.next());
        }
    };
    
    this.myVideoInitialized = false;
    this.buddyVideoInitialized = false;

    this.swapVideo = function () {
        if (self.myVideoStreamObject && self.buddyUsersVideoStreamObject && self.myVideoInitialized) {
            
            var buddySwfObject = $(self.buddyUsersVideoStreamObject);
            var buddyVideoContainer = buddySwfObject.parent();
            var mySwfObject = $(self.myVideoStreamObject);
            var myVideoContainer = mySwfObject.parent();

            if (myVideoContainer.hasClass('parent')) {
                buddySwfObject.css({
                    'width': '540px',
                    'height': '414px'
                });
                mySwfObject.css({
                    'width': '230px',
                    'height': '170px'
                });

                myVideoContainer.removeClass('parent').addClass('children');
                buddyVideoContainer.removeClass('children').addClass('parent');
            }
            else {
                mySwfObject.css({
                    'width': '540px',
                    'height': '414px'
                });
                buddySwfObject.css({
                    'width': '230px',
                    'height': '170px'
                });

                buddyVideoContainer.removeClass('parent').addClass('children');
                myVideoContainer.removeClass('children').addClass('parent');
            }
        }

        return true;
    };

    this.initMyVideo = function (successCallback) {
        self.initBuddyUsersVideo();
        
        //debugger;
        var videoContainerId = self.buddyUsersVideoStreamOpened()
            ? 'small-video-stream-placeholder-' + parentViewModel.MyUser().UserId + '-' + self.BuddyUser().UserId
            : 'big-video-stream-placeholder-' + parentViewModel.MyUser().UserId + '-' + self.BuddyUser().UserId;
        var smallContainer = $('#small-video-stream-placeholder-' + parentViewModel.MyUser().UserId + '-' + self.BuddyUser().UserId).parent();
        var height = self.buddyUsersVideoStreamOpened() ? 170 : 414; //414;
        var width = self.buddyUsersVideoStreamOpened() ? 230 : 540;
        var videoId = 'video-stream-' + parentViewModel.MyUser().UserId + '-' + self.BuddyUser().UserId;
        var videoHandlerName = 'handleMyVideo_' + parentViewModel.MyUser().UserId + '_' + self.BuddyUser().UserId;

        kinkyApp.videoHandlers[videoHandlerName] = function(state) {
            var chat = ko.utils.arrayFirst(kinkyApp.viewModel.Chats(), function(item) {
                return item.BuddyUser().UserId == self.BuddyUser().UserId;
            });

            if (state == 33) {
                if (self.myVideoStreamObject && self.myVideoStreamObject.setterJS) {
                    self.myVideoStreamObject.setterJS("publish", "start");
                }
            }
            else if (state == 22) {
                
            }
            else if (state == 17) {
                debugger;
                {
                    var watchdogVideoId = 'watchdog-' + videoId;
                    var watchdogHandlerName = 'watchdog_' + parentViewModel.currentVideoSessionId;

                    kinkyApp.videoHandlers[watchdogHandlerName] = function (watchdogState) {
                        console.info('watchdog handler: ' + watchdogHandlerName + ', current state: ' + watchdogState);

                        var watchdogVideo = $('#' + watchdogVideoId).length > 0 ? $('#' + watchdogVideoId)[0] : null;

                        if (watchdogVideo != null) {
                            if (watchdogState == 10) {
                                console.info('watchdog video initialized properly');
                                chat.myVideoInitialized = true;
                                console.debug('myVideoInitialized');
                                watchdogVideo.setterJS('volume', '0');
                            }
                        }
                    };

                    $('#broadcasting-watchdog-container')
                        .children().first()
                        .attr('id', watchdogVideoId);

                    var wFlashvars = {
                        __type: 'viewer',
                        __rtmp: 'rtmp://88.198.39.25:1935/kinkylove',
                        __stream: parentViewModel.currentVideoSessionId,
                        __autoplay: '1',
                        __log: '1',
                        __sign: parentViewModel.signature,
                        __setStatusJS: 'kinkyApp.videoHandlers["' + watchdogHandlerName + '"]'
                    };
                    var wParams = {
                        menu: 'false',
                        allowFullscreen: 'false',
                        wmode: 'transparent',
                        allowScriptAccess: 'sameDomain'
                    };
                    var wAttributes = { id: watchdogVideoId, name: watchdogVideoId };

                    swfobject.embedSWF('/client.swf', watchdogVideoId, 1, 1, '11.0.0', '/expressInstall.swf', wFlashvars, wParams, wAttributes);
                }
                
//                chat.myVideoInitialized = true;
                successCallback();
//                console.debug('myVideoInitialized');
            }
            else if (state == 9 && chat.myVideoInitialized == true) {
                chat.closeMyVideo();
            }
        };
        
        var flashvars = {
            __type: 'publisher',
            __rtmp: 'rtmp://88.198.39.25:1935/kinkylove',
            __stream: parentViewModel.currentVideoSessionId,
            __autoplay: '1',
            __log: '1',
            __sign: parentViewModel.signature,
            __getParamFunJS: 'getVideoParamsHandler', //TODO: repair this shit
            __setStatusJS: 'kinkyApp.videoHandlers["' + videoHandlerName + '"]'
        };
        var params = {
            menu: 'true',
            allowFullscreen: 'true',
            wmode: 'transparent',
//            wmode: 'window',
            allowScriptAccess: 'sameDomain'
        };
        var attributes = { id: videoId, name: videoId };

        swfobject.embedSWF('/client.swf', videoContainerId, width, height, '11.0.0', '/expressInstall.swf', flashvars, params, attributes);
        self.myVideoStreamObject = $('#' + videoId).length > 0 ? $('#' + videoId)[0] : null;
        self.myVideoStreamOpened(true);
//        debugger;
        kinkyApp.asyncRequest({
            url: kinkyApp.data.urlTmpl.NotifyUserOfVideoChatStart,
            type: "POST",
            data: {
                buddyUserId: self.BuddyUser().UserId,
                sessionId: parentViewModel.currentVideoSessionId
            }
        });

        setTimeout(function () {
            var attemptCount = 0;
            var trySetVideoQuality = function () {
                //debugger;
                if (self.myVideoStreamObject && self.myVideoStreamObject.setterJS) {
                    self.myVideoStreamObject.setterJS("cameraQ", "99");
                    self.myVideoStreamObject.setterJS("cameraW", "800");
                    self.myVideoStreamObject.setterJS("cameraH", "600");                    
                }
                else {
                    if (attemptCount < 5) {
                        attemptCount++;
                        setTimeout(trySetVideoQuality, 1000);
                    }
                }
            };

            trySetVideoQuality();
        }, 1000);
        
        if (self.myVideoStreamOpened() && self.buddyUsersVideoStreamOpened()) {
            smallContainer.removeClass('hidden').removeClass('offline');
        }

        if (!self.myVideoStreamOpened() && self.buddyUsersVideoStreamOpened()) {
            smallContainer.removeClass('hidden').addClass('offline');
        }

        if (self.myVideoStreamOpened() && !self.buddyUsersVideoStreamOpened()) {
            smallContainer.addClass('hidden').removeClass('offline');
        }
    };

    this.closeMyVideo = function() {
//        debugger;
        kinkyApp.asyncRequest({
            url: kinkyApp.data.urlTmpl.NotifyUserOfVideoChatEnd,
            type: "POST",
            data: {
                buddyUserId: self.BuddyUser().UserId,
                sessionId: parentViewModel.currentVideoSessionId
            },
            success: function(data) {
                if (self.myVideoStreamObject != null) {
                    var swfObject = $(self.myVideoStreamObject);
                    var container = swfObject.parent();
                    var isMainContainer = container.hasClass('parent');
                    

                    if (parentViewModel.myVideoSessionsOpened == 1) {
                        self.myVideoStreamObject.setterJS && self.myVideoStreamObject.setterJS("unpublish", "start");
                    }
                    
                    self.myVideoStreamOpened(false);
                    
                    if (isMainContainer) {
                        if (self.buddyUsersVideoStreamOpened() && self.buddyUsersVideoStreamObject != null) {
                            var smallContainer = $(self.buddyUsersVideoStreamObject).parent();
                            var buddySwfObject = $(self.buddyUsersVideoStreamObject);

                            smallContainer.removeClass('children').addClass('parent');
                            container.removeClass('parent').addClass('children');
                            buddySwfObject.css({
                                'width': '540px',
                                'height': '414px'
                            });     
                        }
                    }
                    
                    isMainContainer = container.hasClass('parent');
                    
                    var placeholder = '<div id="' + (isMainContainer ? 'big' : 'small') + '-video-stream-placeholder-' + parentViewModel.MyUser().UserId + '-' + self.BuddyUser().UserId + '"></div>';
                    
                    swfObject.remove();
                    container.prepend(placeholder);
                    container.insertAfter(container.next());
//                    debugger;

                    $('#broadcasting-watchdog-container').html('<div></div>');

                    var videoHandlerName = 'handleMyVideo_' + parentViewModel.MyUser().UserId + '_' + self.BuddyUser().UserId;
                    var watchdogHandlerName = 'watchdog_' + parentViewModel.currentVideoSessionId;
                    
                    delete kinkyApp.videoHandlers[videoHandlerName];
                    delete kinkyApp.videoHandlers[watchdogHandlerName];

                    self.myVideoStreamObject = null;
                    self.myVideoInitialized = false;

                    var cont = container.hasClass('parent') ? smallContainer : container;
                    if (self.myVideoStreamOpened() && self.buddyUsersVideoStreamOpened()) {
                        cont.removeClass('hidden').removeClass('offline');
                    }

                    if (!self.myVideoStreamOpened() && self.buddyUsersVideoStreamOpened()) {
                        cont.removeClass('hidden').addClass('offline');
                    }

                    if (self.myVideoStreamOpened() && !self.buddyUsersVideoStreamOpened()) {
                        cont.addClass('hidden').removeClass('offline');
                    }
                }
            }
        });
    };
    
    this.closeBuddyUsersVideo = function () {
        //debugger;
        if (self.buddyUsersVideoStreamObject != null) {
            var swfObject = $(self.buddyUsersVideoStreamObject);
            var container = swfObject.parent();
            var isMainContainer = container.hasClass('parent');
      
            self.buddyUsersVideoStreamOpened(false);
            $(self.buddyUsersVideoStreamObject).parent().removeClass('loading');
            
            if (isMainContainer) {
                if (self.myVideoStreamOpened() && self.myVideoStreamObject != null) {
                    var smallContainer = $(self.myVideoStreamObject).parent();
                    var mySwfObject = $(self.myVideoStreamObject);

                    smallContainer.removeClass('children').addClass('parent');
                    container.removeClass('parent').addClass('children');
                    mySwfObject.css({
                        'width': '540px',
                        'height': '414px'
                    });
                }
            }

            isMainContainer = container.hasClass('parent');
            
            var placeholder = '<div id="' + (isMainContainer ? 'big' : 'small') + '-video-stream-placeholder-' + parentViewModel.MyUser().UserId + '-' + self.BuddyUser().UserId + '"></div>';
            
            swfObject.remove();
            container.prepend(placeholder);
            container.insertAfter(container.next());

            var videoHandlerName = 'handleBuddyVideo_' + parentViewModel.MyUser().UserId + '_' + self.BuddyUser().UserId;
            
            delete kinkyApp.videoHandlers[videoHandlerName];
            
            self.buddyUsersVideoStreamObject = null;
            self.buddyVideoInitialized = false;
            
            var cont = container.hasClass('parent') ? smallContainer : container;
            if (self.myVideoStreamOpened() && self.buddyUsersVideoStreamOpened()) {
                cont.removeClass('hidden').removeClass('offline');
            }

            if (!self.myVideoStreamOpened() && self.buddyUsersVideoStreamOpened()) {
                cont.removeClass('hidden').addClass('offline');
            }

            if (self.myVideoStreamOpened() && !self.buddyUsersVideoStreamOpened()) {
                cont.addClass('hidden').removeClass('offline');
            }
        }
    };
    
    this.privateVideoChatButtonText = ko.observable('');
    this.showGuide = ko.observable(false);
    this.clientsCashInitial = ko.observable(0);
    this.clientsCash = ko.observable({integerPart: 0, fractionPart: '00'});
    
    this.privateVideoChatAccept = function () {
        kinkyApp.viewModel.broadcastingModel.stopBroadcasting();
        self.privateVideoMode(true);
        //self.showGuide(true);
        
        self.startVideoChat(true, function() {
            sendToServer(function (serverHub) {
                serverHub.acceptPrivateVideoChat(self.BuddyUser().UserId)
                    .done(function (result) {
                        if (result.success) {
                            self.privateVideoChatButtonText($('#private-video-chat-button-' + self.BuddyUser().UserId).data('started-text'));
                            self.privateVideoMode(true);
                            self.privateVideoChatStartTime = Date.now();
                            self.showPrivateVideoChatInfoPanel(true);
                            self.showGuide(false);
                            self.clientsCashInitial(result.clientsCash);
                        }
                    })
                    .fail(function (res) {
                        debugger;
                        console.error(res);
                    });
            });
        });
    };
    
    this.privateVideoChatClose = function () {
        //        debugger;
        sendToServer(function (serverHub) {
            serverHub.endPrivateVideoChat(self.BuddyUser().UserId)
                .done(function (result) {
                    if (result) {
                        self.privateVideoChatButtonText($('#private-video-chat-button-' + self.BuddyUser().UserId).data('stopped-text'));
                        self.privateVideoMode(false);
                        self.closeMyVideo();
                        self.showPrivateVideoChatInfoPanel(false);
                        self.showModelIsBusy(false);
                    }
                });
        });
    };

    this.privateVideoChatToggle = function() {
//        debugger;
        if (self.privateVideoMode()) {
            
            kinkyApp.modal.showTopByUrl(kinkyApp.data.urlTmpl.ModalBlockerConfirmCloseVideoChat + '?userId=' + self.BuddyUser().UserId + '&close=false');

            return;
            
            sendToServer(function(serverHub) {
                serverHub.endPrivateVideoChat(self.BuddyUser().UserId)
                    .done(function(result) {
                        if (result) {
                            self.privateVideoChatButtonText($('#private-video-chat-button-' + self.BuddyUser().UserId).data('stopped-text'));
                            self.privateVideoMode(false);
                            self.closeMyVideo();
                            self.showPrivateVideoChatInfoPanel(false);
                            self.showModelIsBusy(false);
                        }
                    });
            });
        }
        else {
            sendToServer(function(serverHub) {
                serverHub.startPrivateVideoChat(self.BuddyUser().UserId)
                    .done(function(result) {
                        if (result) {
                            self.privateVideoChatButtonText($('#private-video-chat-button-' + self.BuddyUser().UserId).data('started-text'));
                            self.privateVideoMode(true);
                        }
                    });
            });
        }
    };

    this.privateVideoMode = ko.observable(false);

    this.privateVideoMode.subscribe(function (value) {
        if (value === true) {
            self.chatWindow.draggable('disable');
            
            var width = $(window).width();
            var height = $(window).height();

            self.chatWindow.css({
                left: ((width - 560) / 2) + (560 / 2) + 'px',
                top: ((height - 496) / 2) + 'px'
            });
        }
        else {
            self.chatWindow.draggable('enable');
        }
    });

    this.micIsOn = ko.observable(true);
    
    this.toggleMic = function(element) {
        var el = $(element);

        if (/*el.hasClass('chat-header__mic-control-mute')*/ !self.micIsOn()) {
            el.removeClass('chat-header__mic-control-mute').addClass('chat-header__mic-control-on');
            
            self.myVideoStreamObject.setterJS("micGain", "80");
            self.micIsOn(true);
        }
        else {
            el.removeClass('chat-header__mic-control-on').addClass('chat-header__mic-control-mute');
            
            self.myVideoStreamObject.setterJS("micGain", "0");
            self.micIsOn(false);
        }
    };
    
    this.formatDate = function (stringDate, nullText, prefix) {
        if (stringDate == null || stringDate == "") return nullText || "";
        var date = new Date(stringDate);
        return (prefix || "") + date.getHours() + ":" + FormatNumberLength(date.getMinutes(), 2) + "&emsp;" + FormatNumberLength(date.getDate(), 2) + "." + FormatNumberLength((date.getMonth() + 1), 2) + "." + date.getFullYear();
    };

    function FormatNumberLength(num, length) {
        var r = "" + num;
        while (r.length < length) {
            r = "0" + r;
        }
        return r;
    }

    this.ShowPhotoDetails = function (data) {
        kinkyApp.modal.showByUrl(kinkyApp.data.urlTmpl.PhotoDetails.replace('~0', data.ItemUserId).replace('~1', data.ItemId), null, true);
    };

    this.ShowInterest = function (data) {
        kinkyApp.updateElementById(kinkyApp.data.urlTmpl.Interest.replace('~0', data.ItemUserId).replace('~1', data.ItemId), 'body');
    };
};

ko.bindingHandlers.htmlValue = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var updateHandler = function () {
            var modelValue = valueAccessor(),
                elementValue = element.innerHTML;

            //update the value on keyup
            modelValue(elementValue);
        };

        ko.utils.registerEventHandler(element, "keyup", updateHandler);
        ko.utils.registerEventHandler(element, "input", updateHandler);
    },
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor()) || "",
            current = element.innerHTML;

        if (value !== current) {
            element.innerHTML = value;
        }
    }
};