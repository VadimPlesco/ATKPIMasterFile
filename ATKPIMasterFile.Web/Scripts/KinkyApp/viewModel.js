
kinkyApp.ViewModel = function () {

    var self = this;

    this.MyUser = ko.observable({ Name: "" });
    this.MyUserEventState = ko.observable({ PrivateFeedNewsCount: 0, LastEventsCount: 0, FollowersNewCount: 0 });
    this.UserInfo = ko.observable({});
    this.emailConfirmationVisible = false;
    
    var _myUserLastVar = null;
    this.MyUser.subscribe(function (newValue) {
        if (_myUserLastVar && _myUserLastVar.ConfirmationStatus != newValue.ConfirmationStatus) {
            if (newValue.ConfirmationStatus || newValue.Email == '')
                $("#sendEmailConfirmationAgain").addClass("hidden");
            else
                $("#sendEmailConfirmationAgain").removeClass("hidden");
        }

        if (_myUserLastVar && _myUserLastVar.SocialStatusText == '' && newValue.SocialStatusText != '') {
            var href = $('#menu-item-dating').data('href');
            var i = href.indexOf('?');

            href = href.substring(0, i);
            $('#menu-item-dating').data('href', href);
            $('#menu-item-dating').attr('onclick', "kinkyApp.updateElementById('" + href + "','body')");
        }


        if (_myUserLastVar != null && (newValue.RealSocialStatus != self.MyUser().RealSocialStatus || _myUserLastVar.IsModel != newValue.IsModel)) {
            kinkyApp.reloadBody();
        }
        
        self.broadcastingModel.watchersCount(newValue.WatchersCount);
        self.broadcastingModel.updateWatchers(newValue.Watchers);
        self.UserInfo({ AvatarUrl: newValue.AvatarUrl, AvatarWidth: newValue.AvatarWidth, Status: newValue.SocialStatusText });

        //if (_myUserLastVar == null || _myUserLastVar.UserId != newValue.UserId) {
        //    kinkyApp.analytics.resetUser();
        //}

        if (newValue.SocialStatusText == '') {
            $('#buy-premium-label, #premium-label, #avatar-premium-label-' + newValue.UserId).hide();
        }
        else {
            if (newValue.IsPremium) {
                $('#premium-label, #avatar-premium-label-' + newValue.UserId).show();
                $('#buy-premium-label').hide();
            }
            else {
                $('#premium-label, #club-card-label, #avatar-premium-label-' + newValue.UserId).hide();
                $('#buy-premium-label').show();
            }
        }

        _myUserLastVar = newValue;

        if (!self.emailConfirmationVisible && newValue.ConfirmationEmailStartDate > 0) {

            var confirmationStartDate = +new Date(newValue.ConfirmationEmailStartDate * 1000);
            var currentDate = +new Date();
            
            if (currentDate > confirmationStartDate) {
                kinkyApp.modal.showByUrl('/ctrl/Account/EmailConfirmationAffiliate', null, false, true);
                self.emailConfirmationVisible = true;
            }
            else {
                self.emailConfirmationVisible = true;
                setTimeout(function() {
                    kinkyApp.modal.showByUrl('/ctrl/Account/EmailConfirmationAffiliate', null, false, true);
                }, confirmationStartDate - currentDate);
            }
        }
    });

    this.getFullHostName = function (url) {
        var http = location.protocol;
        var slashes = http.concat("//");
        var host = slashes.concat(window.location.hostname);

        console.debug(host + "//"+ url);

        return host + url;
    };

    this.Chats = ko.observableArray();
    this.Users = ko.observableArray();
    this.MoneyItems = ko.observableArray([]);
    this.Gifts = ko.observableArray([]);

    this.Contacts = ko.observableArray();
    this.ContactsMenu = ko.observableArray();

    //    this.publishVideoModal = new kinkyApp.PublishVideoModal(this);

    this.broadcastingModel = new kinkyApp.PublishVideoModel('publisher-video-frame');

    this.notificationsViewModel = new kinkyApp.NotificationsViewModel(this);

    this.UnreadCount = ko.observable(0);
    this.maxContactsMenuCountPrev = -1;
    this.MaxContactsMenuCount = ko.observable(0);

    this.resetChat = function (user) {
        var chatToReset = ko.utils.arrayFirst(self.Chats(), function (chat) {
            return chat.BuddyUser().UserId == user.UserId;
        });

        if (chatToReset) {
            chatToReset.ChatMessages.removeAll();
            chatToReset.showDeleteCorrespondencePanel(false);
        }
    };

    this.MaxContactsMenuCount.subscribe(function (newValue) {
        if (self.maxContactsMenuCountPrev == newValue)
            return;
        self.UpdateContactsMenu();
        self.maxContactsMenuCountPrev = newValue;
    });

    this.UpdateContactsMenu = function () {
        var temp = self.Contacts();
        var count = Math.min(self.MaxContactsMenuCount(), temp.length);
        self.ContactsMenu.removeAll();
        var i = 0;
        for (; i < count; i++)
            self.ContactsMenu.push(temp[i]);
        var unreadCount = 0;
        for (; i < temp.length; i++)
            unreadCount += temp[i].UnreadCount;
        self.UnreadCount(unreadCount);
    };

    //this.getOnlineClass = function (user, isOnlineClass, wasRecentlyClass, isOfflineClass) {
    //    if (user.IsOnline)
    //        return isOnlineClass;
    //    if (user.WasRecently)
    //        return wasRecentlyClass;
    //    return isOfflineClass;
    //}

    this.putUser = function (user, putIfExist) {
        try {
            var curUser = ko.utils.arrayFirst(self.Users(), function (item) {
                return user.UserId === item.UserId;
            });

            if (!curUser) {
                if (putIfExist || user.IsLite)
                    return;
                self.Users.push(user);
                curUser = user;
            }

            if (user.IsLite)
            {
                curUser.IsOnline = user.IsOnline;
                curUser.WasRecently = user.WasRecently;
                user = curUser;
            }

            try {
                var els = $('div[data-online="' + user.UserId + '"]');
                if (user.IsOnline)
                    els.removeClass("hidden").removeClass("recent");
                else if (user.WasRecently)
                    els.removeClass("hidden").addClass("recent");
                else
                    els.addClass("hidden");
            }
            catch (ex) {
                alert(ex);
            }
            if (curUser.chat) {
                user.chat = curUser.chat;
                curUser.chat.BuddyUser(user);
            }

            var contact = ko.utils.arrayFirst(self.Contacts(), function (item) {
                return user.UserId === item.BuddyUser.UserId;
            });

            if (contact) {
                kinkyApp.mapAsObservable(contact, "BuddyUser", user);
                contact.BuddyUser = user;
            }

            return curUser;
        }
        catch (ex) {
            alert("putUser error:" + ex);
        }
        return null;
    };

    var lastContctDate = null;

    this.putContacts = function (obj) {
        try {
            if (Object.prototype.toString.call(obj) == '[object Array]') {
                for (var i = 0; i < obj.length; i++) {
                    kinkyApp.mapAsObservable(obj[i], "BuddyUser", obj[i].BuddyUser);
                    self.Contacts.push(obj[i]);
                }
                // self.Contacts(obj);
                lastContctDate = obj.length > 0 ? obj[obj.length - 1].UpdateTS : null;
            }
            else {
                self.Contacts.remove(function (item) { return item.BuddyUserId == obj.BuddyUserId; });
                kinkyApp.mapAsObservable(obj, "BuddyUser", obj.BuddyUser);
                self.Contacts.unshift(obj);

                var chat = ko.utils.arrayFirst(self.Chats(), function (item) {
                    return item.BuddyUser().UserId == obj.BuddyUserId;
                });

                if (chat) {
                    chat.sexyChatStatus(obj.SexyChatState);
                    chat.BuddyUser(obj.BuddyUser);
                }
            }
            self.Contacts.sort(function (left, right) { return left.UpdateTS.localeCompare(right.UpdateTS) * -1; });
            self.UpdateContactsMenu();
        }
        catch (ex) {
            alert("putContacts error:" + ex);
        }
    };

    this.getMoreContacts = function (callBack) {
        try {
            if (lastContctDate) {
                sendToServer(function (serverHub) {
                    serverHub.getMoreContacts(lastContctDate).done(function (res) {
                        self.putContacts(res);
                        if (callBack)
                            callBack(res.length);
                    });
                });
            }
            else if (callBack)
                callBack(0);
        }
        catch (e) {
            alert("getMoreContacts error:" + e);
        }
    };

    this.addContactPopup = function (element, index, data) {

        var messageLiveTime = 8000;

        if (element.nodeType === 1) {
            if (data.UnreadCount) {
                if (index == 0) {
                    if (!data.wasShown) {
                        $(element).children(".message-block__popup-menu-message-holder").css({ display: "block", opacity: 0, top: "+=10" }).animate({ top: "-=10", opacity: 0.6 }, 500);

                        setTimeout(function () {
                            $(element).children(".message-block__popup-menu-message-holder").animate({ top: "-=10", opacity: 0 }, 500, null, function () {
                                $(this).hide();
                            });
                            data.initTime = null;
                            data.wasShown = true;
                        }, messageLiveTime);

                        data.initTime = new Date();
                    }
                } else if (data.initTime != null) {
                    $(element).children(".message-block__popup-menu-message-holder").css({ display: "block", opacity: 0.6 });
                    setTimeout(function () {
                        $(element).children(".message-block__popup-menu-message-holder").animate({ top: "-=10", opacity: 0 }, 500, null, function () {
                            $(this).hide();
                        });
                        data.initTime = null;
                        data.wasShown = true;
                    }, messageLiveTime - ((new Date()) - data.initTime));
                }

                $(element).children(".message-block__popup-menu-message-holder").on("mouseover", function () { $(this).css("opacity", 0.85); }).on("mouseout", function () { $(this).css("opacity", 0.6); });
            }
        }
    };

    this.initChat = function (user, show) {
        try {
            if (!user)
                return;

            var chat = ko.utils.arrayFirst(self.Chats(), function (item) {
                return user.UserId === item.BuddyUser().UserId;
            });

            //user = self.putUser(user);

            if (!chat) {
                chat = new kinkyApp.ChatViewModel(self, user);
                self.Chats.push(chat);
                chat.init();
            }
            else {
                user.chat = chat;
            }

            if (show) {
                chat.Active(show);
                chat.tryMarkAsRead();
            }

            return chat;
        }
        catch (e) {
            alert("initChat error:" + e);
        }
        return null;
    };

    var sendAskInProcess = false;

    this.sendAskPhoto = function (userId, interestId) {
        try {

            if (sendAskInProcess) return; sendAskInProcess = true;

            self.getUser(userId, function (user) {
                self.initChat(user, true);
                sendToServer(function (serverHub) {
                    serverHub.sendChatMessage({ UserId: self.MyUser().UserId, BuddyUserId: userId, ItemId: interestId, Type: 9 })
                        .done(function (result) { sendAskInProcess = false; });
                });
            });
        }
        catch (e) {
            alert("sendMessage error:" + e);
        }
    };

    this.sendAskPersonalInfo = function (userId) {
        try {

            if (sendAskInProcess) return; sendAskInProcess = true;

            self.getUser(userId, function (user) {
                self.initChat(user, true);
                sendToServer(function (serverHub) {
                    serverHub.sendChatMessage({ UserId: self.MyUser().UserId, BuddyUserId: userId, Type: 10 })
                        .done(function (result) { sendAskInProcess = false; });
                });
            });
        }
        catch (e) {
            alert("sendMessage error:" + e);
        }
    };

    this.sendAskAvatar = function (userId) {
        try {

            if (sendAskInProcess) return; sendAskInProcess = true;

            self.getUser(userId, function (user) {
                self.initChat(user, true);
                sendToServer(function (serverHub) {
                    serverHub.sendChatMessage({ UserId: self.MyUser().UserId, BuddyUserId: userId, Type: 13 })
                        .done(function (result) { sendAskInProcess = false; });
                });
            });
        }
        catch (e) {
            alert("sendMessage error:" + e);
        }
    };


    this.sendPhotoToChatUserId = 0;
    this.sendPhotoToChat = function (itemId) {
        try {
            if (self.sendPhotoToChatUserId != 0) {
                sendToServer(function (serverHub) {
                    serverHub.sendChatMessage({ UserId: self.MyUser().UserId, BuddyUserId: self.sendPhotoToChatUserId, Type: 4, ItemId: itemId, Value: 0 });
                });
            }

        }
        catch (e) {
            alert("sendMessage error:" + e);
        }
    };

    this.sendAdvertClick = function (userId, itemId) {
        try {
            self.getUser(userId, function (user) {
                self.initChat(user, true);
                //sendToServer(function (serverHub) {
                //    serverHub.sendChatMessage({ UserId: self.MyUser().UserId, BuddyUserId: userId, Type: 11, ItemId: itemId });
                //});
            });
        }
        catch (e) {
            alert("sendMessage error:" + e);
        }
    };


    var pingTimer;
    this.StartPingTimer = function (interval) {
        if (pingTimer)
            clearInterval(pingTimer);
        pingTimer = setInterval(function () {
            try {
                console.log("ping");
                sendToServer(function (serverHub) {
                    console.log("ping siteVersion: " + kinkyApp.data.siteVersion);
                    if (serverHub.ping)
                        serverHub.ping(kinkyApp.data.siteVersion);
                    //else
                    //    location.reload();
                });
            }
            catch (e) {
                console.log(e);
            }
        }, interval);
    };

    this.EnableAudio = ko.observable(localStorage["EnableAudio"] == null || localStorage["EnableAudio"] == "true");

    this.ClickAudio = function (data, e) {
        console.log("ClickAudio:" + self.EnableAudio());
        self.EnableAudio(!self.EnableAudio());
        localStorage["EnableAudio"] = self.EnableAudio().toString();
    };

    this.playAudio = function () {
        if (self.EnableAudio()) {
            try {
                var snd = new Audio(kinkyApp.data.contentCdnPath + "Content/themes/dzen/images/chat.mp3?v1");
                snd.play();
            } catch (e) {
                console.log(e);
            }
        }
    };

    this.checkUser = function () {
        if (!self.MyUser().CanSendMessages) {
            kinkyApp.modal.showTopByUrl(kinkyApp.data.urlTmpl.MenuAvatarNotModeratedAlert, null, null, true);
            return false;
        }
        return true;
    };

    this.showChat = function (userId) {
        self.getUser(userId, function (user) {
            self.initChat(user, true);
            kinkyApp.analytics.trackPageView("/OpenChatDialog", "Открыто окно чата");
        }, true);
    };

    this.getUser = function (userId, callBack, refreshFromServer) {
        try {
            if (!refreshFromServer) {
                var user = ko.utils.arrayFirst(self.Users(), function (item) {
                    return userId === item.UserId;
                });
                if (user && callBack)
                    return callBack(user);
            }
            //console.info('getUser');

            sendToServer(function (serverHub) {
                serverHub.getUser(userId).done(function (result) {
                    if (result) {
                        self.putUser(result);
                        callBack && callBack(result);
                    }
                });
            });
        }
        catch (ex) {
            alert("getUser error:" + ex);
        }
    };
};
kinkyApp = kinkyApp || {};
kinkyApp.mapAsObservable = function (obj, key, value) {
    if (obj[key + "Observable"] == null)
        obj[key + "Observable"] = ko.observable(value);
    else
        obj[key + "Observable"](value);
    obj[key] = value;
};

kinkyApp.viewModel = new kinkyApp.ViewModel();
kinkyApp.videoHandlers = {};