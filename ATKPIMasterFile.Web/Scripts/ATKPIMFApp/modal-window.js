atkpimfApp.modal = (function () {
    var self = {};

    $(document).ready(function () {
        $("#main__overflow").on('click', function (e) {
            if (e.target !== this)
                return;
            else if ($("#main__overflow").attr("data-isDenyClose") != "true")
                self.hide();
        });

        $("#main__overflow-top").on('click', function (e) {
            if (e.target !== this)
                return;
            else if ($("#main__overflow-top").attr("data-isDenyClose") != "true")
                self.hide();
        });
    });

    //self.show = function (html, isDenyCloseByOverflowClick, setUrlInAddressBar) {
    //    $("body").css("overflow", "hidden"); 
    //    $("#main__overflow").css({ "overflow": "auto" }).show().attr("data-isDenyClose", "" + isDenyCloseByOverflowClick).scrollTop(0);
     
    //    var n = $("#main_modal").length; //alert("aaa"+n);
    //    $("#main_modal").css("top", 50).html(html).show().attr("data-setUrlInAddressBar", "" + setUrlInAddressBar); //195
        
    //    //$(".modal-header__tab-item:first").addClass("modal-header__tab-item-selected"); alert("4");
    //    //$(".modal__modal-content:first").show();
    //    var n = $(".main__modal").length; //alert("bbb" + n);
    //    //$(".modal__modal-content").show();
    //    $(".main__modal").show();
    //};

    self.show = function (html, isDenyCloseByOverflowClick, setUrlInAddressBar) {
        $("body").css("overflow", "hidden");
        $("#main__overflow").css({ "overflow": "auto" }).show().attr("data-isDenyClose", "" + isDenyCloseByOverflowClick).scrollTop(0);

        $("#main_modal").css("top", 50).html(html).show().attr("data-setUrlInAddressBar", "" + setUrlInAddressBar); //195
        $(".modal-header__tab-item:first").addClass("modal-header__tab-item-selected");
        $(".modal__modal-content:first").show();
    };

    self.showTop = function (html) {
        $("body").css("overflow", "hidden");
        $("#main__overflow").css("overflow", "hidden");
        $("#main_modal-top").css({"opacity": 0, "bottom": "-10%"}).show(); 
        $("#main_modal-top").html(html); //195
        $("#main_modal-top").animate({ "opacity": 1, "bottom": "-5%" }, 600);
        $("#main__overflow-top").css({ "overflow": "auto" }).scrollTop(0).show();  
    };

    self.hide = function (el) {

        if (el && $(el).parents("#main__overflow-top").length > 0)
        {
            self.hideTop();
            return;
        }

//        if (atkpimfApp.viewModel && atkpimfApp.viewModel.publishVideoModal && !atkpimfApp.viewModel.publishVideoModal.isPublishing) {
//            atkpimfApp.viewModel.publishVideoModal.hide();
        //        }
        
        if (atkpimfApp.viewModel && atkpimfApp.viewModel.broadcastingModel && atkpimfApp.viewModel.broadcastingModel.isBroadcastingWindowVisible() && !atkpimfApp.viewModel.broadcastingModel.isBroadcasting()) {
            atkpimfApp.viewModel.broadcastingModel.hide();
        }
        
        $("#main__overflow").children(".main__modal").hide();
        $("#main__overflow").scrollTop(0).hide();
        $(".modal-header__tab-item").removeClass("modal-header__tab-item-selected");
        $("body").css("overflow", "auto");
        $("#main_modal").html("");
        if ($("#main_modal").attr("data-setUrlInAddressBar") == "true")
            atkpimfApp.replaceState();
    };

    self.hideTop = function () {
        $("body").css("overflow", "auto");
        $("#main__overflow-top").scrollTop(0).hide().children(".main__modal-top").hide();
        $("#main__overflow").css("overflow", "auto");
        $("#main_modal-top").html("");
    };

    self.hideAndReloadBody = function () {
        self.hide();
        atkpimfApp.reloadBody();
    };

    self.showByUrl = function (url, event, setUrlInAddressBar, isDenyCloseByOverflowClick, successCallBack) {
        //console.log(url);
        //if (atkpimfApp.fn.isMobile()) {
        //    return true;
        //}

        var data = {
        };

        $.ajax({
            url: url,
            contentType: 'application/html',
            data: data,
            success: function (content) {
                //alert(content);
                //console.log(content);
                //$('#main_modal').html(content.Html);
                self.hideTop(); //console.log(content);
                //$('#main_modal').html(content.Html);
                self.show(content, isDenyCloseByOverflowClick, setUrlInAddressBar);
                
                if (successCallBack)
                    successCallBack();
            },
            error: function (e) { console.log(e); }
        });
        
        //atkpimfApp.asyncRequest({
        //    url: url,
        //    id: "main_modal",
        //    setUrlInAddressBar: setUrlInAddressBar,
        //    success: function (data) {
        //        self.hideTop(); console.log(data);
        //        self.show(data.Html, isDenyCloseByOverflowClick, setUrlInAddressBar);
        //        //self.show('<p>Hello!</p>', isDenyCloseByOverflowClick, setUrlInAddressBar);
        //        if (successCallBack)
        //            successCallBack();
        //    }
        //});


        if (event) {
            if (event.stopPropagation)
                event.stopPropagation();   // W3C model
            else
                event.cancelBubble = true; // IE model
        }

        return false;
    };

    self.showTopByUrl = function (url) {
        atkpimfApp.asyncRequest({
            url: url,
            id: "main_modal-top",
            success: function (data) {
                self.showTop(data.Html);
            }
        });
        return false;
    };

    function IsShown() {
        return $("#main__overflow").css("display") != "none";// && $("#main_modal").attr("data-setUrlInAddressBar") == "true";
    }

    self.showByUrlInCurrentLevel = function (url) {
        //console.log(url);
        if (IsShown())
            self.showByUrl(url, null, true);
        else
            atkpimfApp.updateElementById(url, "body");

        return false;
    };

    return self;
})();

atkpimfApp.preventBodyScrolling = function () {
    return false;
};