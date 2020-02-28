
atkpimfApp.PublishVideoModel = function(videoContainerId) {
    var me = this,
        devicesInitialized = false,
        videoWrapper,
        currentBroadcastSessionId = '',
        broadcastNotificationTimer;
    
    me.videoContainer = null;
    me.watchersCount = ko.observable(0);
    me.headerVisible = ko.observable(false);
    me.videoContainerVisible = ko.observable(false);
    me.videoIconVisible = ko.observable(true);
    me.testVideoButtonVisible = ko.observable(true);
    me.startBroadcastButtonVisible = ko.observable(false);
    me.broadcastingControlsVisible = ko.observable(false);
    me.isBroadcasting = ko.observable(false);
    me.isBroadcastingWindowVisible = ko.observable(false);
    me.watchers = ko.observableArray();
    me.isPrivate = ko.observable(false);
    me.currentBroadcastSessionId = '';

    this.toggleMic = function (element) {
        var el = $(element);

        if (/*el.hasClass('chat-header__mic-control-mute')*/ !me.micIsOn()) {
            el.removeClass('chat-header__mic-control-mute').addClass('chat-header__mic-control-on');

            me.videoContainer.setterJS("micGain", "80");
            me.micIsOn(true);
        }
        else {
            el.removeClass('chat-header__mic-control-on').addClass('chat-header__mic-control-mute');

            me.videoContainer.setterJS("micGain", "0");
            me.micIsOn(false);
        }
    };
    
    this.updateWatchers = function(watchers) {

        if (watchers == null) {
            return;
        }
        
//        debugger;

        var findIndexById = function(array, value) {

            for (var i = 0; i < array.length; i++) {

                if (array[i].userId == value) {
                    return i;
                }
            }
            
            return -1;
        };

        var differences = ko.utils.compareArrays(me.watchers(), watchers);

        for (var index = 0; index < differences.length; index++) {
            var item = differences[index];

            if (item.status == 'added') {
                me.watchers.push({
                        userId: item.value.UserId,
                        name: item.value.Name,
                        coins: item.value.Coins,
                        cash: item.value.Cash
                });
            }
            else if (item.status == 'deleted') {
                var indexOfItem = findIndexById(me.watchers(), item.value.userId);

                if (indexOfItem > -1) {
                    me.watchers.splice(indexOfItem, 1);
                }
            }

            me.watchers.sort(function (left, right) {
                return left.coins == right.coins ? 0 : (left.coins < right.coins ? 1 : - 1);
            });
        }
    };
    
    
    var setUIToTestView = function() {
        me.headerVisible(false);
        me.videoContainerVisible(false);
        me.videoIconVisible(true);
        me.testVideoButtonVisible(true);
        me.startBroadcastButtonVisible(false);
        me.broadcastingControlsVisible(false);
    };

    var setUIToStartView = function() {
        me.headerVisible(false);
        me.videoContainerVisible(true);
        me.videoIconVisible(false);
        me.testVideoButtonVisible(false);
        me.startBroadcastButtonVisible(false);
        me.broadcastingControlsVisible(false);
    };
    
    var setUIToBroadcastingView = function () {
        me.headerVisible(true);
        me.videoContainerVisible(true);
        me.videoIconVisible(false);
        me.testVideoButtonVisible(false);
        me.startBroadcastButtonVisible(false);
        me.broadcastingControlsVisible(true);
        
        /*$(".translation-notify-modal__translation-container").css({ "width": "100%", "background": "#363636", "padding": "9px 0" });
        $(".translation-notify-modal").css({ "margin": "0", "position": "absolute", "top": "10px", "left": $(window).width() - 350 });
        */
        $(".translation-notify-modal__translation-container").css({ "width": "100%", "background": "#363636" });
        $(".translation-notify-modal").css({ "margin": "0", "position": "absolute", "top": "10px", "left": $(window).width() - 430 });
    };

    me.notifierStarted = ko.observable(false);

    var notifyAboutBroadcast = function(videoId, isPrivate) {
        try {
            if (atkpimfApp.viewModel.broadcastingModel.isBroadcasting()) {
                var broadcastVideo = $('#' + videoId).length > 0 ? $('#' + videoId)[0] : null;

                if (broadcastVideo != null) {
                    
                    var screen = broadcastVideo.GetScreen();

                    if (screen.length > 10000) {
                        
                        atkpimfApp.asyncRequest({
                            url: atkpimfApp.data.urlTmpl.StreamTalkNotifyUsersOfBroadcasting,
                            type: "POST",
                            data: {
                                sessionId: me.currentBroadcastSessionId,
                                image: screen,
                                isPrivate: !!isPrivate
                            },
                            success: function() {
                                console.log('model is broadcasting');
                            }
                        });
                    }
                }
                else {
                    me.notifierStarted(false);
                }

                setTimeout(function() {
                    if (me.notifierStarted()) {
                        notifyAboutBroadcast(videoId, isPrivate);
                    }
                }, 60000);
            }
        }
        catch(err) {
            console.error(err);
        }
    };
    
    me.startModelBroadcasting = function(isPrivate) {
        try {
            //            debugger;
            me.notifierStarted(false);
            me.isPrivate(isPrivate);
            atkpimfApp.asyncRequest({
                url: atkpimfApp.data.urlTmpl.StreamTalkStartBroadcasting,
                type: "POST",
                success: function (data) {
                    me.currentBroadcastSessionId = data.sessionId;

                    videoWrapper = $('#' + videoContainerId).clone();
                    
                    var videoId = 'broadcast-publisher';
                    var videoHandlerName = 'handleMyBroadcast_' + me.currentBroadcastSessionId;

                    atkpimfApp.videoHandlers[videoHandlerName] = function(state) {
                        
                        if (state == 33) {
                            var vid = $('#' + videoId).length > 0 ? $('#' + videoId)[0] : null;
                            
                            if (vid && vid.setterJS) {
                                vid.setterJS("publish", "start");
                            }
                        }
                        else if (state == 22) {
                            var v = $('#' + videoId).length > 0 ? $('#' + videoId)[0] : null;

                            if (v != null) {
                                v.setterJS("cameraQ", "99");
                                v.setterJS("cameraW", "800");
                                v.setterJS("cameraH", "600");
                                v.setterJS("micGain", "0");
                            }

//                            debugger;
                            if (!me.notifierStarted()) {
                                me.notifierStarted(true);
                                setTimeout(function () {
                                    notifyAboutBroadcast(videoId, isPrivate);
                                }, 3000);
                            }
                        }
                        else if (state == 17) {
                            var video = $('#' + videoId).length > 0 ? $('#' + videoId)[0] : null;

                            if (video != null) {
                                video.setterJS("cameraQ", "99");
                                video.setterJS("cameraW", "800");
                                video.setterJS("cameraH", "600");
                                video.setterJS("micGain", "0");

                                {
                                    var watchdogVideoId = 'watchdog-' + videoId;
                                    var watchdogHandlerName = 'watchdog_' + me.currentBroadcastSessionId;
                                    
                                    atkpimfApp.videoHandlers[watchdogHandlerName] = function(watchdogState) {
                                        console.info('watchdog handler: ' + watchdogHandlerName + ', current state: ' + watchdogState);
                                        
                                        var watchdogVideo = $('#' + watchdogVideoId).length > 0 ? $('#' + watchdogVideoId)[0] : null;

                                        if (watchdogVideo != null) {
                                            if (watchdogState == 10) {
                                                console.info('watchdog video initialized properly');

                                                watchdogVideo.setterJS('volume', '0');
                                            }
                                            else if (watchdogState == 9) {
                                                me.stopBroadcasting();
                                                setTimeout(function() {
                                                    me.startModelBroadcasting(isPrivate);
                                                }, 1000);
                                            }
                                        }
                                    };
                                    
                                    $('#broadcasting-watchdog-container')
                                        .children().first()
                                        .attr('id', watchdogVideoId);

                                    var wFlashvars = {
                                        __type: 'viewer',
                                        __rtmp: 'rtmp://88.198.39.28:1940/',
                                        __stream: data.sessionId,
                                        __autoplay: '1',
                                        __log: '1',
                                        __sign: data.signature,
                                        __setStatusJS: 'atkpimfApp.videoHandlers["' + watchdogHandlerName + '"]'
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
                                //debugger;
                                if (!me.notifierStarted()) {
                                    me.notifierStarted(true);
                                    setTimeout(function () {
                                        notifyAboutBroadcast(videoId, isPrivate);
                                    }, 3000);
                                }
                            }
                        }
                    };
                    var flashvars = {
                        __type: 'publisher',
                        __rtmp: 'rtmp://88.198.39.28:1940/',
                        __stream: data.sessionId,
                        __autoplay: '0',
                        __log: '1',
                        __sign: data.signature,
                        __getParamFunJS: 'getVideoParamsHandler', //TODO: repair this shit
                        __setStatusJS: 'atkpimfApp.videoHandlers["' + videoHandlerName + '"]'
                    };
                    var params = {
                        menu: 'true',
                        allowFullscreen: 'true',
                        wmode: 'window',
                        allowScriptAccess: 'sameDomain'
                    };
                    var attributes = { id: videoId, name: videoId, style: 'padding: 10px 0px 0px 10px;' };

                    swfobject.embedSWF('/client.swf', videoContainerId, '410', '307', '11.0.0', '/expressInstall.swf', flashvars, params, attributes);
                    me.videoContainer = $('#' + videoId).length > 0 ? $('#' + videoId)[0] : null;

                    if (me.videoContainer) {
                        devicesInitialized = true;

                        setUIToBroadcastingView();
                        me.isBroadcastingWindowVisible(true);
                        me.isBroadcasting(true);
                    }
                }
            });
        }
        catch (error) {
            alert(error);
        }
    };
    
  
    me.micIsOn = ko.observable(true);

    me.stopBroadcasting = function () {
//        debugger;
        if (me.currentBroadcastSessionId != '') {
            me.notifierStarted(false);
            me.videoContainer.setterJS("unpublish", "start");
            me.micIsOn(false);

            $('#broadcasting-watchdog-container').html('<div></div>');
            
            var videoHandlerName = 'handleMyBroadcast_' + me.currentBroadcastSessionId;
            var watchdogHandlerName = 'watchdog_' + me.currentBroadcastSessionId;
            
            delete atkpimfApp.videoHandlers[videoHandlerName];
            delete atkpimfApp.videoHandlers[watchdogHandlerName];
            
            sendToServer(function (serverHub) {
                serverHub.endBroadcasting();
            });

            me.currentBroadcastSessionId = '';
        }

        setUIToTestView();

        var containerParent = $(me.videoContainer).parent();

        $(me.videoContainer).remove();
        videoWrapper && videoWrapper.appendTo(containerParent);

        me.isBroadcasting(false);
        me.isBroadcastingWindowVisible(false);
    };

    me.show = function () {
        me.stopBroadcasting();
        me.isBroadcastingWindowVisible(true);
    };

    me.hide = function () {
        me.stopBroadcasting();
        me.isBroadcastingWindowVisible(false);
    };
};

getVideoParamsHandler = function(id, param, value) {
    if (atkpimfApp.viewModel && atkpimfApp.viewModel.broadcastingModel) {
        if (param == 'screen') {

        }
        console.debug('param: ' + param);
        console.debug('value: ' + value);
    }
};