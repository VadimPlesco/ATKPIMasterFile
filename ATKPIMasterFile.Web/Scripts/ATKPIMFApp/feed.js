atkpimfApp.FeedScroll = function (scrollSelector) {
    this.Url = "";
    this.loading = false;
    this.timer = null;
    this.feedGridModel = null;
    this.feedBlockSelector = ".feed-block";
    this.feedwrapperId = "feedwrapper";
    this.scrollSelector = scrollSelector;
    this.showPaging = false;
    var self = this;

    this.init = function (url, feedwrapperId, feedBlockSelector, feedwrapperMaxHeight, showPaging) {
        self.Url = url;
        self.feedBlockSelector = feedBlockSelector;
        self.feedwrapperId = feedwrapperId;
        self.showPaging = showPaging;

        if (self.feedBlockSelector != null) {
            try {
                if (self.timer == null) {
                    $(window).on("resize", function () {
                        self.timer && clearTimeout(self.timer);
                        self.timer = setTimeout(function () { self.feedGridModel.setupBlocks(self.feedwrapperId, self.feedBlockSelector); }, 200);
                    });

                    $(self.scrollSelector).scroll(self.onScroll);
                }
                if (self.feedGridModel == null)
                    self.feedGridModel = new atkpimfApp.FeedGridModel(feedwrapperMaxHeight);
                self.feedGridModel.setupBlocks(self.feedwrapperId, self.feedBlockSelector);
            } catch (e) {
                alert(e);
            }
        }
        else
            $(self.scrollSelector).scroll(self.onScroll);
    };

    var getScrollHeight = function (selector) {
        if (selector == window) {
            if (self.isEarly)
                return $(document).height() / 3 * 2;
            return $(document).height() - 1000;
        }
        return $(selector)[0].scrollHeight - 300;
    };

    var direction = 0;

    this.moreClick = function (el, clickUrl) {
        document.getElementById(self.feedwrapperId).removeChild(el);
        self.Url = clickUrl;
        self.onScroll();
    };

    this.onScroll = function () {
        if (document.getElementById(self.feedwrapperId) != null) {

            var curScroll = $(self.scrollSelector).scrollTop();
            var isBottomScroll = curScroll > direction;
            direction = curScroll
           
            if (self.showPaging) {
                //var t = "";

                var items = $("#" + self.feedwrapperId).find("[data-page]");

                for (var i = items.length - 1; i >= 0; i--) {
                    var el = items[i];
                    //var prevP = '' + $(el).attr('data-page');
                    //var nextP = '' + $(el).attr('data-pageNew');
                    //prevP = prevP.substr(prevP.length - 1, 1);
                    //nextP = nextP.substr(nextP.length - 1, 1);
                    //t += i + ') prevP:' + prevP + ' nextP:' + nextP + ' elOffset:' + elOffset(el) + " " + el.offsetTop + ' winOffset:' + window.pageYOffset + "<br>";

                    var elTop = elOffset(el);
                    if (elTop < window.pageYOffset) {
                        var url = $(el).attr('data-pageNew');
                        if ((document.location.pathname + document.location.search) != url) {
                            window.history.replaceState({ url: url, id: "body", pageTitle: document.title }, document.title, url);
                        }
                        break;
                    }

                    if (i == 0 && elTop > window.pageYOffset) {
                        var url = $(el).attr('data-page');
                        if ((document.location.pathname + document.location.search) != url)
                            window.history.replaceState({ url: url, id: "body", pageTitle: document.title }, document.title, url);
                    }

                }

                //$('.feed-category-title-text-container').html("<span style='color:white;'>" + t + "</span>");
            }

            if (!isBottomScroll)
                return;
            //console.info("isBottomScroll");

            if (curScroll >= getScrollHeight(self.scrollSelector) - $(self.scrollSelector).height()
                && self.loading == false) {

                if (self.Url == null || self.Url == "")
                    return;
                self.loading = true;

                atkpimfApp.asyncRequest({
                    url: self.Url,
                    //replaceUrlInAddressBar: self.showPaging,
                    id: self.showPaging ? self.feedwrapperId : null,
                    success: function (data) {
                        if (data.Html) {
                            if (self.showPaging) {
                                $('#paging').remove();
                                var div = $("<div data-page='" + document.location.href + "' data-pageNew='" + data.Url + "' >" +
                                    //"<hr style='width:10px'/>" + data.Url.substr(data.Url.length - 1, 1) +
                                    "</div>");
                                $("#" + self.feedwrapperId).append(div);
                                if (self.feedBlockSelector)
                                    div.addClass(self.feedBlockSelector.replace('.', ''));
                                else
                                    div.css("float", "left").css("position", "relative");
                                //console.log(data.Ur);
                            }
                            $("#" + self.feedwrapperId).append(data.Html);
                            self.feedGridModel && self.feedGridModel.setupBlocks(self.feedwrapperId, self.feedBlockSelector);
                            self.Url = data.CustomData;
                        }
                        else {
                            self.Url = null;                           
                        }
                    },
                    complete: function (data) {
                        self.loading = false;
                    }
                });
            }
        }
    };

    function elOffset(el) {
        var top = el.offsetTop;
        while (el.offsetParent) {
            el = el.offsetParent;
            top += el.offsetTop;
        }
        return top;
    }

    this.setupBlocks = function () {
        self.feedGridModel && self.feedGridModel.setupBlocks(self.feedwrapperId, self.feedBlockSelector);
    }
};

atkpimfApp.feedScroll = new atkpimfApp.FeedScroll(window);
atkpimfApp.feedScrollMainOverflow = new atkpimfApp.FeedScroll('#main__overflow');

