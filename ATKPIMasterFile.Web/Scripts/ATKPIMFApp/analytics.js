if (typeof String.prototype.contains === 'undefined') {
    String.prototype.contains = function (it) { return this.toLowerCase().indexOf(it.toLowerCase()) != -1; };
}

kinkyApp.analytics = (function () {
    var evtTypes = {
        registration: {
            name: 'Registration complete',
            description: 'Регистрация завершена'
        },
        registrationSecondScreen: {
            name: 'Registration: second screen',
            description: 'Регистрация: второй шаг'
        },
        registrationThirdScreen: {
            name: 'Registration: third screen',
            description: 'Регистрация: начальная подписка'
        },
        login: {
            name: 'Login',
            description: 'Авторизация'
        },
        profileEdit: {
            name: 'Edit profile',
            description: 'Заполнение анкеты с личными данными'
        },
        socialReferrer: {
            name: 'Social referrer',
            description: 'Социальный реферрал'
        },
        firstAvatarUpload: {
            name: 'First avatar upload',
            description: 'Первая загрузка аватара'
        },
        videoBroadCasting: {
            name: 'Video broadcasting',
            description: 'Запуск трансляции'
        },
        interestSubscription: {
            name: 'Subscribtion',
            description: 'Подписка на интерес'
        },
        like: {
            name: 'Like',
            description: 'Like'
        },
        reblog: {
            name: 'Reblog',
            description: 'Reblog'
        },
        chatImageSendPc: {
            name: 'Chat image send [PC]',
            description: 'Отправка картинки из чата с ПК'
        },
        chatImageSendInterest: {
            name: 'Chat image send [Interest]',
            description: 'Отправка изображения из интереса'
        },
        chatMessageSend: {
            name: 'Chat message send',
            description: 'Отправка сообщения из чата'
        },
        settingsEdit: {
            name: 'Settings edit',
            description: 'Редактирование настроек'
        },
        flashStart: {
            name: 'Flash start',
            description: 'Запуск флэш-заявки'
        },
        textUpload: {
            name: 'Text upload',
            description: 'Загрузка текстового поста'
        },
        commentSend: {
            name: 'Comment send',
            description: 'Отправка комментария'
        },
        photoUpload: {
            name: 'Interest photo upload',
            description: 'Загрузка картинки интереса'
        },
        share: {
            name: 'Share',
            description: 'Поделились в социальной сети'
        },
        earnedGuestStatus: {
            name: 'Earned "Guest" status',
            description: 'Получил статус "Гость"'
        },
        earnedInTheKnowStatus: {
            name: 'Earned "InTheKnow" status',
            description: 'Получил статус "В теме"'
        },
        earnedMasterStatus: {
            name: 'Earned "Master" status',
            description: 'Получил статус "Мастер"'
        },
        sharedItemViaMail: {
            name: 'Shared item via mail',
            description: 'Поделился картинкой через почту'
        },
        openedSharedMailItem: {
            name: 'Opened shared mail item',
            description: 'Открыл расшаренную картинку из почты'
        },
        openedPrivateItem: {
            name: 'Opened private item',
            description: 'Открыл платное фото'
        },
        maskModeActivated: {
            name: 'Buy mask',
            description: 'Покупка маски'
        },
        premiumStatusActivated: {
            name: 'Bought Premium status',
            description: 'Купил Премиум статус'
        },
        clickedPaymentButton: {
            name: 'Clicked payment button',
            description: 'Нажатие на кнопку "Купить"'
        },
        clickedPremiumButton: {
            name: 'Clicked premium button',
            description: 'Нажатие на кнопку "Активировать премиум статус"'
        }
    };

    var eventQueue = [];

    var addEventToQueue = function (event, eventData, callback) {
        eventQueue.push({ event: event, eventData: eventData, callback: callback });

        console.log(eventQueue);
    };

    var processQueue = function () {
        var item;

        while ((item = eventQueue.shift()) != null) {
            trackEvent(item.event, item.eventData, item.callback);
        }
    };

    var mergeEventData = function (defaultData, extendedData) {
        if (defaultData == null || extendedData == null)
            return defaultData;

        for (var key in extendedData)
            if (extendedData.hasOwnProperty(key))
                defaultData[key] = extendedData[key];

        return defaultData;
    };

    var canTrack = function () {
        var mx = window.mixpanel,
            ga = window.ga;

        console.debug(['can_track >>', { mixpanel: mx, google: ga, host: document.location.host }]);

        return mx && ga && document.location.host == 'kinkylove.com';
    };

    var resetUser = function () {
        var mx = window.mixpanel,
            user = kinkyApp.viewModel != null && kinkyApp.viewModel.MyUser != null ? kinkyApp.viewModel.MyUser() : null;

        console.debug(['resetting user:>', user]);

        if (mx && user) {
            var sex = user.Sex == 1 ? 'Male' : 'Female';

            mx.identify(user.Email);
            mx.people.set({
                '$last_login': new Date(),
                'sex': sex,
                'age': user.Age,
                'internal_id': user.UserId,
                '$first_name': user.Name,
                '$email': user.Email
            });
            mx.name_tag(user.Name);
            mx.register({
                sex: sex,
                user_name: user.Name,
                internal_id: user.UserId
            });
        }
    };

    var trackEvent = function (event, eventData, callback) {
        var mx = window.mixpanel;
        
        console.debug(['tracking event:>', event]);

        if (!canTrack()) { callback && callback(); return; }

        try {
            if (typeof window.__insp !== 'undefined') {
                window.__insp.push(['tagSession', event.name]);
            }

            var eventinfo = mergeEventData({
                description: event.description
            }, eventData || {});

            if (event.name == evtTypes.registration.name) {
                var user = kinkyApp.viewModel != null && kinkyApp.viewModel.MyUser != null ? kinkyApp.viewModel.MyUser() : null;
                
                if (window.PostAffTracker && user) {
                    var sale = window.PostAffTracker.createSale();

                    sale.setTotalCost('0.0');
                    sale.setOrderID('Registration_' + user.UserId + '_' + Date.now());
                    sale.setProductID('Registration');
                    sale.setStatus('A');
                    sale.setData1('' + user.UserId);

                    window.PostAffTracker.register();
                }
                
                return;
            }

            mx.track(event.name, eventinfo);
        }
        catch (ex) {
            console.error(ex);
        }
    };

    var trackPageView = function (url, title, isAjaxRequest) {
        var ga = window.ga,
            chartbeat = window.pSUPERFLY,
            user = kinkyApp.viewModel != null && kinkyApp.viewModel.MyUser != null ? kinkyApp.viewModel.MyUser() : null;

        console.debug(['document.location.host: ' + document.location.host, { url: url, title: title, user: user }]);

        if (!canTrack()) return;

        try {
            if (isAjaxRequest) {
                ga("set", "title", title ? title : url);
            }

            url = (user && url == '/') ? "/Feed" : url;

            if (user != null) {
                var sex = (user.Sex == 1 ? 'Male' : (user.Sex == 2 ? 'Female' : 'Trans'));

                ga('set', 'dimension1', sex);

                if (!isAjaxRequest && document.referrer.contains('kinkylove.com/login')) {
                    resetUser();
                    trackEvent(evtTypes.login);
                }
            }

            ga('send', 'pageview', url);

            if (chartbeat) {
                chartbeat.virtualPage(url, title ? title : url);
            }
            else {
                setTimeout(function () {
                    chartbeat = window.pSUPERFLY;
                    chartbeat && chartbeat.virtualPage(url, title ? title : url);
                }, 1000);
            }
        }
        catch (ex) {
            console.error(ex);
        }
    };

    var init = function (landingItem) {
        var url = document.location.pathname + document.location.search;

        if (landingItem) {
            url += (url.indexOf('?') > -1 ? "&" : "?") + landingItem;
        }

        //trySetUtmCookie();

        trackPageView(url, null, false);
    };

    //function trySetUtmCookie() {

    //    var kinkHost = "kinkylove.com";

    //    if (document.location.host.indexOf("kinkylove.azurewebsites.net") > -1)
    //        kinkHost = "kinkylove.azurewebsites.net";
    //    else if (document.location.host.indexOf("localhost") > -1)
    //        kinkHost = "localhost";
        
    //    if ((document.location.search.contains("utm_") || document.location.search.contains("aid")) && document.location.host.contains(kinkHost)) {
    //        var val = document.location.search.replace('?', '');
    //        var date = new Date();
    //        date.setTime(date.getTime() + (365 * 24 * 60 * 60 * 1000));
    //        setCookie("kinkyutm", val, date.toGMTString(), "/", "." + kinkHost);
    //    }
    //    if (document.referrer) {
    //        var ind = document.referrer.indexOf(kinkHost);
    //        if (ind == -1 || ind > 20) {
    //            var date = new Date();
    //            date.setTime(date.getTime() + (365 * 24 * 60 * 60 * 1000));
    //            setCookie("kinkyref", document.referrer, date.toGMTString(), "/", "." + kinkHost);
    //        }
    //    }
    //};

    //function setCookie(name, value, expires, path, domain, secure) {
    //    document.cookie = name + "=" + escape(value) +
    //      ((expires) ? "; expires=" + expires : "") +
    //      ((path) ? "; path=" + path : "") +
    //      ((domain) ? "; domain=" + domain : "") +
    //      ((secure) ? "; secure" : "");
    //};


    return {
        init: init,
        addEventToQueue: addEventToQueue,
        processQueue: processQueue,
        trackEvent: trackEvent,
        trackPageView: trackPageView,
        resetUser: resetUser,
        eventType: evtTypes,
    };
})();