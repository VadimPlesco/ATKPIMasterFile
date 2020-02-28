
atkpimfApp.WatchVideoModel = function(chat, userId) {
    var me = this,
        currentBroadcastSessionId = '',
        forcedClose = false;

    var successfulConnectionHandler = function(sessionId, signature) {
        currentBroadcastSessionId = sessionId;
//        debugger;
        chat.buddyUsersVideoSessionId(sessionId);
        chat.buddyUsersSignature(signature);
        chat.initBuddyUsersVideo(true);

    };
    
    me.connect = function (currentSessionId, source) {
        console.log('*------------------------------*');
        console.log('current session id: ' + currentBroadcastSessionId);
        console.log('new session id: ' + currentSessionId);
        console.log('source: :' + source);
        console.log('forced close: ' + forcedClose);
        console.log('*------------------------------*');
        if ((currentBroadcastSessionId == '' || currentBroadcastSessionId != currentSessionId) && !forcedClose) {
            sendToServer(function(serverHub) {
                serverHub.connectToBroadcasting(userId, currentSessionId)
                    .done(function (data) {
                        console.log('@------------------------------@');
                        console.log('current session id: ' + currentBroadcastSessionId);
                        console.log('new session id: ' + currentSessionId);
                        console.log('source: :' + source);
                        console.log('data: :' + JSON.stringify(data));
                        console.log('@------------------------------@');
                        successfulConnectionHandler(data.sessionId, data.signature);
                    });
            });
        }
        else {
            chat.initBuddyUsersVideo();
        }
    };

    me.disconnect = function (forcedBroadcastClose) {
//        debugger;
        forcedClose = !!forcedBroadcastClose;

        if (currentBroadcastSessionId != '') {
            sendToServer(function (serverHub) {
//                debugger;
                serverHub.disconnectFromBroadcasting(userId);
            });

            currentBroadcastSessionId = '';
            chat.currentBroadcastSessionId = '';
        }
    };
};