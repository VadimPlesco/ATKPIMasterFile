
function sendToServer(func) {
    try {
        if ($.connection.hub.state != 1) {
            $.connection.hub.start(/*{ pingInterval: 30 }*/).done(function () {
                func($.connection.eventsHub);
                kinkyApp.viewModel.StartPingTimer(50 * 1000);
                console.log("--------------- start connection 2");
            }).fail(function (reason) {
                console.log(reason);
            });
        }
        else {
            func($.connection.eventsHub.server);
        }
    }
    catch (e) {
        console.log(e);
    }
}


$(function () {

    var initNotificationGet = function () {
        var notificationGetInterval = setInterval(function () {
            $.connection.eventsHub.server.getNotfication().done(function (data) {
                console.log(data);
                if (data) {
                    if (data.notification)
                        kinkyApp.viewModel.notificationsViewModel.addNotification(data.notification);
                    if (!data.state)
                        clearInterval(notificationGetInterval);
                }
            });
        }, 5000);
    };

    if (document.getElementById("menu")) {
        var initSignalRTryCount = 0;
        var initSignalR = function () {
            console.info("init SignalR Try Nr:", initSignalRTryCount);
            $.getScript(kinkyApp.data.urlTmpl.SignalrUrl + "/hubs", function (sc, stat, xhr) {

                $.connection.hub.url = kinkyApp.data.urlTmpl.SignalrUrl;

                $.connection.eventsHub.client.receiveChatMessage = function (chatMessages, contact, myUser) {
                    //    console.log(chatMessages);
                    console.info("receiveChatMessage");
                    var ka = kinkyApp;

                    try {
                        if (contact)
                            ka.viewModel.putContacts(contact);
                        if (myUser)
                            ka.viewModel.MyUser(myUser);
                        if (chatMessages.length > 0) {
                            var chat = ka.viewModel.initChat(chatMessages[0].ChatUser, false);
                            if (!chatMessages[0].IsMy)
                                ka.viewModel.playAudio();
                            if (chat) {
                                chat.putChatMessages(chatMessages);
                                chat.tryMarkAsRead();
                            }

                            if (chatMessages.length > 0 && (chatMessages[0].Type == 54 || chatMessages[0].Type == 55)) {
                                //                    debugger;

                                if (chat) {
                                    if (chatMessages[0].Type == 54) {
                                        for (var idx = 0; idx < kinkyApp.viewModel.Chats().length; idx++) {
                                            var usrChat = kinkyApp.viewModel.Chats()[idx];

                                            if (usrChat.Active() && usrChat.BuddyUser().UserId != chatMessages[0].ChatUser.UserId) {
                                                usrChat.Active(false);
                                            }
                                        }

                                        if (!chat.Active()) {
                                            //                                            debugger;
                                            chat.Active(true);
                                        }
                                        else {
                                            chat.bringToFront();
                                        }

                                        if (kinkyApp.viewModel.MyUser().IsModel) {
                                            chat.privateVideoChatAccept();
                                        }
                                    }
                                    else {
                                        chat.privateVideoChatClose();
                                    }
                                }
                            }

                            var premium = chatMessages.length > 0 && chatMessages[0].Type == 33;

                            if (premium) {
                                kinkyApp.analytics.trackEvent(kinkyApp.analytics.eventType.premiumStatusActivated);
                            }

                            var clubCard = chatMessages.length > 0 && chatMessages[0].Type == 39;

                            if (clubCard) {
                                if (typeof window.__insp !== 'undefined') {
                                    window.__insp.push(['tagSession', 'clubcard_295']);
                                }
                            }
                        }
                    }
                    catch (e) {
                        alert("initChat error:" + e);
                    }
                };

                $.connection.eventsHub.client.receiveReadTS = function (chatUser, events) {
                    console.info("receiveReadTS");
                    var chat = kinkyApp.viewModel.initChat(chatUser, false);
                    if (chat) {
                        chat.updateChatMessages(events);
                    }
                };

                $.connection.eventsHub.client.notify = function (notification) {
                    console.info("notify-" + notification.Type + "-U:" + notification.LiteUser.UserId);
                    if (notification.CustomData && notification.CustomData.FullUser) {
                        kinkyApp.viewModel.putUser(notification.CustomData.FullUser, true);
                    }
                    else if (notification.LiteUser)
                        kinkyApp.viewModel.putUser(notification.LiteUser, true);

                    if (!notification.LiteUser)
                        console.info(notification);

                    var chat;

                    if (notification.Type == 6) {//BroadcastStarted
                        chat = kinkyApp.viewModel.initChat(notification.CustomData.FullUser, false);
                        chat.currentBroadcastSessionId = notification.CustomData.sessionId;
                        chat.BuddyUser(notification.CustomData.FullUser);

                        console.log('^-------------------------------------------------------------^');
                        console.log(notification);
                        console.log(notification.CustomData);
                        console.log('^-------------------------------------------------------------^');
                    }

                    if (notification.Type == 16) {//VideoChatStarted
                        chat = kinkyApp.viewModel.initChat(notification.CustomData.FullUser, false);
                        chat.closeBuddyUsersVideo();
                        chat.buddyUsersVideoSessionId(notification.CustomData.sessionId);
                        chat.buddyUsersSignature(notification.CustomData.signature);
                        chat.BuddyUser(notification.CustomData.FullUser);
                        chat.initBuddyUsersVideo(true);

                        return;
                    }

                    if (notification.Type == 17) {//VideoChatStopped
                        //debugger;
                        chat = kinkyApp.viewModel.initChat(notification.CustomData.FullUser, false);

                        chat.buddyUsersVideoSessionId('');
                        chat.buddyUsersSignature('');
                        chat.closeBuddyUsersVideo();

                        return;
                    }

                    if (notification.Type == 18) {//BroadcastStopped
                        //debugger;
                        chat = kinkyApp.viewModel.initChat(notification.CustomData.FullUser, false);

                        if (!chat.privateVideoMode()) {
                            chat.buddyUsersVideoSessionId('');
                            chat.buddyUsersSignature('');
                            chat.closeBuddyUsersVideo();
                        }

                        return;
                    }

                    if (notification.Type == 20) {//ModelIsBusyInPrivateChat
                        //debugger;
                        chat = kinkyApp.viewModel.initChat(notification.CustomData.FullUser, false);

                        if (!chat.privateVideoMode()) {
                            chat.showModelIsBusy(true);
                        }

                        return;
                    }

                    if (notification.Type == 21) {//ModelIsFreeForPrivateChat
                        //debugger;
                        chat = kinkyApp.viewModel.initChat(notification.CustomData.FullUser, false);
                        chat.BuddyUser(notification.CustomData.FullUser);

                        return;
                    }

                    if (notification.Type > 15) {
                        return;
                    }

                    //                    debugger;
                    var privateChat = ko.utils.arrayFirst(kinkyApp.viewModel.Chats(), function (item) {
                        return item.privateVideoMode() == true;
                    });

                    if (privateChat != null) {
                        return;
                    }

                    if (
                        !(notification.IsGroup && notification.LiteUser.UserId == kinkyApp.viewModel.MyUser().UserId)
                        &&
                        !(notification.CustomData && notification.CustomData.Hide)
                        ) {
                        kinkyApp.viewModel.notificationsViewModel.addNotification(notification);
                    }

                    if (notification.Type == 10/* || notification.Type == 13*/)
                        kinkyApp.reloadBody();
                };


                $.connection.eventsHub.client.updateUserView = function (user) {
                    console.info("updateUserView:" + user.UserId);
                    if (kinkyApp.viewModel.MyUser().UserId == user.UserId) {
                        kinkyApp.viewModel.MyUser(user);
                    }
                    kinkyApp.viewModel.putUser(user);
                };

                $.connection.eventsHub.client.resetChat = function (user) {
                    console.info("resetChat:" + user.UserId);

                    kinkyApp.viewModel.resetChat(user);
                };

                $.connection.eventsHub.client.updateUserEventState = function (userEventState) {
                    console.info(userEventState);
                    kinkyApp.viewModel.MyUserEventState(userEventState);

                    if (userEventState.LastUserLikesCount > 0) {
                        $('#last-events-liked')
                            .text('+' + userEventState.LastUserLikesCount)
                            .show();
                    }

                    if (userEventState.LastRegularEventCount > 0) {
                        $('#last-events-regular')
                            .text('+' + userEventState.LastRegularEventCount)
                            .show();
                    }
                };

                $.connection.eventsHub.client.buddyUserTyping = function (buddyUserId) {
                    console.info("buddyUserTyping");
                    var chat = ko.utils.arrayFirst(kinkyApp.viewModel.Chats(), function (item) {
                        return buddyUserId === item.BuddyUser().UserId;
                    });
                    if (chat != null)
                        chat.BuddyUserTyping(true);
                };

                $.connection.eventsHub.client.showModal = function (url) {
                    console.info("showModal");
                    kinkyApp.modal.showTopByUrl(url);
                };

                $.connection.eventsHub.client.updateSiteVersion = function (toFeed) {
                    console.info("updateSiteVersion");
                    if (toFeed)
                        document.location.href = "/";
                    else
                        document.location.reload();
                };

                $.connection.eventsHub.client.notifyUserAboutPrivateVideoChatStart = function (buddyUserId) {
                    var chat = ko.utils.arrayFirst(kinkyApp.viewModel.Chats(), function (item) {
                        return item.BuddyUser().UserId == buddyUserId;
                    });

                    if (chat != null) {
                        //            chat.buddyUserIsInPrivateChat(true);
                    }
                };

                $.connection.eventsHub.client.notifyUserAboutPrivateVideoChatEnd = function (buddyUserId) {
                    var chat = ko.utils.arrayFirst(kinkyApp.viewModel.Chats(), function (item) {
                        return item.BuddyUser().UserId == buddyUserId;
                    });

                    if (chat != null) {
                        //            chat.buddyUserIsInPrivateChat(false);
                    }
                };

                $.connection.eventsHub.client.receivePrivateVideoChatInfo = function (userId, buddyUserId, diamonds) {
                    console.info("receivePrivateVideoChatInfo: " + userId + "; " + buddyUserId + "; " + diamonds);
                    if (diamonds < 0) {
                        //говорим, что недостаточно денег у мальчика
                        //userId - id мальчика
                        if (kinkyApp.viewModel.MyUser().UserId == userId) {
                            kinkyApp.modal.showTopByUrl(kinkyApp.data.urlTmpl.ModalBlockerPrivateVideoChat);


                            var usrChat = ko.utils.arrayFirst(kinkyApp.viewModel.Chats(), function (item) {
                                return item.BuddyUser().UserId == buddyUserId;
                            });

                            if (usrChat) {
                                usrChat.privateVideoChatClose();
                                usrChat.showModelIsBusy(false);
                            }
                        }
                    }
                    else {
                        //Обновляем баланс в чате у девочки
                        //userId - id девочки

                        var chat = ko.utils.arrayFirst(kinkyApp.viewModel.Chats(), function (item) {
                            return item.BuddyUser().UserId == buddyUserId;
                        });

                        if (chat) {
                            //chat.privateVideoChatReward(diamonds);
                        }
                    }
                };


                $.connection.hub.start(/*{ pingInterval: 30 }*/).done(function () {

                    var timezoneOffset = (new Date).getTimezoneOffset() * -1;

                    $.connection.eventsHub.server.initUser(timezoneOffset).done(function (res) {
                        try {
                            console.info("InitUser().done");
                            kinkyApp.viewModel.MyUser(res.User);
                            kinkyApp.viewModel.MyUserEventState(res.UserEventState);
                            kinkyApp.viewModel.putUser(res.User);

                            kinkyApp.viewModel.Contacts.removeAll();
                            kinkyApp.viewModel.Chats.removeAll();
                            kinkyApp.viewModel.putContacts(res.Contacts);
                            kinkyApp.viewModel.MoneyItems(res.Money);
                            kinkyApp.viewModel.Gifts(res.Gifts);
                            if (!ko.dataFor(document.getElementById("menu")))
                                ko.applyBindings(kinkyApp.viewModel, document.getElementById("menu"));
                            var notify_holder = document.getElementById("notify-holder");
                            if (notify_holder && !ko.dataFor(notify_holder))
                                ko.applyBindings(kinkyApp.viewModel.notificationsViewModel, document.getElementById("notify-holder"));
                            var match = document.location.hash.match(/#chat-(\d+)/);
                            if (match)
                                kinkyApp.viewModel.showChat(parseInt(match[1]));
                            var broadcasting_modal = document.getElementById("broadcasting-modal");
                            if (broadcasting_modal && !ko.dataFor(broadcasting_modal))
                                //                        ko.applyBindings(kinkyApp.viewModel.publishVideoModal, document.getElementById("broadcasting-modal"));
                                ko.applyBindings(kinkyApp.viewModel.broadcastingModel, document.getElementById("broadcasting-modal"));

                            initNotificationGet();

                        } catch (e) {
                            console.error('connection.eventsHub.server.InitUser ', e)
                            //alert('connection.eventsHub.server.InitUser ' + e);
                        }
                    });

                    kinkyApp.viewModel.StartPingTimer(50 * 1000);
                });

                $.connection.hub.error(function (error) {
                    //if (error.status == 400)
                    //    location.reload();
                });
            })
                .fail(function (jqxhr, settings, exception) {
                    initSignalRTryCount++;
                    if (initSignalRTryCount < 180)
                        setTimeout(initSignalR, 10000);
                });
        };

        setTimeout(initSignalR, 100);
    }
});