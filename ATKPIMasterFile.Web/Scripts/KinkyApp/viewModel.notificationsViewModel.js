kinkyApp.NotificationsViewModel = function (parentViewModel) {
    var parentViewModel = parentViewModel;
    var self = this;

    this.Notifications = ko.observableArray([]);
        
    this.addLine = function (element, index, data) {
        if (element.nodeType === 1) {
            $(element).css('opacity', 0).slideDown('slow').animate({ opacity: 1 }, { queue: false, duration: 'slow' }, 'easeOutQuint');
        }
    };

    this.removeLine = function (element, index, data) {
        if (element.nodeType === 1) {
            $(element).animate({ opacity: 0 }, { queue: false, duration: 'slow' }, 'easeOutQuint').slideUp('slow', null, function () {
                $(element).remove();
            });
        }
    };


    this.freezeNotification = function (data) {
        data.freeze = true;
    };


    this.unfreezeNotification = function (data) {
        if (data.needDelete) self.Notifications.remove(data);
        else data.freeze = false;
    };

    this.addNotification = function (data) {
        if (document.getElementById("fixed-top-bar") != null) {
            $("#notify-holder").css({ "top": "80px" });
        } else {
            $("#notify-holder").css({ "top": "10px" });
        }




        if (data.Type == 2 || data.Type == 3 || data.Type == 5 || data.Type == 6 || data.Type == 9 || data.Type == 12) data.lifeTime = 10000;
        else data.lifeTime = 8000;

        data.Clicked = ko.observable(false);
        this.Notifications.push(data);
        setTimeout(function () {
            if (data.freeze)
                data.needDelete = true;
            else
                self.Notifications.remove(data);
        }, data.lifeTime);
    };

    this.closeNotification = function (data, event) {
        self.Notifications.remove(data);
    };


    this.clickAdvertNotification = function (data, event) {
        parentViewModel.sendAdvertClick(data.LiteUser.UserId, data.ItemId);
        data.Clicked(true);
    };

    this.clickAvatarNotification = function (data, event) {
        if (data.Type == 1 || data.Type == 2 || data.Type == 3 || data.Type == 4 || data.Type == 40 || data.Type == 5 || data.Type == 6 || data.Type == 9 || data.Type == 12)
            kinkyApp.updateElementById(data.LiteUser.Url, 'body');
        if (data.Type == 7 || data.Type == 10)
            kinkyApp.modal.showByUrl(data.Url, null, true);
    };

    this.clickBodyNotification = function (data, event) {       
        if (data.Type == 2 || data.Type == 3 || data.Type == 5 || data.Type == 7 || data.Type == 14)
            kinkyApp.modal.showByUrl(data.Url, null, true);
        if (data.Type == 4)
            kinkyApp.updateElementById(data.Url, 'body');
        if (data.Type == 6)
            parentViewModel.showChat(data.LiteUser.UserId);
        if (data.Type == 40)
            kinkyApp.updateElementById(data.LiteUser.Url, 'body');
    };
};

/* $.connection.eventsHub.server.sendFakeNotyNotification(1,0,1000000224) */
