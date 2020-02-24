kinkyApp.fn.initCheckbox = function (selector, ajaxRequestFunc, onChangeCallback) {
    //alert('1');
    if ($(selector).attr("data-initCheckbox")) return;
    $(selector).attr("data-initCheckbox", "true");

    $(selector).mousedown(
        function () {
            kinkyApp.fn.changeCheck($(this), onChangeCallback);
        });

    if (ajaxRequestFunc)
        $(selector).mouseup(function () {
            var $this = $(this);
            $this.addClass('loading');
            ajaxRequestFunc($this.find("input[type='hidden']"), function () {
                $this.removeClass('loading');
            });
        });

    $(selector).each(
        function () {
            kinkyApp.fn.changeCheckStart($(this));
        });
};

kinkyApp.fn.changeCheck = function (el, onChangeCallback) {
    //alert('2');
    input = el.find("input").eq(0);
    input_hidden = el.find("input[type='hidden']").eq(0);
    offset = input.attr("offset") ? input.attr("offset") : input_hidden.attr("offset");
    onChangeCallback = onChangeCallback || function () { };
    ///alert(offset);
    if (!input.attr("checked")) {
        el.css("background-position", "0 -" + (offset ? offset : 20) + "px");
        input.attr("checked", true).attr("value", true);
        input_hidden.attr("checked", true).attr("value", true);

        onChangeCallback(input, true);
    } else {
        el.css("background-position", "0 0");
        input.attr("checked", false).attr("value", false);
        input_hidden.attr("checked", false).attr("value", false);

        onChangeCallback(input, false);
    }

    return true;
};

kinkyApp.fn.changeCheckStart = function (el) {
    //alert('3');
    var input = el.find("input[type='checkbox']").eq(0);
    var input_hidden = el.find("input[type='hidden']").eq(0);
    offset = input.attr("offset") ? input.attr("offset") : input_hidden.attr("offset");

    if (input_hidden.attr("value") == "true" || input_hidden.attr("value") == "True" || input.attr("checked") == "checked") {
        input_hidden.attr("checked", true);
        el.css("background-position", "0 -" + (offset ? offset : 20) + "px");
    }
    return true;
};

//function parentCheckBoxClick(id) {
//    $("." + id).each(function (index) {
//        var offset = $(this).attr("offset");
//        if($("#"+id).attr("checked")=="checked"){
//            $(this).attr("checked", true).attr("value","true");
//            $(this).parent("span").parent("span.shared__checkbox-small").css("background-position", "0 -" + (offset ? offset : 20) + "px");
//        } else {
//            $(this).removeAttr("checked").attr("value", "false");
//            $(this).parent("span").parent("span.shared__checkbox-small").css("background-position", "0 0");
//        }
//    });

//}

kinkyApp.fn.childrenCheckBoxClick = function (id) {
    //var children = $("." + id);
    var childrenChecked = $("." + id + "[checked='checked']");
    if (childrenChecked.length == 0) {
        $("#" + id).removeAttr("checked").attr("value", "false");
        $("#" + id).parent("span").parent("span.shared__checkbox-small").css("background-position", "0 0");
    } else {
        $("#" + id).attr("checked", true).attr("value", "true");
        $("#" + id).parent("span").parent("span.shared__checkbox-small").css("background-position", "0 -" + (offset ? offset : 20) + "px");
    }
    //console.log(childrenChecked.length);
};